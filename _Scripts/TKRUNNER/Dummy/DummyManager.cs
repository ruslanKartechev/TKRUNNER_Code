using Dreamteck.Splines;
using General;
using General.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
namespace TKRunner
{
    public enum DummyStates
    {
        Idle, Run, Drag, Thrown,Standup, Truck, Dead
    }


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


    [DisallowMultipleComponent]
    public class DummyManager : RunnerManager, IWeaponTarget
    {
        [Header("Dummy Settings")]
        [SerializeField] private EnemySettings _settings;
        [SerializeField] private DummyTruckData _truckSettings;
        [Space(10)]
        [SerializeField] private SkinnedMeshRenderer _renderer;
        [SerializeField] private AudioSource audio;
        [Header("Colliders")]
        [SerializeField] private Collider TriggerColl;
        [SerializeField] private CapsuleCollider capColl;
        [Header("Scripts")]
        public DummyRagdollManager ragdollManager;
        public DummySlicer _sclicer;
        public DummyTarget DragTarget;
        
        public SkinnedMeshRenderer Renderer { get { return _renderer; } }

        private Coroutine Flying;
        private Coroutine Detecting;
        private Coroutine SideMoment;
        private CancellationTokenSource disableToken;
        private CancellationTokenSource jumpToken;
        private bool IsGrounded = true;
        private bool IsDefeated = false;

        private Action OnGroundTouch = null;
        private Action OnFlyCollision = null;

        public float CurrentPercent { get { return (float)follower.result.percent; } }

        public DummyTarget _Target { get { return DragTarget; } }

        public DummyStates CurrentState = DummyStates.Idle;


        private void Start()
        {
            DragTarget?.Init(this);
            ragdollManager?.Init(this);
        }


        private void SetFollowSpeed(float speed)
        {
            mainSpeed = speed * GameManager.Instance.data.currentInst.Data.moveData.GlobalSpeedMod ;
            follower.followSpeed = mainSpeed;
        }


        public override void InitActive(SplineComputer spline)
        {
            base.InitActive(spline);
            if (follower.spline == null)
                follower.spline = spline;
            SetFollowSpeed(_settings.StartSpeed );
            StartImmidiate();
            CurrentState = DummyStates.Run;
        }
        public void InitTrucker(SplineComputer spline,bool mirror)
        {
            CurrentState = DummyStates.Truck;

            follower.follow = false;
            rb.isKinematic = true;
            follower.enabled = true;
            transform.localPosition = Vector3.zero;
            follower.spline = spline;
            mAnim.Play(AnimNames.DummyTruckIdle,0,0);
            mAnim.SetBool("MirrorTruckAnim", mirror);
            follower.motion.applyPositionX = true;
            follower.motion.applyPositionY = true;
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

            if (Detecting != null) StopCoroutine(Detecting);
            Detecting = StartCoroutine(PlayerDetector());
            if (jumpToken.Token.IsCancellationRequested == false)
            {
               // CurrentState = DummyStates.Run;
                IsRunning = true;
                rb.isKinematic = false;
                await SpeedChangingDirect(2.5f,
                    _settings.StartSpeed * GameManager.Instance.data.currentInst.Data.moveData.GlobalSpeedMod,
                     jumpToken.Token);
                if(jumpToken.Token.IsCancellationRequested == false)
                    SetFollowSpeed(_settings.StartSpeed);
            }

        }
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



        public override void StartImmidiate()
        {
            StartSplineMovement();
            if (Detecting != null) StopCoroutine(Detecting);
            Detecting = StartCoroutine(PlayerDetector());
        }
        public override void StopMoving()
        {
            base.StopMoving();
            if (Detecting != null) StopCoroutine(Detecting);
        }
        private IEnumerator PlayerDetector()
        {
            bool detect = true;
            float attackDistance = GameManager.Instance.data.currentInst.Data.moveData.DummyJumpAttackDistance;
            bool startedSideMoving = false;
            while (detect)
            {
                SplineSample result = new SplineSample();
                follower.Project(GameManager.Instance.data.Player.transform.position, result);
                double distance = (result.percent - follower.result.percent) 
                    * GameManager.Instance.data.currentInst.TrackUnitLength*100;
                float dirDist = (GameManager.Instance.data.Player.transform.position - transform.position).magnitude;
                if(distance <= 0)
                {
                    StopMoving();
                    yield return new WaitForSeconds(1f);
                    //StartMoving();
                    detect = false;
                }
                if (distance <= _settings.ApproachDistance)
                {

                    if (startedSideMoving == false)
                    {
                        if (SideMoment != null)
                            StopCoroutine(SideMoment);
                        SideMoment = StartCoroutine(SideMoving());
                        startedSideMoving = true;
                    }

                    if ( dirDist <= attackDistance)
                    {
                        detect = false;
                        AttackPlayer();
                    }
                } else if (distance >= _settings.TPdistance)
                {
                    SplineSample res = new SplineSample();
                    follower.Project(Camera.main.transform.position,res);
                    follower.SetPercent(res.percent);
                    SetFollowSpeed(_settings.StartSpeed);
                }
                
                yield return null;
            }
        }



