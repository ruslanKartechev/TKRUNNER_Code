using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragEffect : MonoBehaviour
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private ParticleSystem _particlesTrail;
    [SerializeField] private ParticleSystem _particlesLocal;
    [SerializeField] private int _pointCount;
    [SerializeField] private float _animSpeed;
    [Space(5)]
    [SerializeField] private ParticleSystem StartParticlesPF;
    //[SerializeField] private ParticleSystem EndParticlesPf;
    [SerializeField] private ParticleSystem OnTheLineParticlesPF;

    private List<ParticleSystem> particles = new List<ParticleSystem>();


    private bool _disabled;
    private float _direction = 1f;
    private float _animValue;
    private Joint _dragPoint;
    private Transform _rootPoint;
    private Coroutine effectShowing;

    private void Start()
    {
        particles = new List<ParticleSystem>(_pointCount);
        PreInstParticles();
    }

    private void PreInstParticles()
    {
        particles.Add(Instantiate(StartParticlesPF));
        particles[0].gameObject.SetActive(false);
        particles[0].transform.parent = gameObject.transform;
        for (int i = 1;i < _pointCount; i++)
        {
            particles.Add( Instantiate(OnTheLineParticlesPF));
            particles[i].gameObject.SetActive(false);
            particles[i].transform.parent = gameObject.transform;
        }
        particles.TrimExcess();

    }
    private IEnumerator EffectHandling()
    {
        while (true)
        {
            if (_dragPoint && _dragPoint.connectedBody && _rootPoint)
            {
                _animValue += Time.deltaTime * _animSpeed * _direction;
                Vector3 targetPosition = _dragPoint.connectedBody.transform.TransformPoint(_dragPoint.connectedAnchor);

                if (_particlesTrail)
                {
                    _particlesTrail.transform.position = QuadInterpolation(_rootPoint.position, _dragPoint.transform.position, targetPosition, _animValue);
                }

                List<Vector3> points = new List<Vector3>();
                for (int i = 0; i <= _pointCount; i++)
                {
                    float t = ((float)i) / ((float)_pointCount);
                    Vector3 connectedPosition = _dragPoint.connectedBody.transform.TransformPoint(_dragPoint.connectedAnchor);
                    points.Add(QuadInterpolation(_rootPoint.position, _dragPoint.transform.position, connectedPosition, t));
                }
                _line.positionCount = points.Count;
                SetParticles(points);
                _line.SetPositions(points.ToArray());
            }
            else
            {
                Disable();
            }
            yield return null;
        }
    }


    private void SetParticles(List<Vector3> positions)
    {

        for (int i = 0; i< particles.Count; i++)
        {
            if ( particles[i] != null)
            {
                particles[i].gameObject.SetActive(true);
                particles[i].transform.position = positions[i];
            }

        }
    }

    public void Init(Joint dragPoint, Transform rootPoint)
    {
        _dragPoint = dragPoint;
        _rootPoint = rootPoint;
        if (effectShowing != null) StopCoroutine(effectShowing);
        effectShowing = StartCoroutine(EffectHandling());
    }

    public void Disable()
    {
        if (!_disabled)
        {
            if (effectShowing != null) StopCoroutine(effectShowing);
            _disabled = true;
            gameObject.SetActive(false);
          
        }
    }


    private Vector3 QuadInterpolation(Vector3 a, Vector3 b, Vector3 c, float t) 
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }
}
