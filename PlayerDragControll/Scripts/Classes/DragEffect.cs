using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Beizer;
namespace TKRunner
{
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


        private Transform _dragPoint;
        private Transform _rootPoint;
        private Transform _curvePoint;
        private Vector3 _offset;

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
            for (int i = 1; i < _pointCount; i++)
            {
                particles.Add(Instantiate(OnTheLineParticlesPF));
                particles[i].gameObject.SetActive(false);
                particles[i].transform.parent = gameObject.transform;
            }
            particles.TrimExcess();

        }
        private IEnumerator EffectHandling()
        {
            while (true)
            {
                if (_curvePoint == null)
                    Debug.Log("Null curve");
                if (_dragPoint && _rootPoint && _curvePoint)
                {
                    _animValue += Time.deltaTime * _animSpeed * _direction;
                    Vector3 targetPosition = _dragPoint.TransformPoint(_dragPoint.position);

                    //if (_particlesTrail)
                    //{
                    //    _particlesTrail.transform.position = QuadInterpolation(_rootPoint.position, _dragPoint.transform.position, targetPosition, _animValue);
                    //}

                    List<Vector3> points = new List<Vector3>();
                    points = GetPoints(_rootPoint.position, _dragPoint.position + _offset, _curvePoint.position, _pointCount);
                    //for (int i = 0; i <= _pointCount; i++)
                    //{
                    //    float t = ((float)i) / ((float)_pointCount);
                    //    Vector3 connectedPosition = _dragPoint.TransformPoint(_dragPoint.position);
                    //    points.Add(QuadInterpolation(_rootPoint.position, _dragPoint.transform.position, connectedPosition, t));
                    //}
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


        private List<Vector3> GetPoints(Vector3 start, Vector3 end, Vector3 curve, int _pointCount)
        {
            List<Vector3> points = new List<Vector3>(_pointCount);
            int i = 1;
            while (i <= _pointCount)
            {
                //Vector3 pos = Vector3.Lerp(start, end, i / _pointCount);
                Vector3 pos = Bezier.GetPointQuadratic(start, curve, end, (float)i / _pointCount);
                points.Add(pos);
                i++;
            }
            return points;
        }


        private void SetParticles(List<Vector3> positions)
        {

            for (int i = 0; i < particles.Count; i++)
            {
                if (particles[i] != null)
                {
                    particles[i].gameObject.SetActive(true);
                    particles[i].transform.position = positions[i];
                }

            }
        }

        public void Init(Transform dragPoint, Transform rootPoint, Transform curvePoint, Vector3 offset)
        {
            _dragPoint = dragPoint;
            _rootPoint = rootPoint;
            _curvePoint = curvePoint;
            _offset = offset;
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
                _curvePoint = null;
                _dragPoint = null;


            }
        }


        private Vector3 QuadInterpolation(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            Vector3 ab = Vector3.Lerp(a, b, t);
            Vector3 bc = Vector3.Lerp(b, c, t);
            return Vector3.Lerp(ab, bc, t);
        }
    }
}