using Dreamteck.Splines;
using Commongame;
using Commongame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
namespace TKRunner
{
    public enum DummyStates
    {
        Idle, Waiting,Run, Drag, Thrown,Standup, Truck, Dead
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DummyManager))]
    public class DummyManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DummyManager me = (DummyManager)target;
            if (GUILayout.Button("Die"))
            {
                me.Die();
            }
        }
    }
#endif





    [DefaultExecutionOrder(1000)]
    [DisallowMultipleComponent]
    public class DummyManager : RunnerManager, IWeaponTarget, ISpawnable, IPlayerHunter
    {
        [Header("Dummy Settings")]
        [SerializeField] private EnemySettings _settings;
        [SerializeField] private DummyTruckData _truckSettings;
        [Space(10)]
        [SerializeField] private SkinnedMeshRenderer _renderer;
        [SerializeField] private AudioSource _audio;
        [Header("Colliders")]
        [SerializeField] private Collider TriggerColl;
        [SerializeField] private CapsuleCollider capColl;
        [Header("Scripts")]
        public DummyRagdollManager ragdollManager;
        public DummySlicer _sclicer;
        public DummyTarget DragTarget;
        public DummyTriggerCollider _triggerColl;

        public SkinnedMeshRenderer Renderer { get { return _renderer; } }
        private bool IsDefeated = false;
        private bool IsVulnarable = true;
        private Vector3 distanceToTarget = new Vector3();
        private Transform _CurrentTarget;

        #region Async
        private Coroutine detecting;
        private Coroutine sideMoving;
        private Coroutine slowingDown;
        private Coroutine turning;

        private CancellationTokenSource disableToken;
        private CancellationTokenSource jumpToken;
        #endregion

        #region Actions
        private Action OnPlayerApproached = null;
        private Action OnAttackDistance = null;
        public Action<Transform> OnDummyTrigger = null;
        private Action OnSlowDownDistance = null;
        public Action OnPlayerContact = null;
        public Action OnGroundFall = null;
        public Action<Transform> OnTruckTrigger = null;
        #endregion

        public float CurrentPercent { get { return (float)follower.result.percent; } }
        public float CurrentSpeed { get { return follower.followSpeed; } }

        public DummyTarget _Target { get { return DragTarget; } }

        public DummyStates CurrentState = DummyStates.Idle;



        private void Awake()
        {
            if(GameManager.Instance.data.currentInst == null)
            {

                follower.follow = false;
                follower.enabled = false;
                return;
            }
            else
            {
                transform.parent = GameManager.Instance.data.currentInst.transform;
            }

        }
        private void OnEnable()
        {
            _triggerColl?.Init(this);
            DragTarget?.Init(this);
            ragdollManager?.Init(this);
            if(TriggerColl != null)
                TriggerColl.enabled = true;
            StartCoroutine(VulnerabilityCheckHandler());

        }

        #region PlayerHunter

        private float hunterSpeed = 4f;
        private float huntApproach = 1f;
        private Vector3 _attackPoint = new Vector3();
        public void InitHunter()
        {
            StopAllCoroutines();
            StopMoving();
            OnDisable();
        }

        public void SetTarget(Transform target)
        {
            _CurrentTarget = target;
        }

        public void SetSpline(SplineComputer spline)
        {
            follower.spline = spline;
        }
        
        public void SetAttackPoint(Vector3 point)
        {
            _attackPoint = point;
           
        }

        public void Attack()
        {
            HuntDown(_attackPoint);
        }


        private void HuntDown(Vector3 point)
        {
            CurrentState = DummyStates.Run;
            mAnim.Play(AnimNames.DummyRun, 0, 0);
            follower.follow = false;
            rb.isKinematic = true;
            StartCoroutine(HuntTargetDown(hunterSpeed, point));
        }
     
        private IEnumerator HuntTargetDown(float speed, Vector3 point)
        {
            bool reached = false;
            Vector3 lookPos = _CurrentTarget.position;
            Vector3 targetPos = _CurrentTarget.position + point;
            targetPos.y = 0;
            Vector3 pointingVector = targetPos - transform.position;
            lookPos.y = GameManager.Instance.data.Player.transform.position.y;
            while (pointingVector.magnitude >= 0.1f)
            {
                lookPos = _CurrentTarget.position;
                targetPos = _CurrentTarget.position + point;
                targetPos.y = 0;

                pointingVector = targetPos - transform.position;

                Vector3 move = pointingVector.normalized * speed * Time.deltaTime;
                if (reached == false && pointingVector.magnitude <= huntApproach)
                {
                    reached = true;
                    mAnim.SetBool(GetRandomAttackAnim(), true);
                }
            
                if(move.magnitude >= pointingVector.magnitude)
                {
                    move = pointingVector;
                }
                transform.position += move;
                transform.rotation = Quaternion.LookRotation(lookPos - transform.position );
                yield return null;
            }
            targetPos = _CurrentTarget.position + point;
            targetPos.y = 0;
            transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(lookPos - transform.position);


        }
        private string GetRandomAttackAnim()
        {
            float rand = UnityEngine.Random.Range(0f, 1f);
            if (rand >= 0.65)
            {
                return AnimNames.ZombieBiting;
            }
            else
                return AnimNames.DoublePound;
        }

        #endregion





        #region Spawnable
        public void Hide()
        {
            follower.enabled = false;
            gameObject.SetActive(false);
        }
        public void OnSpawn()
        {
            GameManager.Instance.dummyController.AddDummy(this);
            gameObject.SetActive(true);
            follower.spline = GameManager.Instance.data.currentInst.levelSpline;
            follower.follow = false;
            follower.enabled = true;
            follower.motion.offset = Vector2.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            mAnim.Play(AnimNames.PortalJump);
            CurrentState = DummyStates.Run;

            disableToken = new CancellationTokenSource();
        }
        public void Activate(ActivatorModes mode)
        {
            if (CurrentState == DummyStates.Dead) return;
            SetVulnerable();
            switch (mode)
            {
                case ActivatorModes.Default:
                    ActivateDefault();
                    break;
                case ActivatorModes.Forward:
                    ActivateForward();
                    break;
                case ActivatorModes.Hunter:
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    transform.parent = GameManager.Instance.data.currentInst.gameObject.transform;
                    break;
            }


        }

        public async void SpawnDefault()
        {
            mAnim.Play(AnimNames.LookAround,0,UnityEngine.Random.Range(0f,1f));
            await Task.Delay((int)(_settings.WaitAfterSpawn * 1000));
            InitActive(GameManager.Instance.data.currentInst.levelSpline);

        }
        public async void ActivateDefault()
        {
            await Task.Delay((int)(_settings.WaitAfterSpawn*1000));
            if (disableToken.IsCancellationRequested == true) return;
            rb.constraints = RigidbodyConstraints.None;
            transform.parent = GameManager.Instance.data.currentInst.gameObject.transform;
            base.InitActive(GameManager.Instance.data.currentInst.levelSpline);
            _CurrentTarget = GameManager.Instance.data.Player.transform;

            SetFollowSpeed(_settings.StartSpeed);
            SetFollowerDetection();
            if (detecting != null) StopCoroutine(detecting);
            detecting = StartCoroutine(PlayerDetector());
            StartImmidiate(false);
            mAnim.Play(AnimNames.DummyRun, 0, UnityEngine.Random.Range(0f,0.5f));
            CurrentState = DummyStates.Run;
            OnTruckTrigger = OnTruckCollision;
            OnSlowDownDistance = SlowDown;
        }
        public async void ActivateForward()
        {
            if (disableToken.IsCancellationRequested == true) return;
            rb.constraints = RigidbodyConstraints.None;
            InitWaiting();

            await Task.Delay((int)(_settings.WaitAfterSpawn * 1000));
            if (disableToken.IsCancellationRequested == true) return;
            StartFromWaiting();
            transform.parent = GameManager.Instance.data.currentInst.gameObject.transform;
        }


        public void SetSettings(object settings)
        {
            _settings = (EnemySettings)settings;
        }
        #endregion



        #region WaitingDummy

        private Coroutine lookingAtPlayer;
        private Coroutine freeMoving;
        public void InitWaiting()
        {
            GameManager.Instance.dummyController.AddDummy(this);
            CurrentState = DummyStates.Waiting;
            follower.follow = false;
            follower.enabled = false;
            transform.parent = GameManager.Instance.data.currentInst.gameObject.transform;
            _CurrentTarget = GameManager.Instance.data.Player.transform;
            SetFollowSpeed(0);
            mAnim.enabled = true;
            mAnim.Play(AnimNames.DummyWait,0,UnityEngine.Random.Range(0f,0.6f));
            if (lookingAtPlayer != null) StopCoroutine(lookingAtPlayer);

            lookingAtPlayer = StartCoroutine(LookAtPlayer());
        }
        public async void StartFromWaiting()
        {
            await Task.Delay((int)(1000*_settings.WaitAfterSpawn));
            if (CurrentState == DummyStates.Dead) return;
            _CurrentTarget = GameManager.Instance.data.Player.transform;
            CurrentState = DummyStates.Run;
            mAnim.Play(AnimNames.DummyRun);
            freeMoving = StartCoroutine(FreeMovement());
            if (detecting != null) StopCoroutine(detecting);
            detecting = StartCoroutine(PlayerDetector());
            OnAttackDistance = OnFreePlayerApproached;

        }

        private IEnumerator LookAtPlayer()
        {
            Transform player = GameManager.Instance.data.Player.transform;
            follower.motion.applyRotation = false;
            while (player != null)
            {
                transform.LookAt(player.position , Vector3.up);
                yield return null;
            }

        }
        private IEnumerator FreeMovement()
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            follower.follow = false;
            mainSpeed = _settings.StartSpeed * GameManager.Instance.data.currentInst.Data.moveData.GlobalSpeedMod;
            while (true)
            {
                if (rb == null)
                    yield break;
                rb.velocity = (distanceToTarget).normalized 
                    * mainSpeed;
                yield return null;
            }
        }
        private void OnFreePlayerApproached()
        {
            StopCoroutine(detecting);
            StopCoroutine(lookingAtPlayer);
            StopCoroutine(freeMoving);
            AttackPlayer();
        }
        #endregion


        private void SetFollowSpeed(float speed)
        {
            mainSpeed = speed * GameManager.Instance.data.currentInst.Data.moveData.GlobalSpeedMod ;
            follower.followSpeed = mainSpeed;
        }

        public override void InitActive(SplineComputer spline)
        {
            GameManager.Instance.dummyController.AddDummy(this);
            base.InitActive(spline);
            _CurrentTarget = GameManager.Instance.data.Player.transform;

            SetFollowSpeed(_settings.StartSpeed );
            SetFollowerDetection();
            if (detecting != null) StopCoroutine(detecting);
            detecting = StartCoroutine(PlayerDetector());
            StartImmidiate(true);
            CurrentState = DummyStates.Run;
            OnTruckTrigger = OnTruckCollision;
            OnSlowDownDistance = SlowDown;

        }


        private void SetInvulnerable()
        {
            IsVulnarable = false;
        }
        private void SetVulnerable()
        {
            IsVulnarable = true;
        }


        #region Trucker
        public void InitTrucker(SplineComputer spline,bool mirror)
        {
            CurrentState = DummyStates.Truck;
            //capColl.enabled = false;
            follower.follow = false;
            rb.isKinematic = true;
            follower.enabled = true;
            transform.localPosition = Vector3.zero;
            follower.spline = spline;
            mAnim.Play(AnimNames.DummyTruckIdle,0,0);
            mAnim.SetBool("MirrorTruckAnim", mirror);
            follower.motion.applyPositionX = true;
            follower.motion.applyPositionY = true;
            OnTruckTrigger = null;
        }

        public void JumpFromTruck(float speed, bool mirrored)
        {
            IsRunning = false;
            jumpToken = new CancellationTokenSource();
            transform.parent = follower.spline.gameObject.transform;
            //follower.enabled = true;
            mainSpeed = speed;
            follower.followSpeed = mainSpeed;
            SetupFollowerOffsets();
            mAnim.Play(AnimNames.DummyTruckJump);
            JumpingFromTruck(mirrored);

        }
        private async void JumpingFromTruck(bool mirrored)
        {
            await Task.Yield();
            follower.follow = true;

            float startX = follower.motion.offset.x;
            float X = follower.motion.offset.x - _truckSettings.JumpDistance ;
            if(mirrored)
                X = follower.motion.offset.x + _truckSettings.JumpDistance;
            float startY = follower.motion.offset.y;
            float Y = 0;
            float elapsed = 0f;
            float time = _truckSettings.DeployTime;

            while (elapsed <= time && jumpToken.Token.IsCancellationRequested == false)
            {
                follower.motion.offset = new Vector2(
                    Mathf.Lerp(startX, X, elapsed / time),
                    Mathf.Lerp(startY, Y, elapsed / time)
                    );

                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            SetFollowerDetection();
            if (detecting != null) StopCoroutine(detecting);
            detecting = StartCoroutine(PlayerDetector());
            if (jumpToken.Token.IsCancellationRequested == false)
            {
                CurrentState = DummyStates.Run;
                IsRunning = true;
                rb.isKinematic = false;
                capColl.enabled = true;
                capColl.isTrigger = false;
                await SpeedChangingDirect(2.5f,
                    _settings.StartSpeed * GameManager.Instance.data.currentInst.Data.moveData.GlobalSpeedMod,
                     jumpToken.Token);
                if(jumpToken.Token.IsCancellationRequested == false)
                    SetFollowSpeed(_settings.StartSpeed);
            }

        }

        #endregion
        public void SetupFollowerOffsets()
        {
            follower.follow = false;
            follower.enabled = true;
            SplineSample result = new SplineSample();
            follower.Project(transform.position, result);
            Vector3 dist = (transform.position - result.position);

            float Xoffset = Vector3.Dot(dist, follower.result.right);
            float YOffset = Vector3.Dot(dist, follower.result.up);
            follower.motion.offset = new Vector2(Xoffset, YOffset);
            follower.SetPercent(result.percent);
            follower.Project(transform.position, result);
        }



        public override void StartImmidiate(bool animate)
        {
            StartSplineMovement(animate);

        }
        public override void StopMoving()
        {
            base.StopMoving();
            if (detecting != null) StopCoroutine(detecting);
        }
        private IEnumerator PlayerDetector()
        {
            bool detect = true;
            bool approachedCalled = false;
            while (detect)
            {
                SplineSample result = new SplineSample();
                follower.Project(GameManager.Instance.data.Player.transform.position, result);
                double distance = (result.percent - follower.result.percent) 
                    * GameManager.Instance.data.currentInst.TrackUnitLength*100;
                distanceToTarget = _CurrentTarget.position - transform.position;
             
                float dirDist = (distanceToTarget).magnitude;

                if(distance <= _settings.SlowDownDistance)
                {
                    OnSlowDownDistance?.Invoke();
                }
                if (distance <= _settings.ApproachDistance)
                {
                    if (!approachedCalled) { OnPlayerApproached?.Invoke() ;  approachedCalled = true; }
                    if ( dirDist <= _settings.JumpAttackDistance)
                    {
                        OnAttackDistance?.Invoke();
                    }

                }
                //else if (distance >= _settings.TPdistance)
                //{
                //    OnDummyBehind?.Invoke();
                //}
                yield return null;
            }
        }

        private IEnumerator VulnerabilityCheckHandler()
        {
            while (true)
            {
                if (Renderer.isVisible == false)
                {
                    SetInvulnerable();
                }
                else
                    SetVulnerable();
                yield return null;
            }
        }

        #region ActionsForPlayerDetectionWhenFollower
        private void SetFollowerDetection()
        {
            OnPlayerApproached = OnFollowerPlayerApproached;
            OnDummyTrigger = OnDummyCollision;
            OnAttackDistance = OnFollowerAttackDistance;
            OnSlowDownDistance = SlowDown;
        }
        private void OnFollowerPlayerApproached()
        {
            OnPlayerApproached = null;
            if (sideMoving != null)
                StopCoroutine(sideMoving);
            sideMoving = StartCoroutine(SideMoving());
        
        }
        private void OnFollowerAttackDistance()
        {
            if(detecting!=null) StopCoroutine(detecting);
            AttackPlayer();
        }
        private void OnFollowerFarBehind()
        {
            SplineSample res = new SplineSample();
            follower.Project(Camera.main.transform.position, res);
            follower.SetPercent(res.percent);
        }


        private void SlowDown()
        {
            if (sideMoving != null) StopCoroutine(sideMoving);
            TurnForward();
            if (slowingDown != null) StopCoroutine(slowingDown);
            slowingDown = StartCoroutine(SlowingDown());
            OnSlowDownDistance = null;
        }
        private IEnumerator SlowingDown()
        {
            float startSpeed = follower.followSpeed;
            follower.followSpeed = GameManager.Instance.data.Player.currentSpeed;
            yield return new WaitForSeconds(_settings.SlowDownTime);
            follower.followSpeed = startSpeed;
            OnPlayerApproached = OnFollowerPlayerApproached;

        }
        #endregion


        private IEnumerator SideMoving()
        {
            if (turning != null) StopCoroutine(turning);

            float turnTime = 0.5f;
            float elapsed = 0f;  
            
            float offsetMax = 5f;
            Quaternion startRot = transform.rotation;
            follower.motion.applyRotation = false;
            while (true)
            {
                SplineSample result = new SplineSample();
                follower.Project(GameManager.Instance.data.Player.transform.position, result);

                Vector3 dist = GameManager.Instance.data.Player.transform.position - transform.position;

                float projX = Vector3.Dot(dist, follower.result.right);
                float projZ = Vector3.Dot(dist, follower.result.forward);
                float v = (follower.followSpeed - GameManager.Instance.data.Player.currentSpeed);
                if(v < 0)
                {
                    follower.followSpeed += Mathf.Abs(v);
                }
                float SideSpeed = Mathf.Abs(projX) * v / projZ;
                if (projX < 0)
                    SideSpeed *= -1;

                if (Mathf.Abs(follower.motion.offset.x) < offsetMax)
                {
                    follower.motion.offset += new Vector2(SideSpeed, 0) * Time.deltaTime;
                }
                if(elapsed <= turnTime)
                {
                    transform.rotation = Quaternion.Lerp(startRot, 
                        Quaternion.LookRotation(GameManager.Instance.data.Player.transform.position - transform.position,Vector3.up), elapsed/turnTime);
                    elapsed += Time.deltaTime;
                }
                else
                {
                    transform.LookAt(GameManager.Instance.data.Player.transform);
                }
                yield return null;
            }
        }

        private void TurnForward()
        {
            float turnTime = 0.5f;
            if (turning != null) StopCoroutine(turning);
            turning = StartCoroutine(Turning(turnTime, follower.result.forward, ()=> { ApplyFollowerRotation(true); }));
        }
        private IEnumerator Turning(float time, Vector3 dir, Action onEnd)
        {
            float elapsed = 0f;
            Vector3 start = transform.forward;
            while (elapsed <= time)
            {
                Vector3 d = Vector3.Lerp(start, dir, elapsed / time);
                transform.rotation = Quaternion.LookRotation(d);
                elapsed += Time.deltaTime;
                yield return null;
            }
            onEnd?.Invoke();
        }
        private void ApplyFollowerRotation(bool apply)
        {
            follower.motion.applyRotation = apply;
        }



        #region PlayerAttaack

        private void AttackPlayer()
        {
            if (jumpToken != null) jumpToken.Cancel();
            jumpToken = new CancellationTokenSource();
            
            mAnim.Play(AnimNames.DummyJump);
            Vector3 pushVector = (GameManager.Instance.data.Player.transform.position 
                - transform.position 
                + GameManager.Instance.data.Player.transform.forward).normalized
                + transform.up * _settings.UpwardPushModifier;

            OnPlayerContact = OnPlayerAttackContact;
            if (sideMoving != null)
                StopCoroutine(sideMoving);
            PushDummy(pushVector * mainSpeed * _settings.AttackPushModifier);
          
            OnGroundFall = OnAttackedFall;

        }

        private void OnAttackedFall()
        {
            mAnim.StopPlayback();
            SetDead();
            ragdollManager.SetPureRagdoll();
            OnGroundFall = null;
        }
        private void OnPlayerAttackContact()
        {
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit,3, GameManager.Instance.data.PlayerMask))
            {
                IDamagable target = hit.collider.gameObject.GetComponent<IDamagable>();
                if(target != null)
                {
                    target.TakeHit();
                }
            }

            OnPlayerContact = null;
        }


        #endregion



        public void Die()
        {
            if (CurrentState != DummyStates.Dead)
            {                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.None;
                SetDead();
                if (detecting != null)
                    StopCoroutine(detecting);
                SetRagdoll();
                DragTarget.SetNonDraggable();

            }
        }
        private void SetDead()
        {
            if (rb != null)
                Destroy(rb);
            if (capColl != null)
                Destroy(capColl);
            if (TriggerColl != null)
                Destroy(TriggerColl);
            //if (slowingDown != null) StopCoroutine(slowingDown);
            //if (freeMoving != null) StopCoroutine(freeMoving);
            StopAllCoroutines();
            if (disableToken != null) disableToken.Cancel();
            CurrentState = DummyStates.Dead;
            IsRunning = false;
            mAnim.StopPlayback();
            mAnim.enabled = false;
            StopMoving();
        }



        // ragdoll management
        private void SetRagdoll(bool doGroundCheck = true)
        {
            if (ragdollManager == null) { Debug.Log("<color=red>ragdoll manager is null</color>");return; }
            ragdollManager.SetActive();
            if(doGroundCheck)
                ragdollManager.StartGroundCheck(GameManager.Instance.data.currentInst.Data.effects.freeFallTime);
            if(mAnim!=null)
                mAnim.enabled = false;
        }

        // end ragdoll
        public void PushFromTruck(Vector3 origin, float force)
        {
            CurrentState = DummyStates.Run;
            PushAway(origin, force);
        }


        #region Collisions/Triggers
        public void ExchangeSpeed(float speed)
        {
            follower.followSpeed = speed;
        }
        public void OnDummyCollision(Transform dummy)
        {
            if (CurrentState == DummyStates.Dead)
                return;
            DummyManager dum = dummy.gameObject.GetComponent<DummyManager>();
            if (dum.CurrentState == DummyStates.Run && CurrentState == DummyStates.Run)
            {

                if(CurrentPercent >= dum.CurrentPercent) // if I am ahed
                {
                    if(dum.CurrentSpeed > CurrentSpeed) // if He is faster
                    {
                        float mySpeed = follower.followSpeed;
                        follower.followSpeed = dum.CurrentSpeed;
                        dum.ExchangeSpeed(mySpeed);
                    }
                }
            }
        }
        public void PushDummy(Vector3 force)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            CurrentState = DummyStates.Thrown;
            StopMoving();
            rb.velocity = force;


        }
        public void OnBulletEnter(IBullet bullet)
        {
            if (bullet == null || IsVulnarable == false) return;
            KillAndPush((bullet.GetVelocity() + Vector3.up)*100);
        }

        #endregion

        #region WeaponTarget


        public bool Slash(Plane plane)
        {
            //if (IsVulnarable == false) return false;
            GameManager.Instance.sounds.PlaySingleTime(Sounds.AxeHit);
            SetDead();
            StartCoroutine(Slicing(plane));
            IsVulnarable = false;
            return true;
        }

        private IEnumerator Slicing(Plane plane)
        {
            gameObject.transform.parent = null;
            gameObject.tag = "Untagged";
            ragdollManager.PrepareSlice();
            Destroy(capColl);
            Destroy(TriggerColl);
            yield return null;
            Plane p = new Plane(transform.position+transform.up*UnityEngine.Random.Range(0.5f,1.2f), transform.position);
            Destroy(this);
            _sclicer.Slice(p, 1, null);
 
        }
        public bool PushAway(Vector3 origin, float force)
        {

            if (IsVulnarable == false) return false;
            if (CurrentState != DummyStates.Dead && CurrentState != DummyStates.Drag)
            {
                Die();
                Vector3 dir = (transform.position - origin);
                dir.y = Mathf.Abs(dir.y);
                dir.Normalize();
                rb.velocity = dir * force;
                ragdollManager.PushDoll( dir * force, true);
            }
            return true;
        }
        public bool KillAndPush(Vector3 force)
        {
            if (IsVulnarable == false) return false;
            if (CurrentState == DummyStates.Run || CurrentState == DummyStates.Waiting)
            {
                force.y = Mathf.Abs(force.y);
                StopAllCoroutines();
                Die();
                rb.velocity = force;
                ragdollManager.PushDoll(force, true);
                GameManager.Instance.sounds.PlaySingleTime(Sounds.BatHit);
               
            }
            return true;
        }

        public Transform GetTransform()
        {
            return transform;
        }
        #endregion



        public void Defeated()
        {
            if(capColl!=null)
                capColl.enabled = false;
            StopAllCoroutines();
            if (CurrentState != DummyStates.Dead)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                StopMoving();
                PlayDefeatAnim();
            }
        }
        public void Winner()
        {
            StopAllCoroutines();
            if (CurrentState != DummyStates.Dead)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                if (disableToken != null)
                    disableToken.Cancel();
                StopMoving();
                PlayWinAnim();
            }
        }

        // Small helpers
        public void DisableDummy()
        {
            gameObject.SetActive(false);
        }



        private void OnTruckCollision(Transform hit)
        {
            Die();
            ragdollManager.PushDoll( (hit.position - transform.position + transform.forward) * 10,true);
        }

        private void OnDestroy()
        {
            OnDisable();
        }
        private void OnDisable()
        {
            if(TriggerColl != null)
                TriggerColl.enabled = false;
            if (disableToken != null)
                disableToken.Cancel();
            if (jumpToken != null)
                jumpToken.Cancel();
            if (detecting != null) StopCoroutine(detecting);
            if (sideMoving != null) StopCoroutine(sideMoving);

        }


    }

}
