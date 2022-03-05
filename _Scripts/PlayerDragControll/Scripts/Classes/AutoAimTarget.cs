using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoAimTarget : MonoBehaviour
{
    [SerializeField] private Transform _markerPosition;
    [SerializeField] private SpriteRenderer _markerPrefab;
    //[SerializeField] private ParticleSystem _markerPrefab;

    //private ParticleSystem _markerInstance;
    private SpriteRenderer _markerInstance;

    public bool isActive;


    public void Activate() 
    {
        if (isActive)
        {
            this.DOKill();
            _markerInstance = Instantiate(_markerPrefab);
            _markerInstance.transform.position = _markerPosition.position;
            _markerInstance.transform.localScale = Vector3.zero;
            _markerInstance.transform.DOScale(Vector3.one, 0.25f)
                .SetTarget(this)
                .SetEase(Ease.OutBack);
        }
    }

    public void Deactivate() 
    {
        if (_markerInstance != null)
        {
            this.DOKill();
            _markerInstance.transform.DOScale(Vector3.zero, 0.25f)
                .SetEase(Ease.InBack)
                .SetTarget(this)
                .OnComplete(() => Destroy(_markerInstance.gameObject))
                .OnKill(() => Destroy(_markerInstance.gameObject));
        }
    }
}
