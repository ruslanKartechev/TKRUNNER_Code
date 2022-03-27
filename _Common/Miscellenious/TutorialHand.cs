using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
//using ButcherGames.CardboardAnimals.Mono.Gameplay;
using Commongame;

public class TutorialHand : MonoBehaviour
{
    [SerializeField] private float _clickMoveTime;
    [SerializeField] private Image _handImage;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool useOffset;
    public Vector3 CurrentImagePosition
    {
        get
        {
            return _handImage.gameObject.transform.position;
        }
    }
    public Vector3 CurrentTargetPosition
    {
        get
        {
            if (noneCoursorTarget != null)
                return Camera.main.WorldToScreenPoint(noneCoursorTarget.transform.position);
            else
                return Vector3.zero;
        }
    }


    private float _distance;
    private Vector2 _targetPosition;
    private Vector2 _clickPosition;
    private Sequence _clickSeq;

    private bool followTool = false;
    private Transform noneCoursorTarget;
    private Coroutine currentMovement;
    private void Awake()
    {
        Hide();
    }
    private void Start()
    {
        _targetPosition = _handImage.transform.position;
        GameManager.Instance._events.LevelLoaded.AddListener(OnFirstLevelLoaded);
     //   GameManager.Instance.eventManager.ClickableHit.AddListener(OnButtonClick);
    }
    private void OnFirstLevelLoaded()
    {
        GameManager.Instance._events.LevelLoaded.RemoveListener(OnFirstLevelLoaded);
        currentMovement = StartCoroutine(UIPointerMovement());
    }
    private void OnToolInit()
    {
  //      SetGOTarget(tool.HandlePoint);
        SetPosition(CurrentTargetPosition);
        OnToolFollow();
    }
    private void OnButtonClick()
    {
        followTool = false;
      //  GameManager.Instance.eventManager.MouseUp.AddListener(OnMouseRelease);
    }
    private void OnToolFollow()
    {
        followTool = true;
    }
    private void OnMouseRelease()
    {
      //  GameManager.Instance.eventManager.MouseUp.RemoveListener(OnMouseRelease);
        if (noneCoursorTarget != null)
            OnToolFollow();
        else
            Debug.Log("target tool is null");
    }
    public void SetGOTarget(Transform target)
    {
        noneCoursorTarget = target;
    }
    private IEnumerator UIPointerMovement()
    {
        Show();
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                if (followTool == true && noneCoursorTarget != null)
                {
                    _targetPosition = Camera.main.WorldToScreenPoint(noneCoursorTarget.position);
                }
                else
                {
                    _targetPosition = Input.mousePosition;
                }
            }
            SetPosition((Vector2)_targetPosition);


            yield return null;
        }

    }
    private void SetPosition(Vector2 _pos)
    {
        if (_clickSeq == null && _clickPosition != _pos)
        {
            if(useOffset == true)
            {
                _pos += (Vector2)offset;
            }

            _handImage.transform.position = Vector2.Lerp(_handImage.transform.position, _pos, Time.deltaTime * 15f);
            Vector3 targetScale;
            float distanceToTarget = ((Vector2)CurrentImagePosition - _pos).magnitude;
            if (Input.GetMouseButton(0) && distanceToTarget <= 10f)
                targetScale = Vector3.one * 0.6f;
            else
                targetScale = Vector3.one;
            _handImage.transform.localScale = Vector3.Lerp(_handImage.transform.localScale, targetScale, Time.deltaTime * 5f);
        }
    }
    public void Show()
    {
        _handImage.gameObject.SetActive(true);
    }
    public void Hide()
    {
        _handImage.gameObject.SetActive(false);
    }
    private void DUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            _targetPosition = Input.mousePosition;
            //if (PainterSpray.Default.gameObject.activeSelf) 
            //    _targetPosition = Camera.main.WorldToScreenPoint(PainterSpray.Default.transform.position + new Vector3(-0.5f, 0f, 1f));
        }

        if (Input.GetMouseButtonDown(0))
            _clickPosition = Input.mousePosition;
        if (Input.GetMouseButtonUp(0) && _clickPosition == (Vector2)Input.mousePosition)
        {
            Vector2 animPosition = Input.mousePosition;
            if (_clickSeq == null)
            {
                _clickSeq = DOTween.Sequence();
                _clickSeq.Append(_handImage.transform.DOMove(animPosition, _clickMoveTime).SetEase(Ease.OutCubic));
                _clickSeq.Append(_handImage.transform.DOScale(Vector3.one * 0.5f, 0.25f).SetEase(Ease.InOutCubic));
                _clickSeq.Append(_handImage.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack));
                _clickSeq.OnComplete(() =>
                {
                    _clickSeq = null;
                    _targetPosition = animPosition;
                });
                return;
            }
        }

        if (_clickSeq == null && _clickPosition != (Vector2)Input.mousePosition)
        {
            _handImage.transform.position = Vector2.Lerp(_handImage.transform.position, _targetPosition, Time.deltaTime * 15f);
            Vector3 targetScale;
            float distanceToTarget = ((Vector2)_handImage.transform.position - _targetPosition).magnitude;
            if (Input.GetMouseButton(0) && distanceToTarget <= 10f)
                targetScale = Vector3.one * 0.6f;
            else
                targetScale = Vector3.one;
            _handImage.transform.localScale = Vector3.Lerp(_handImage.transform.localScale, targetScale, Time.deltaTime * 5f);
        }

    }

}
