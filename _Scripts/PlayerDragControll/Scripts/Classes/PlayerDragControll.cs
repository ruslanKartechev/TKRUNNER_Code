using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerDragControll : MonoBehaviour
{
    #region Singleton
    private static PlayerDragControll _default;
    public static PlayerDragControll Default => _default;
    #endregion

    [Header("Options")]
    [SerializeField] private float _autoAimDistance;
    [SerializeField] private float _autoAimMaxVelocity;
    [SerializeField] private float _autoAimThrowForce;
    [SerializeField] private float _upperDragPlaneSize;
    [Header("Sub Components")]
    [SerializeField] private DragPlane _dragPlanePrefab;
    [SerializeField] private Joint _dragJointPrefab;
    [Header("Visual Effects")]
    [SerializeField] private DragEffect _dragEffectPrefab;
    [SerializeField] private Transform _dragEffectRoot;
    [SerializeField] private ParticleSystem _nonActiveEffect;
    [SerializeField] private ParticleSystem _nonActiveTrailEffect;



    private Joint _dragPoint;
    private DragEffect _dragEffect;
    private DragPlane _dragPlane;
    private IDraggable _draggingNow;
    private AutoAimTarget _curentAimTarget;
    private List<AutoAimTarget> _aimTargets;

    public Transform DragPoint => _dragPoint ? _dragPoint.transform : null;


    private void Awake()
    {
        _default = this;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) 
        {
            if (_dragPoint == null)
            {
                CreateDragPoint(Input.mousePosition);
            }
            else
            {
                if (_draggingNow as MonoBehaviour != null)
                    MoveDragPoint(Input.mousePosition);
                else
                    DestroyDragPoint();
            }
        }
        if (Input.GetMouseButtonDown(0)) 
        {
            if (_dragPoint == null)
                CreateDisabledEffect(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && _dragPoint != null) 
        {
            DestroyDragPoint();
        }
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


    public void ChangeDragingNow(IDraggable draggable) 
    {
        _draggingNow = draggable;
        _dragEffect.Disable();
        _dragEffect = Instantiate(_dragEffectPrefab);
        _dragEffect.transform.SetParent(null);
        _dragEffect.Init(_dragPoint, _dragEffectRoot);
    }


    private void CreateDragPoint(Vector2 screenPosition) 
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Draggable", "DraggableNonContact"))) 
        {
            IDraggable draggable = hit.collider.GetComponentInParent<IDraggable>();
            if (draggable != null) 
            {
                if (draggable.IsActive())
                {
                    _draggingNow = draggable;
                    Rigidbody target = hit.collider.attachedRigidbody;

                    _dragPoint = Instantiate(_dragJointPrefab);
                    _dragPoint.transform.SetParent(null);
                    _dragPoint.transform.position = hit.point;
                    _dragPoint.connectedBody = target;
                    _dragEffect = Instantiate(_dragEffectPrefab);
                    _dragEffect.transform.SetParent(null);
                    _dragEffect.Init(_dragPoint, _dragEffectRoot);

                    draggable.GetGameObject().transform.position = draggable.GetGameObject().transform.position;

                    _aimTargets = new List<AutoAimTarget>(FindObjectsOfType<AutoAimTarget>()).FindAll((t) => t.isActive);
                    ComputeAimTarget();
                    _draggingNow?.DragStart(SpawnDragPlane(_dragPoint.transform.position), _dragPoint, Break);
                }
            }
        }
    }
    public void Break()
    {

    }

    private void CreateDisabledEffect(Vector2 screenPosition) 
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Draggable", "DraggableNonContact")))
        {
            IDraggable draggable = hit.collider.GetComponentInParent<IDraggable>();
            if (draggable != null)
            {
                if (!draggable.IsActive())
                {
                    ParticleSystem parts = Instantiate(_nonActiveEffect);
                    parts.transform.position = hit.point;
                    Destroy(parts.gameObject, parts.main.duration);

                    ParticleSystem trail = Instantiate(_nonActiveTrailEffect);
                    trail.Play();
                    trail.transform.position = _dragEffectRoot.position;
                    Destroy(trail.gameObject, trail.main.duration);
                    StartCoroutine(MoveTrailCoroutine(trail.transform, hit.point));
                }
            }
        }
    }

    private void MoveDragPoint(Vector2 screenPosition) 
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("DragPlane"))) 
        {
            _dragPoint.transform.position = hit.point;
        }
        ComputeAimTarget();
    }

    private void DestroyDragPoint() 
    {
        if (_dragPoint)
        {
            Destroy(_dragPoint.gameObject);
            Destroy(_dragPlane.gameObject);
            _draggingNow?.DragEnd();
            _dragEffect?.Disable();
            _aimTargets = new List<AutoAimTarget>();

            if (_curentAimTarget != null && _draggingNow as MonoBehaviour != null //&& _draggingNow.UseAutoAim()
                )
            {
                _curentAimTarget.Deactivate();
                List<Rigidbody> childRb = new List<Rigidbody>(_draggingNow.GetGameObject().GetComponentsInChildren<Rigidbody>());
                Rigidbody throwRb = childRb.Count == 1 ? childRb[0] : childRb.Find((rb) => rb.tag == "MainRB");
                if (throwRb)
                {
                    Vector3 pathToTarget = _curentAimTarget.transform.position - throwRb.transform.position;
                    Vector3 targetVelocity = (pathToTarget).normalized * _autoAimThrowForce;
                    float duration = pathToTarget.magnitude / targetVelocity.magnitude;
                    DOTween.To(() => 0f, (v) =>
                    {
                        Vector3 velocity = Vector3.LerpUnclamped(Vector3.zero, targetVelocity, v);
                        if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y) && !float.IsNaN(velocity.z))
                            throwRb.velocity = velocity;
                    }, 1f, duration + 0.5f)
                        .SetEase(Ease.Linear)
                        .SetUpdate(UpdateType.Fixed);
                }
                _curentAimTarget = null;
            }
        }
    }


    private void ComputeAimTarget() 
    {
        if (_dragPoint != null && _dragPoint.connectedBody)
        {
            if (_aimTargets != null && _draggingNow as MonoBehaviour != null //&& _draggingNow.UseAutoAim()
                )
            {
                List<AutoAimTarget> activeTargets = new List<AutoAimTarget>(_aimTargets.FindAll((t) => t.isActive));
                activeTargets = activeTargets.FindAll((t) => Vector3.Distance(t.transform.position, _dragPoint.transform.position) <= _autoAimDistance);
                if (activeTargets.Count > 0 && _dragPoint.connectedBody.velocity.magnitude < _autoAimMaxVelocity)
                {
                    AutoAimTarget target = activeTargets[0];
                    foreach (AutoAimTarget t in activeTargets)
                    {
                        float curentDistance = Vector3.Distance(target.transform.position, _dragPoint.transform.position);
                        float newDistance = Vector3.Distance(t.transform.position, _dragPoint.transform.position);
                        if (newDistance < curentDistance)
                            target = t;
                    }

                    if (target != _curentAimTarget)
                    {
                        _curentAimTarget?.Deactivate();
                        _curentAimTarget = target;
                        _curentAimTarget.Activate();
                    }
                }
                else
                {
                    _curentAimTarget?.Deactivate();
                    _curentAimTarget = null;
                }
            }
        }
    }

    private DragPlane SpawnDragPlane(Vector3 dragPosition) 
    {
        _dragPlane = Instantiate(_dragPlanePrefab);
        Vector3 targetPosition = dragPosition;
        targetPosition.y = transform.position.y;
        _dragPlane.transform.SetParent(transform);
        _dragPlane.transform.position = targetPosition;
        _dragPlane.transform.LookAt(transform);
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
}
