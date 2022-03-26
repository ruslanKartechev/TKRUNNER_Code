using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System;
using System.Threading;
using System.Threading.Tasks;
using Commongame.Data;
using Commongame;
namespace TKRunner
{
    public class TruckManager : MonoBehaviour
    {
        [SerializeField] private SplineFollower follower;
        [SerializeField] private TruckDummyController dummies;
        [SerializeField] private TruckWheelsManager wheels;
        [Space(5)]
        [SerializeField] private TruckMovementData _settings;
        [Space(5)]
        [SerializeField] private Transform Cabin;
        [SerializeField] private Rigidbody CabinRB;
        [SerializeField] private Rigidbody rb;
        //[SerializeField] private Collider coll;


        private PlayerController player;
        private CancellationTokenSource tokenSource;
        private CancellationTokenSource tintingSource;



        private float startYAngle;
        private float RotAngle = 30f;
        private float TintZ = 10;
        private float TintX = 5f;
        // positive z = back
        // negative z = front
        private int hitsTaken = 0;
        public int HitsToBreak = 2;

        public void Init(PlayerController target)
        {
            CabinRB.isKinematic = true;
            if (dummies == null)
                dummies = GetComponent<TruckDummyController>();
            dummies.Init(follower);
            wheels.Init();
            player = target;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.useGravity = true;
            tokenSource = new CancellationTokenSource();
            SetupFollowerOffsets();
            StartMovement();
            startYAngle = transform.eulerAngles.y;
        }
        public void SetupFollowerOffsets()
        {
            SplineSample result = new SplineSample();
            follower.Project(transform.position, result);
            Vector3 dist = (transform.position - result.position);

            float Xoffset = Vector3.Dot(dist, follower.result.right);
            float Yoffset = Vector3.Dot(dist, follower.result.up);
            follower.motion.offset = new Vector2(Xoffset, Yoffset);
            follower.SetPercent(result.percent);
            follower.Project(transform.position, result);
        }

        private void StartMovement()
        {
            switch (_settings.movePattern)
            {
                case -1:
                    HandlePattern_left();
                    break;
                case 0:
                    HandlePattern_center();
                    break;
                case 1:
                    HandlePattern_right();
                    break;
            }
            tintingSource = new CancellationTokenSource();
            ShakeCabin(tintingSource.Token);
        }
        private void FullStart()
        {
            follower.motion.applyRotation = true;
            follower.motion.offset = Vector2.zero;
            follower.motion.applyPosition = true;
            follower.enabled = true;
            follower.follow = true;
            follower.followSpeed = _settings.MainSpeed;

        }
        private void FullStop()
        {
            follower.enabled = true;
            follower.follow = false;
        }

        private void Hide()
        {
            if(gameObject != null)
                gameObject.SetActive(false);
        }

        bool destroyed = false;
        public void TakeHit()
        {
            hitsTaken++;
            Debug.Log("hit taken");
            switch (hitsTaken)
            {
                case 1:
                    PushWheel();
                    PushDummy(false);
                    break;
                case 2:
                    DestroyTruck();
                    PushDummy(true);
                    wheels.PushWheels(4);                    
                    break;
                default:
                    DestroyTruck();
                    break;
            }
        }
        private void PushWheel()
        {
            wheels.PushWheels((int)UnityEngine.Random.Range(1,3));
        }
        public void PushDummy(bool all)
        {
            if (all == false)
                dummies.PushDummy((int)UnityEngine.Random.Range(1, dummies.TotalDummyCount - 1));
            else
                dummies.PushDummy(dummies.TotalDummyCount) ;
        }

        public void DestroyTruck()
        {
            if (destroyed == true)
                return;
            tokenSource?.Cancel();
            tintingSource?.Cancel();
            FullStop();
            CabinRB.isKinematic = false;
            CabinRB.transform.parent = null;
            CabinRB.constraints = RigidbodyConstraints.None;
            CabinRB.AddTorque(UnityEngine.Random.onUnitSphere * 100,ForceMode.Impulse);
            CabinRB.AddForce(transform.up * 20, ForceMode.Impulse);
            CabinRB.AddForce(UnityEngine.Random.onUnitSphere*10, ForceMode.Impulse);
            destroyed = true;
        }


        private async void HandlePattern_center()
        {
            FullStart();
            await PlayerDetection(_settings.StopDistance, tokenSource.Token);
            StopCabinShake();
            await SpeedChanging(player.currentSpeed, _settings.SlowTime, tokenSource.Token);
            dummies.DeployAll(follower.followSpeed);

            // common part
            await Task.Delay((int)(_settings.DeploymentTime * 1000));
            LeanCabin(_settings.SlowTime, true);
            follower.direction = Spline.Direction.Backward;
            await Task.Delay((int)(_settings.HideTime * 1000));
            if (tokenSource.Token.IsCancellationRequested == false)
                Hide();
        }
        private async void HandlePattern_left()
        {
            FullStart();

            await PlayerDetection(_settings.TurningDistance, tokenSource.Token);
            float turnTime = Mathf.Abs(_settings.TurningDistance - _settings.StopDistance) / follower.followSpeed;
            if (turnTime < 0.3f)
                RotateLeft(0.3f);
            else
                RotateLeft(turnTime);
           
            await OffsetChange(_settings.leftOffset, turnTime);
            follower.motion.applyRotationY = true;
            await PlayerDetection(_settings.StopDistance, tokenSource.Token);
            StopCabinShake();
            await SpeedChanging(player.currentSpeed, _settings.SlowTime, tokenSource.Token);

            dummies.DeployRight(follower.followSpeed);
            // common part
            await Task.Delay((int)(_settings.DeploymentTime * 1000));
            LeanCabin(_settings.SlowTime, true);
            follower.direction = Spline.Direction.Backward;
            await Task.Delay((int)(_settings.HideTime * 1000));
            if (tokenSource.Token.IsCancellationRequested == false)
                Hide();
        }

