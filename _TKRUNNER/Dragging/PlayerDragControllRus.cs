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
    private PlayerController controller;

    private Coroutine movingDP;
    private Coroutine sensitivityChange;
    private Coroutine dragPlaneRaise;

    private bool DraggingAllowed = false;

    private DummyTarget CurrentAim = null;
    private Vector2 virtualPosition = new Vector2();
    private float currentSensitivity = 0;


    private void Start()
    {
        virtualPosition.x = Screen.width / 2;
        virtualPosition.y = Screen.height / 2;
    }


    private void Update()
    {
        #region DragPointsManagement

        if (DraggingAllowed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DestroyDragPoint();
                InitDragPoint(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (_draggingNow as MonoBehaviour != null)
                {
                    controller.manager.OnDragEnd(DestroyAnimated);
                }
            }
        }
        #endregion

    }


    private IEnumerator MovingDragPoint()
    {
        Vector2 oldPos = Camera.main.WorldToScreenPoint(_CurrentTarget.position);
        Vector2 newPos = Input.mousePosition;
        while (_CurrentTarget != null)
        {
            newPos = Input.mousePosition;
            Vector2 d = (newPos - oldPos);
            Vector2 delta = (newPos - oldPos).normalized;
            float distance = (newPos - oldPos).magnitude;
            if (distance > 0)
            {
                Vector2 move = delta* currentSensitivity * Time.deltaTime * 100f;
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
                    if (curvePops == Vector3.zero) 
                        ApplyCurvePostionDefault();
                    else
                        ApplyCurvePosition(curvePops);
                }
            }
            else
            {
                Vector3 realPos = GetRealPosition(oldPos);
                ApplyNewPos(realPos);
                if (realPos == Vector3.zero) 
                    ApplyCurvePostionDefault();
                else
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

        if (GameManager.Instance._data.currentInst.Data.dragData.UseSensitivityChange == false)
            currentSensitivity = GameManager.Instance._data.currentInst.Data.dragData.DragSensitivity;
        else
        {
            float end = GameManager.Instance._data.currentInst.Data.dragData.DragSensitivity;
            float start = 0f;
            float time = GameManager.Instance._data.currentInst.Data.dragData.SensitivityChangeTime;
            if (sensitivityChange != null) StopCoroutine(sensitivityChange);
            sensitivityChange = StartCoroutine(SensitivityChange(start, end, time));
        }

    }

    private IEnumerator SensitivityChange(float start, float end, float time)
    {
        float elapsed = 0f;
        currentSensitivity = start;

        while(elapsed <= time)
        {
            currentSensitivity = Mathf.Lerp(start, end, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentSensitivity = end;
    }





    private void OnLevelEnd()
    {
        if(_draggingNow != null)
        {
            _draggingNow.DragEnd();
            DestroyDragPoint();
            effectManager.StopDragEffect();
        }
           
    }
    private void OnNewLevel()
    {
        _constraintManager = new DragConstraintManager();
        _constraintManager.SetSpline(GameManager.Instance._data.currentInst.levelSpline);
        _constraintManager.MaxOffset = GameManager.Instance._data.currentInst.TrackHalfWidth;
    }

    public void Init(PlayerController _controller, PlayerManager _manager)
    {
        controller = _controller;
        controller.manager = _manager;

        GameManager.Instance._events.LevelEndreached.AddListener(OnLevelEnd);
        GameManager.Instance._events.LevelStarted.AddListener(OnNewLevel);
    }

    public void AllowDrag()
    {
        DraggingAllowed = true;
    }
    public void DisallowDrag()
    {
       
        if (_draggingNow != null)
            BreakDrag();
        DraggingAllowed = false;
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
                controller.IKmanager.StartHandIK();
                if (draggable.IsActive())
                {
                    _draggingNow = draggable;
                    _CurrentTarget = draggable.GetGameObject().transform;
                    //Rigidbody target = draggable.GetRigidBody() ;
                    //target.isKinematic = false;
                    DragPlane plane = SpawnDragPlane(_CurrentTarget.position, draggable.GetPlaneHeight());
                    SpawnCurvePoint(transform.position, _CurrentTarget.position);
                    SpawnDragAura();
                    effectManager.InitOffset(_draggingNow.AuraOffset());
                    effectManager.ShowEffectDrag(_CurrentTarget, _CurvePoint);
                    controller.manager.OnDragStart(_CurrentTarget.position);
                    _draggingNow?.DragStart(plane, BreakDrag);
                    StartDragPointMovement();
                }
            }
  
        }
    }

    private void SpawnDragAura()
    {
        if (_draggingNow.CreateDragAura() == true)
            effectManager.InstAura(_CurrentTarget, _draggingNow.AuraOffset(),
                _draggingNow.GetSize(), _draggingNow.GetColor());
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
        controller.manager.OnDragBroken();
        _calculator.StopStopCalculator();
        DestroyAnimated();
    }

    private void ApplyNewPos(Vector3 newPos)
    {
        if (_CurrentTarget == null) return;
        _CurrentTarget.position = newPos;
        controller.IKmanager.Move(newPos);
        controller.manager.OnDragMove(newPos);

    }
    private void ApplyCurvePosition(Vector3 dragPos)
    {
        if (_CurrentTarget == null)
        {
            effectManager.StopDragEffect();
            if (_CurvePoint != null)
                Destroy(_CurvePoint.gameObject);
        }
        Vector3 pos = Vector3.Lerp(transform.position, dragPos, 0.5f);
        pos.y = dragPos.y + 2;
        _CurvePoint.position = pos;
    }
    private void ApplyCurvePostionDefault()
    {
        if(_CurrentTarget == null) { 
            effectManager.StopDragEffect(); 
            if(_CurvePoint!=null)
            Destroy(_CurvePoint.gameObject); }
        _CurvePoint.position = Vector3.Lerp(transform.position, _CurrentTarget.position, 0.5f);
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

        if (sensitivityChange != null) StopCoroutine(sensitivityChange);
        if (movingDP != null)
            StopCoroutine(movingDP);
        controller.IKmanager.StopHandIK();
        // new
        if (_dragPlane != null)
            Destroy(_dragPlane.gameObject);
        _draggingNow?.DragEnd();
        _draggingNow = null;

        effectManager.StopDragEffect();
    }

    private DragPlane SpawnDragPlane(Vector3 dragPosition, float height)
    {
        _dragPlane = Instantiate(_dragPlanePrefab, transform);

        Vector3 targetPosition = dragPosition;
        if (GameManager.Instance._data.currentInst.Data.dragData.UseDragPlaneRaising)
        {
            float start = dragPosition.y;
            float end = start + height;
            float time = GameManager.Instance._data.currentInst.Data.dragData.DragPlaneRaiseTime;
            if (dragPlaneRaise != null) StopCoroutine(dragPlaneRaise);
            dragPlaneRaise = StartCoroutine(RaisingDragPlane(start, end, time));
        }
        else
        {
            targetPosition.y = transform.position.y + height;
            _dragPlane.transform.position = targetPosition;
        }

        _dragPlane.transform.rotation *= Quaternion.Euler(Vector3.up * 180f);
        _dragPlane.Init(100, Vector3.Distance(_dragPlane.transform.position, transform.position) - 1f);



        return _dragPlane;
    }

    private IEnumerator RaisingDragPlane(float start, float end, float time)
    {
        float elapsed = 0f;
        while(_dragPlane && elapsed <= time)
        {
            Vector3 currentPos = _dragPlane.transform.position;
            currentPos.y = Mathf.Lerp(start, end, elapsed / time);
            _dragPlane.transform.position = currentPos;

            elapsed += Time.deltaTime;
            yield return null;
        }
        if (_dragPlane)
        {
            Vector3 pos = _dragPlane.transform.position;
            pos.y = end;
            _dragPlane.transform.position = pos;
        }

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