        private IEnumerator SideMoving()
        {
            float offsetMax = 5f;

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
                transform.LookAt(GameManager.Instance.data.Player.transform);
                yield return null;
            }
        }

        public void OnDragStart()
        {
            gameObject.transform.parent = null;
            Destroy(capColl);
            Die();
            ragdollManager.SetRagdollState(RagdollStates.Drag);

        }


        public void OnDragStop()
        {
            //ragdollManager.SetRagdollState(RagdollStates.Dead);
            ragdollManager.HideCollider();
            GameManager.Instance.sounds.PlaySingleTime(Sounds.Scream, audio);
            CurrentState = DummyStates.Dead;
        }
        #region Dragging



        //public void FlyToTarget(Transform target, float speed)
        //{
        //    //ragdollManager.FlyToTarget(target,speed);
        //    //if (Flying != null)
        //    //    StopCoroutine(Flying);
        //    //Flying = StartCoroutine(FlyingToTarget(target, speed));
        //    //OnFlyCollision = OnFlyingCollision;
        //}
        //public void StopFlying()
        //{
        //    if (Flying != null)
        //        StopCoroutine(Flying);
        //    if(rb!= null)
        //    {
        //        RB.useGravity = true;
        //        RB.velocity *= 0.5f;
        //    }

        //    if (disableToken != null)
        //        disableToken.Cancel();
        //    disableToken = new CancellationTokenSource();
        //    OnGroundTouch = OnGroundFall;
        //}
        //private IEnumerator FlyingToTarget(Transform target, float speed)
        //{
        //    CurrentState = DummyStates.Thrown;
        //    int ErrorRad = GameManager.Instance.data.currentInst.Data.autoAimData.AutoAimError;
        //    Vector2 startPos = Camera.main.WorldToScreenPoint(target.position);
        //    Vector3 startDir = (target.position - transform.position).normalized;
        //    float elapsed = 0f;
        //    float CheckTime = 5f;
        //    RB.useGravity = false;
        //    RB.velocity = startDir.normalized * speed;
        //    yield return null;
        //    while (elapsed <= CheckTime)
        //    {
        //        Vector2 currentTargetPos = Camera.main.WorldToScreenPoint(target.position);

        //        if ((currentTargetPos - startPos).magnitude <= ErrorRad)
        //        {
        //            RB.velocity = (target.position - transform.position).normalized * speed;
        //        }
        //        elapsed += Time.deltaTime;
        //        yield return null;
        //    }
        //    {
        //        gameObject.SetActive(false);
        //    }
        //}


        #endregion


