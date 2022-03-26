using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TKRunner;
using Commongame;
public class PlayerDragControllRus : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private float _grabMaxDistance;
    [Space(5)]
    [Header("Sub Components")]
    [SerializeField] private DragPlane _dragPlanePrefab;
    [SerializeField] private Joint _dragJointPrefab;
    [Space(5)]
    [Header("Visual Effects")]
    [SerializeField] private DragEffectsManager effectManager;
    [Space(5)]
    [SerializeField] private DragConstraintManager _constraintManager;

    //private Joint _dragPoint;
    private Transform _CurrentTarget;
    private Transform _CurvePoint;

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

    private DummyTarget CurrentAim = null;
    private Coroutine movingDP;
    private Vector2 virtualPosition = new Vector2();


    private void Start()
    {
        if (manager == null)
            manager = GetComponent<PlayerManager>();

        virtualPosition.x = Screen.width / 2;
        virtualPosition.y = Screen.height / 2;
    }


    private void Update()
    {
        #region DragPointsManagement

        if (allowDrag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DestroyDragPoint();
                InitDragPoint(Input.mousePosition);
                //if (_dragPoint == null)
                //{
                //    InitDragPoint(Input.mousePosition);
                //}
                //else
                //{
                //    if (_draggingNow as MonoBehaviour == null)
                //        DestroyDragPoint();
                //}
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
        Vector2 oldPos = Input.mousePosition;
        Vector2 newPos = Input.mousePosition;
        while (_CurrentTarget != null)
        {
            newPos = Input.mousePosition;
            Vector2 d = (newPos - oldPos);
            Vector2 delta = (newPos - oldPos).normalized;
            float distance = (newPos - oldPos).magnitude;
            if (distance > 0)
            {
                Vector2 move = delta* GameManager.Instance.data.currentInst.Data.dragData.DragMaxSpeed* Time.deltaTime * 100f;
                if (distance < move.magnitude) 
                {
                    move = d;
                }
                Vector3 realPos = GetRealPosition(oldPos + move);
                Vector3 curvePops = GetRealPosition(newPos);
                if (_constraintManager.CheckConstraint(realPos))
                {
                    oldPos += move;
                    ApplyNewPos(realPos);
                    ApplyCurvePosition(curvePops);
                }
                else
                {
                    Debug.Log("not allowed");
                }
            }
            else
            {
                Vector3 realPos = GetRealPosition(oldPos);
                ApplyNewPos(realPos);
                ApplyCurvePosition(realPos);
            }
            yield return null;
        }
    }

    private void StartDragPointMovement()
    {
        if (movingDP != null)
            StopCoroutine(movingDP);
        movingDP = StartCoroutine(MovingDragPoint());
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
    private void OnNewLevel()
    {
        _constraintManager = new DragConstraintManager();
        _constraintManager.SetSpline(GameManager.Instance.data.currentInst.levelSpline);
        _constraintManager.MaxOffset = GameManager.Instance.data.currentInst.TrackHalfWidth;
    }

    public void Init(PlayerController _controller, PlayerManager _manager)
    {
        controller = _controller;
        manager = _manager;

        GameManager.Instance.eventManager.LevelEndreached.AddListener(OnLevelEnd);
        GameManager.Instance.eventManager.LevelStarted.AddListener(OnNewLevel);
    }

    public void AllowDrag()
    {
        allowDrag = true;
    }
    public void DisallowDrag()
    {
        if (_draggingNow != null)
            BreakDrag();
        allowDrag = false;
    }

    private void AutoAimThrow()
    {
        if(CurrentAim == null)
        {
            return;
        }
    }
    public void DestroyAnimated()
    {
        AutoAimThrow();
        DestroyDragPoint();
    }

    private void InitDragPoint(Vector2 screenPosition) 
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, _grabMaxDistance, DragMask)) 
        {
            IDraggable draggable = hit.collider.GetComponentInParent<IDraggable>();

            if(draggable != null && draggable.AllowDrag() == false )
            {
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
                    _CurrentTarget = draggable.GetGameObject().transform;

                    Rigidbody target = draggable.GetRigidBody() ;
                    target.isKinematic = false;
                    DragPlane plane = SpawnDragPlane(hit.point, draggable.GetPlaneHeight());

                    SpawnCurvePoint(transform.position, _CurrentTarget.position);

                    effectManager.InitOffset(_draggingNow.AuraOffset());
                    effectManager.ShowEffectDrag(_CurrentTarget, _CurvePoint);

                    if (draggable.CreateDragAura() == true)
                        effectManager.InstAura(_CurrentTarget, _draggingNow.AuraOffset(), 
                            _draggingNow.GetSize(), _draggingNow.GetColor());

                    _draggingNow?.DragStart(plane, BreakDrag);


                    ApplyNewPos(GetRealPosition(Input.mousePosition));
                    StartDragPointMovement();
                }
            }
  
        }
    }

    private Transform SpawnCurvePoint(Vector3 start, Vector3 end)
    {
        if (_CurvePoint != null)
            Destroy(_CurvePoint.gameObject);
        Transform point = new GameObject("CurvePoint").transform;
        point.position = Vector3.Lerp(start, end, 0.5f);
        _CurvePoint = point;

        return point;
    }

    public void BreakDrag()
    {
        if(movingDP != null)
            StopCoroutine(movingDP);
        manager.OnDragBroken();
        _calculator.StopStopCalculator();
        DestroyAnimated();
    }

    private void ApplyNewPos(Vector3 newPos)
    {
        if (_CurrentTarget == null) return;
        _CurrentTarget.position = newPos;
        controller.IKmanager.Move(newPos);
    }
    private void ApplyCurvePosition(Vector3 dragPos)
    {
        if (_CurvePoint == null) return;
        Vector3 pos = Vector3.Lerp(transform.position, dragPos, 0.5f);
        pos.y = dragPos.y + 2;
        _CurvePoint.position = pos;
    }


    private Vector3 GetRealPosition(Vector2 newPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(newPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("DragPlane")))
        {

            return hit.point;
        }
        else
        {
            Debug.Log("didn't hit dragPlane");
            return Vector3.zero;
        }
            
    }


    private void DestroyDragPoint() 
    {
        if (controller == null) return;

        if (movingDP != null)
            StopCoroutine(movingDP);
        controller.IKmanager.StopHandIK();
        // new
        if (_dragPlane != null)
            Destroy(_dragPlane.gameObject);
        _draggingNow?.DragEnd();
        _draggingNow = null;

        effectManager.HideEffectDrag();
    }

    private DragPlane SpawnDragPlane(Vector3 dragPosition, float height)
    {
        _dragPlane = Instantiate(_dragPlanePrefab, transform);
        Vector3 targetPosition = dragPosition;
        targetPosition.y = transform.position.y + height;
        _dragPlane.transform.position = targetPosition;

        _dragPlane.transform.rotation *= Quaternion.Euler(Vector3.up * 180f);
        _dragPlane.Init(100, Vector3.Distance(_dragPlane.transform.position, transform.position) - 1f);

        return _dragPlane;
    }


    // Depricated
    //private void SpawnDragJoint(float height, Vector3 hitPoint)
    //{
    //    //_dragPoint = Instantiate(_dragJointPrefab);
    //    //_dragPoint.transform.SetParent(null);
        
    //    //Vector3 targetPos = hitPoint;
    //    //targetPos = Vector3.Lerp(transform.position, hitPoint, 0.2f);
    //    //targetPos.y = transform.position.y + height;
        
    //    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    //if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("DragPlane")))
    //    //{
    //    //    targetPos.y = hit.point.y;
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log("problem");
    //    //}
    //    //_dragPoint.transform.position = targetPos;
    //}

    //private IEnumerator MoveTrailCoroutine(Transform trail, Vector3 position) 
    //{
    //    yield return new WaitForFixedUpdate();
    //    yield return new WaitForFixedUpdate();
    //    trail.position = position;
    //}


    private void OnDisable()
    {
        StopAllCoroutines();
        DestroyDragPoint();
       
    }

    private void OnDrawGizmos()
    {
        if (_CurrentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_CurrentTarget.transform.position, 0.3f);
        }
    }
}
