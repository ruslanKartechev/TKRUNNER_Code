using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TKRunner;
using General;
public class PlayerDragControllRus : MonoBehaviour
{
    #region Singleton
    private static PlayerDragControllRus _default;
    public static PlayerDragControllRus Default => _default;
    #endregion

    [Header("Options")]
    [SerializeField] private float _grabMaxDistance;
    [Space(5)]
    [SerializeField] private float _autoAimMaxVelocity;
    [SerializeField] private float _autoAimThrowForce;
    [SerializeField] private float _upperDragPlaneSize;
    [Space(5)]
    [Header("Sub Components")]
    [SerializeField] private DragPlane _dragPlanePrefab;
    [SerializeField] private Joint _dragJointPrefab;
    [Space(5)]
    [Header("Visual Effects")]
    [SerializeField] PlayerDragEffectsManager effectManager;


    private Joint _dragPoint;
    private DragPlane _dragPlane;
    private IDraggable _draggingNow;

    // Ruslan's
    [Space(5)]
    [SerializeField] private DragVelocityCalculator _calculator;
    [SerializeField] private LayerMask DragMask;
    [SerializeField] private float AimCheckTime;
    [SerializeField] int moveError = 1;
    private PlayerManager manager;
    private PlayerController controller;
    
    private bool allowDrag = false;
    bool TargetShown = false;

    private DummyTarget CurrentAim = null;
    private Coroutine movingDP;
    float elapsed = 0f;
    //Vector2 oldPos = Vector2.zero;
    //Vector2 newPos = Vector2.one;

    private Vector2 virtualPosition = new Vector2();

    private void Awake()
    {
        _default = this;
        if (manager == null)
            manager = GetComponent<PlayerManager>();
        GameManager.Instance.eventManager.LevelEndreached.AddListener(OnLevelEnd);
        virtualPosition.x = Screen.width / 2;
        virtualPosition.y = Screen.height / 2;
    }

    private void OnLevelEnd()
    {
        if(_draggingNow != null)
        {
            _draggingNow.DragEnd();
            DestroyDragPoint();
            effectManager.HideEffectDrag();
        }
           
    }


    public void Init(PlayerController _controller, PlayerManager _manager)
    {
        controller = _controller;
        manager = _manager;
    }

    public void AllowDrag()
    {
        allowDrag = true;
    }
    public void DisallowDrag()
    {
        allowDrag = false;
    }