        private async void HandlePattern_right()
        {
            FullStart();

            await PlayerDetection(_settings.TurningDistance, tokenSource.Token);
            float turnTime = Mathf.Abs(_settings.TurningDistance - _settings.StopDistance) / follower.followSpeed;
            RotateRight(turnTime);
            await OffsetChange(_settings.rightOffset, turnTime);
            
            await PlayerDetection(_settings.StopDistance, tokenSource.Token);
            StopCabinShake();
            await SpeedChanging(player.currentSpeed, _settings.SlowTime, tokenSource.Token);
     
            dummies.DeployLeft(follower.followSpeed);

            // common part
            await Task.Delay((int)(_settings.DeploymentTime * 1000));
            LeanCabin(_settings.SlowTime, true);
            follower.direction = Spline.Direction.Backward;
            await Task.Delay((int)(_settings.HideTime * 1000));
            if (tokenSource.Token.IsCancellationRequested == false)
                Hide();
        }


        private async Task PlayerDetection(float targetDistance, CancellationToken token)
        {
            bool detect = true;
            while (detect && token.IsCancellationRequested == false)
            {
                float distance = (player.transform.position - transform.position).magnitude;
                if(distance <= targetDistance && !token.IsCancellationRequested)
                {
                    detect = false;
                }
                await Task.Yield();
            }
        }

        private async Task SpeedChanging(float targetVal, float time,CancellationToken token)
        {
            float elapsed = 0f;
            float startVal = follower.followSpeed;
            if (targetVal < startVal)
                LeanCabin(time,false);
            else
            {
                LeanCabin(time, true);
            }
            while (elapsed <= time && !token.IsCancellationRequested)
            {
                follower.followSpeed = Mathf.Lerp(startVal,targetVal,elapsed/time);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            follower.followSpeed = targetVal;
        }

        private async void RotateRight(float time)
        {
            await RotationChange(startYAngle + RotAngle, time / 2);
            await RotationChange(startYAngle, time / 2);
        }
        private async void RotateLeft(float time)
        {
            await RotationChange(startYAngle- RotAngle, time / 2);
            await RotationChange(startYAngle, time / 2);
        }
        
        private async Task RotationChange(float targetAngle, float time)
        {
            follower.motion.applyRotationY = false;
            float elapsed = 0f;
            float startAngle = transform.eulerAngles.y;
            while(elapsed <= time)
            {
                transform.eulerAngles = new Vector3(
                     transform.eulerAngles.x,
                     Mathf.Lerp(startAngle, targetAngle, elapsed / time),
                      transform.eulerAngles.z
                    ) ;

                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            

        }
        private async Task OffsetChange(float endOffset,float time)
        {
            float startOffset = follower.motion.offset.x;
            float elapsed = 0f;
 
            while(elapsed <= time)
            {

                follower.motion.offset = new Vector2(
                    Mathf.Lerp(startOffset,endOffset,elapsed/time),
                    follower.motion.offset.y
                    );
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
        }


        private async Task ShakeCabin(CancellationToken token)
        {
            float time = 0.5f;
            while(token.IsCancellationRequested == false)
            {
                await TintCabin(TintX,time);
                await TintCabin(-TintX, 2*time);
                await TintCabin(0, time);
            }
 
        }
        private void StopCabinShake()
        {
            tintingSource?.Cancel();
            TintCabin(0, 0.5f);
        }
        private async Task TintCabin(float targetTint, float time)
        {
            float start = Cabin.transform.eulerAngles.x;
            if(start > 90)
            {
                start = start-360;
            }
            float elapsed = 0f;
            while(elapsed <= time)
            {
                SetCabinTint(Mathf.Lerp(start,targetTint,elapsed/time));
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            SetCabinTint(targetTint);
        }
        private void SetCabinTint(float amount)
        {
            Cabin.transform.eulerAngles = new Vector3(amount,
    Cabin.transform.eulerAngles.y,
    Cabin.transform.eulerAngles.z);
        }
        private async void LeanCabin(float leanTime, bool backwards)
        {
            float stayTime = 0.1f;
            if (backwards)
            {
                await LeaningCabin(TintZ, leanTime);
            }
            else
            {
                await LeaningCabin(-TintZ, leanTime);
            }
          
            await Task.Delay((int)(1000 * stayTime));
            await LeaningCabin(0, leanTime/2);
        }
        private async Task LeaningCabin(float targetAngle, float time)
        {
            float elapsed = 0f;
            float start = Cabin.transform.eulerAngles.z;
            if (start > 90)
            {
                start = start - 360;
            }
            while (elapsed < time)
            {
                SetCabinLean(Mathf.Lerp(start, targetAngle, elapsed / time));
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            SetCabinLean(targetAngle);
        }

        private void SetCabinLean(float amount)
        {
          
            Cabin.transform.eulerAngles = new Vector3(Cabin.transform.eulerAngles.x,
Cabin.transform.eulerAngles.y,
amount);
        }

        private void OnDisable()
        {
            if (tokenSource != null)
                tokenSource.Cancel();
            if (tintingSource != null)
                tintingSource.Cancel();
        }
    }
}