        private async void CheckFallover()
        {
            if(this != null)
            {
                return;
            }
            if (Detecting != null) StopCoroutine(Detecting);
            if (disableToken != null)
                disableToken.Cancel();
            disableToken = new CancellationTokenSource();
            float elapsed = 0f;
            bool doCount = true;
            float time = GameManager.Instance.data.currentInst.Data.effects.freeFallTime;
            while (elapsed <= time
                && doCount == true && disableToken.Token.IsCancellationRequested == false)
            {
                if (IsGrounded == true)
                {
                    if (rb.velocity.magnitude < GameManager.Instance.data.currentInst.Data.effects.fallingMaxSpeed)
                    {
                        //IsThrown = false;
                        CurrentState = DummyStates.Standup;
                        StandUp(disableToken.Token);
                    }
                    else
                    {
                        SetRagdoll();
                    }
                    doCount = false;
                }

                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (gameObject != null && doCount == true && disableToken.Token.IsCancellationRequested == false)
            {
                gameObject.SetActive(false);
            }
        }


        #region PlayerAttaack

        private async void AttackPlayer()
        {
            if (jumpToken != null) jumpToken.Cancel();
            jumpToken = new CancellationTokenSource();
            mAnim.Play(AnimNames.DummyJump);
            SetFollowSpeed(GameManager.Instance.data.Player.currentSpeed - 1f);
            float time = 0.5f;
            await ElevateDummy(time);
            if(this != null)
                PushDummy(transform.forward * _settings.AttackPushForce);
            if (jumpToken.IsCancellationRequested == false)
            {
                if (SideMoment != null)
                    StopCoroutine(SideMoment);
                HitPlayer();
                await Task.Yield();
                await Task.Yield();
                Die();

               
            }
        }

        private async Task ElevateDummy(float time)
        {
            float elevation = UnityEngine.Random.Range(0.7f, 2f);
            follower.motion.applyPositionY = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            float startY = transform.position.y;
            float endY = startY + elevation;           
            float elapsed = 0;
            while(elapsed <= time && jumpToken.IsCancellationRequested == false)
            {
                transform.position = new Vector3(transform.position.x,
                    Mathf.Lerp(startY, endY, elapsed / time)
                    , transform.position.z) ;
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
        }

        private void HitPlayer()
        {
            if (Physics.Raycast(transform.position,transform.forward,out RaycastHit hit, 5f, LayerMask.GetMask("Default")))
            {
                if(hit.collider.gameObject.tag == Tags.Player)
                {
                    hit.collider.gameObject.GetComponent<PlayerController>()?.TakeHit();
                }
            }
        }
        // end player attacking

        #endregion



        public void Die()
        {
            if (CurrentState != DummyStates.Dead)
            {
                //gameObject.transform.parent = null;
                if (Detecting != null)
                    StopCoroutine(Detecting);
                SetDead();
                SetRagdoll();
               // ragdollManager.SetActive();
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
            CurrentState = DummyStates.Dead;
            IsRunning = false;
            mAnim.StopPlayback();
            mAnim.enabled = false;
            StopMoving();
        }



        // ragdoll management
        private void SetRagdoll(bool doGroundCheck = true)
        {
            if (ragdollManager == null) { Debug.Log("doll manager is null");return; }

            mAnim.StopPlayback();
            ragdollManager.SetActive();
            if(doGroundCheck)
                ragdollManager.StartGroundCheck(GameManager.Instance.data.currentInst.Data.effects.freeFallTime);
            if(mAnim!=null)
                mAnim.enabled = false;
        }

        // end ragdoll



        #region StandUp
        public async void StandUp(CancellationToken token)
        {
            EffectsData data = GameManager.Instance.data.currentInst.Data.effects;
            capColl.direction = 1;
            List<Task> tasks = new List<Task>(2);
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            capColl.direction = 1;
            tasks.Add(RotateModel(data.layPosTime, token));
            tasks.Add(LayingTransition(data.layPosTime, token));
            await Task.WhenAll(tasks);
            await StandUpAnim(data.standUpTime, token);
            if (!IsDefeated && !token.IsCancellationRequested)
            {
                //CurrentState = DummyStates.Run;
                rb.constraints = RigidbodyConstraints.None;
                StartSplineMovement();
                if (Detecting != null) StopCoroutine(Detecting);
                StartCoroutine(PlayerDetector());
                
            }
            else if(IsDefeated && !token.IsCancellationRequested)
            {
                PlayDefeatAnim();
            }
        }
        private async Task RotateModel(float time,CancellationToken token)
        {
            rb.constraints = RigidbodyConstraints.None;
            float timeElpased = 0f;
            Quaternion start = transform.rotation;
            Quaternion end = Quaternion.LookRotation(follower.result.forward, follower.result.up);
            float startSpeed = rb.velocity.magnitude;
            Vector3 velocity = rb.velocity;
            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(transform.position.x, follower.result.position.y + 0.65f, transform.position.z);
            while (timeElpased <= time && token.IsCancellationRequested == false)
            {
                rb.velocity = velocity * Mathf.Lerp(startSpeed, 0, timeElpased / time);
                transform.rotation = Quaternion.Lerp(start, end, timeElpased / time);
                transform.position = Vector3.Lerp(startPos, endPos, timeElpased / time);
                timeElpased += Time.deltaTime;
                await Task.Yield();
            }
            if(token.IsCancellationRequested == false)
            {
                transform.rotation = end;
                transform.position = endPos;
            }
        }
        private async Task LayingTransition(float time, CancellationToken token)
        {
            float elapsed = 0f;
            mAnim.SetFloat("Blend", 0);
            if (mAnim != null)
                mAnim.Play(AnimNames.FallLayTree, 0, 0);
            float val_1 = 0f; // blend start val
            float val_2 = 1f; // blend end val
            while (elapsed <= time && token.IsCancellationRequested == false)
            {
                mAnim.SetFloat("Blend", Mathf.Lerp(val_1, val_2, elapsed / time) );
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (token.IsCancellationRequested == false)
                mAnim.SetFloat("Blend", val_2);

        }
        private async Task StandUpAnim(float time ,CancellationToken token)
        {
            if(token.IsCancellationRequested == false)
            {
                mAnim.SetFloat("Blend", 0);
                if (mAnim != null)
                    mAnim.Play(AnimNames.StandUpAndRunTree, 0, 0);

                float timeElpased = 0f;
                float val_1 = 0f; // blend start val
                float val_2 = 1f; // blend end val
                while (timeElpased <= time && token.IsCancellationRequested == false)
                {
                    mAnim.SetFloat("Blend", Mathf.Lerp(val_1, val_2, timeElpased/time));
                    timeElpased += Time.deltaTime;
                    await Task.Yield();
                }
                if (token.IsCancellationRequested == false)
                    mAnim.SetFloat("Blend", val_2);
            }

        }
        #endregion



        // on collisions
        public void OnDummyCollision(Transform dummy)
        {
            if (CurrentState == DummyStates.Dead)
                return;
            DummyManager dum = dummy.gameObject.GetComponent<DummyManager>();
            if (CurrentState == DummyStates.Thrown)
            {
                //StopFlying();
                if (dum.CurrentState == DummyStates.Run || dum.CurrentState==DummyStates.Truck)
                {
                   
                    dum.PushDummy(rb.velocity/2);
                    GameManager.Instance.sounds.PlaySingleTime(Sounds.DummyCollision, audio);
                }
            } 
            else if(CurrentState == DummyStates.Drag) 
            {
                DragTarget.BreakConnection();
                OnDragStop();
                GameManager.Instance.sounds.PlaySingleTime(Sounds.DummyCollision, audio);
                if (dum.CurrentState == DummyStates.Run || dum.CurrentState == DummyStates.Truck)
                {
                    dum.PushDummy(rb.velocity / 2);
                }
            }

        }
        public void PushDummy(Vector3 force)
        {
            //mAnim.Play(AnimNames.LongFall, 0, 0);
            CurrentState = DummyStates.Thrown;
            if(rb != null)
                rb.constraints = RigidbodyConstraints.None;
            StopMoving();
            rb.velocity = force;

        }

        //public void OnRagdollCollision(Vector3 dollVel)
        //{
        //    float VelocityThreshold = GameManager.Instance.data.currentInst.Data.dragData.VelocityThreshold;
        //    //Rigidbody rb = ragdoll.GetComponent<Rigidbody>();

        //    if (dollVel.magnitude >= VelocityThreshold)
        //    {
        //        //Debug.Log("ok: " + dollVel.magnitude);
        //        GameManager.Instance.eventManager.StrongDummyImpact.Invoke();
        //        SetDead();
        //        ragdollManager.SetActive();
        //        ragdollManager.PushDoll(rb.velocity / 2,false);
        //    }
        //    else
        //    {
        //        //Debug.Log("too small or too big: " + dollVel.magnitude);
        //    }
                
        //}





        public void OnBulletEnter()
        {
            if(CurrentState != DummyStates.Idle && CurrentState != DummyStates.Dead)
                Die();
        }


        private void OnWallEnter()
        {
            GameManager.Instance.sounds.PlaySingleTime(Sounds.WallHit);
            //if (CurrentState == DummyStates.Thrown)
            //    StopFlying();
            if(CurrentState == DummyStates.Drag)
            {
                DragTarget.BreakConnection();
                OnDragStop();
            }
            Die();
        }
        private void OnPortalEnter(DummyPortal portal)
        {
            if (portal == null || CurrentState != DummyStates.Run)
                return;
            PortalData data = portal.GetOutPortalData();
            if (data == null)
                return;
            gameObject.transform.parent = null;
            Die();
            ragdollManager.HideCollider();
            ragdollManager.OnPortalEnter(portal);
            
            //portal.ShowEffect();
            //StartCoroutine(PortalTransition(portal));
        }
        private IEnumerator PortalTransition(DummyPortal portal)
        {
            disableToken?.Cancel();
            _renderer.enabled = false;
            yield return null;
            transform.rotation = Quaternion.LookRotation(Vector3.up);
            //Die();
            if (Detecting != null)
                StopCoroutine(Detecting);
            SetDead();
            SetRagdoll();
            DragTarget.SetNonDraggable();
            yield return null;
            ragdollManager.OnPortalEnter(portal);

        }







        public void PushFromTruck(Vector3 origin, float force)
        {
            CurrentState = DummyStates.Run;
            PushAway(origin, force);
        }


        public void Slice(Plane plane)
        {
            if (this == null ||   CurrentState == DummyStates.Dead)
            {
                return;
            }
            if(CurrentState == DummyStates.Drag || ragdollManager.CurrentState == RagdollStates.Drag)
            {
                DragTarget.BreakConnection();
            }
            DragTarget.SetNonDraggable();
            GameManager.Instance.sounds.PlaySingleTime(Sounds.AxeHit);
            StartCoroutine(Slicing( plane));
        }
        public void SliceForced(Plane plane)
        {
            DragTarget.SetNonDraggable();
            GameManager.Instance.sounds.PlaySingleTime(Sounds.AxeHit);
            StartCoroutine(Slicing(plane));
        }

        private IEnumerator Slicing(Plane plane)
        {
            gameObject.transform.parent = null;
            gameObject.tag = "Untagged";
            SetDead();
            SetRagdoll(false);
            DragTarget.SetNonDraggable();
            Destroy(capColl);
            Destroy(TriggerColl);
            yield return null;
            _sclicer.SliceFrameDelayed(plane, 1, null);
            Destroy(this);
            
        

        }


        public void PushAway(Vector3 origin, float force)
        {
            if (CurrentState != DummyStates.Dead && CurrentState != DummyStates.Idle && CurrentState != DummyStates.Drag)
                Die();
            ragdollManager.PushDoll((transform.position - origin).normalized*force,false);
        }
        public void Slash(Plane plane)
        {
            DragTarget.SetNonDraggable();
            GameManager.Instance.sounds.PlaySingleTime(Sounds.AxeHit);
            StartCoroutine(Slicing(plane));
        }
        public void KillAndPush(Vector3 force)
        {
            if (CurrentState == DummyStates.Run || CurrentState == DummyStates.Thrown)
            {
                GameManager.Instance.sounds.PlaySingleTime(Sounds.BatHit);
                Die();
                ragdollManager.PushDoll(force,false);
            }
        }

        public Transform GetTransform()
        {
            return transform;
        }
        // end taking damage




        public void Defeated()
        {
            IsDefeated = true;
            if(capColl!=null)
                capColl.enabled = false;
            if (IsRunning == true)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                StopMoving();
                PlayDefeatAnim();
            }
        }
        public void Winner()
        {
            if (CurrentState == DummyStates.Run)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                if (disableToken != null)
                    disableToken.Cancel();
                StopMoving();
                PlayWinAnim();
            }
            else
                Die();
        }

        // Small helpers
        public void DisableDummy()
        {
            gameObject.SetActive(false);
        }
        public void SetGround(bool grounded) => IsGrounded = grounded;




        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case Tags.Portal:
                    OnPortalEnter(other.GetComponent<DummyPortal>());
                    break;
                case Tags.LazerBeam:
                    ISclicing weapon = other.gameObject.GetComponent<ISclicing>();
                    if(weapon != null)
                        Slice(weapon.GetSlicePlane());
                    break;
                case Tags.Wall:
                    OnWallEnter();
                    break;
                case Tags.Bullet:
                    OnBulletEnter();
                    break;

            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.collider.tag)
            {
                case Tags.Ground:
                    OnGroundTouch?.Invoke();
                    SetGround(true);
                    break;
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            switch (collision.collider.tag)
            {
                case Tags.Ground:
                    SetGround(false);
                    break;
            }
        }


        private void OnDestroy()
        {
            if (disableToken != null)
                disableToken.Cancel();
            if (jumpToken != null)
                jumpToken.Cancel();
            if (Flying != null) StopCoroutine(Flying);
            if (Detecting != null) StopCoroutine(Detecting);
            if (SideMoment != null) StopCoroutine(SideMoment);
        }
        private void OnDisable()
        {
            if(disableToken != null)
                disableToken.Cancel();
            if (jumpToken != null)
                jumpToken.Cancel();
            if (Flying != null) StopCoroutine(Flying);
            if (Detecting != null) StopCoroutine(Detecting);
            if (SideMoment != null) StopCoroutine(SideMoment);

        }


    }

}