    private void Update()
    {
        #region DragPointsManagement

        if (allowDrag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_dragPoint == null)
                {
                    CreateDragPoint(Input.mousePosition);
                }
                else
                {
                    if (_draggingNow as MonoBehaviour == null)
                        DestroyDragPoint();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (_draggingNow as MonoBehaviour != null)
                {
                    //AutoAimThrow();
                    manager.OnDragEnd(DestroyAnimated);
                }
            }
        }
        #endregion

    }


    private IEnumerator MovingDragPoint()
    {
        Vector3 oldPos = virtualPosition;
        Vector3 newPos = virtualPosition;
        while (_dragPoint != null)
        {
            newPos = Input.mousePosition;
            Vector2 delta = newPos - oldPos;
            virtualPosition += delta * GameManager.Instance.data.currentInst.Data.dragData.DragSensitivity;
            MoveDragPoint(virtualPosition);
            //if (elapsed >= AimCheckTime && delta.magnitude < moveError)
            //{
            //    if(TargetShown == false)
            //        ShowAimTarget();
            //}
            //if (delta.magnitude > moveError)
            //{
            //    HideAimTarget();
            //    elapsed = 0f;
            //}

            elapsed += Time.deltaTime;
            oldPos = newPos;
            yield return null;
        }
        TargetShown = false;
        HideAimTarget();
    }

    private DummyTarget ShowAimTarget()
    {
        CurrentAim =  GameManager.Instance.dummyController.GetClosestTarget();
        if (CurrentAim != null)
            CurrentAim.ShowAim();
        TargetShown = true;
        return CurrentAim;
    }
    private void HideAimTarget()
    {
        TargetShown = false;
        CurrentAim?.HideAim();
        CurrentAim = null;

    }

    private void AutoAimThrow()
    {
        if(CurrentAim == null)
        {
            return;
        }
        _draggingNow.FlyToTarget(CurrentAim.GetGameObject().transform, _autoAimMaxVelocity);
    }
    public void DestroyAnimated()
    {
        AutoAimThrow();
        DestroyDragPoint();
    }





    private void CreateDragPoint(Vector2 screenPosition) 
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, _grabMaxDistance, DragMask)) 
        {
            IDraggable draggable = hit.collider.GetComponentInParent<IDraggable>();

            if(draggable != null && draggable.AllowDrag() == false )
            {
                return;
                Rigidbody target = hit.collider.attachedRigidbody;
                _dragPoint = Instantiate(_dragJointPrefab);
                _dragPoint.transform.SetParent(null);
                _dragPoint.transform.position = hit.point;
                _dragPoint.connectedBody = target;
                effectManager.ShowEffectNotAllowed(_dragPoint);
                return;
            }

            if (draggable != null) 
            {
                //
                controller.IKmanager.StartHandIK();
                //
                if (draggable.IsActive())
                {
                    manager.OnDragStart();
                    _draggingNow = draggable;

                    Rigidbody target = draggable.GetRigidBody() ;
                    target.isKinematic = false;
                        // hit.collider.attachedRigidbody;

                    _dragPoint = Instantiate(_dragJointPrefab);
                    _dragPoint.transform.SetParent(null);
                    _dragPoint.transform.position = hit.point;
                    _dragPoint.connectedBody = target;
                    _calculator.SetTarget(_dragPoint.transform.gameObject.GetComponent<Rigidbody>());
                    _calculator.StartCalculator();


                    effectManager.ShowEffectDrag(_dragPoint);
                    if (draggable.CreateDragAura() == true)
                        effectManager.InstAura(target.transform, Vector3.zero);//draggable.AuraOffset()) ;

                    draggable.GetGameObject().transform.position = draggable.GetGameObject().transform.position;
                    _draggingNow?.DragStart(SpawnDragPlane(_dragPoint.transform.position, draggable.GetPlaneHeight() ), 
                        _dragPoint, BreakDrag);
                    
                }
            }
            virtualPosition = Camera.main.WorldToScreenPoint(_dragPoint.transform.position);
            if (movingDP != null)
                StopCoroutine(movingDP);
            movingDP = StartCoroutine(MovingDragPoint());
        }
    }

    public void BreakDrag()
    {
  
        if(movingDP != null)
            StopCoroutine(movingDP);
        manager.OnDragBroken();
        _calculator.StopStopCalculator();
        DestroyAnimated();
    }

    private void MoveDragPoint(Vector2 newPos) 
    {

        Ray ray = Camera.main.ScreenPointToRay(newPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("DragPlane")))
        {
            _dragPoint.transform.position = hit.point;
            controller.IKmanager.Move(hit.point);
        }

    }

    private void DestroyDragPoint() 
    {
        controller.IKmanager.StopHandIK();
        if (_dragPoint)
        {
            Destroy(_dragPoint.gameObject);
            if(_dragPlane!=null)
                Destroy(_dragPlane.gameObject);
            _draggingNow?.DragEnd();
            _draggingNow = null;

            // _dragEffect?.Disable();
            effectManager.HideEffectDrag();
        }
    }




    private DragPlane SpawnDragPlane(Vector3 dragPosition, float height) 
    {
        _dragPlane = Instantiate(_dragPlanePrefab);
        Vector3 targetPosition = dragPosition;
        targetPosition.y = transform.position.y;
        _dragPlane.transform.SetParent(transform);
        _dragPlane.transform.position = 
            transform.position + transform.up * height; // dragPOs
        //_dragPlane.transform.LookAt(transform.forward);

        _dragPoint.transform.rotation = transform.rotation;

        _dragPlane.transform.rotation *= Quaternion.Euler(Vector3.up * 180f);
        _dragPlane.Init(_upperDragPlaneSize, Vector3.Distance(_dragPlane.transform.position, transform.position) - 1f);

        return _dragPlane;
    }

    private IEnumerator MoveTrailCoroutine(Transform trail, Vector3 position) 
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        trail.position = position;
    }


    private void OnDisable()
    {
        DestroyDragPoint();
    }

    private void OnDrawGizmos()
    {
        if (_dragPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_dragPoint.transform.position, 0.3f);
        }
    }
}
