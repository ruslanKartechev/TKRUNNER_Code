using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Commongame.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
public class RunnerManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Animator mAnim;
    [SerializeField] protected SplineFollower follower;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected int BaseAnimLayer = 0;
    // [SerializeField] protected SplineProjector projector;
    [Header("Settings")]
    public bool UseBlendStart = true;
    public bool IsRunning;
   // private MovementData data;

    protected float mainSpeed;
    protected float speedModifier = 1f;
    protected Coroutine movementRoutine;
    public float SpeedModifier { get { return speedModifier; } }
    public Rigidbody RB { get { return rb; } }

    protected CancellationTokenSource accelerationSource;

    protected CancellationTokenSource movementStart;

    protected Action OnRunStarted;

    private void Awake()
    {
        if (follower == null)
            follower = GetComponent<SplineFollower>();
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    public virtual void InitActive(SplineComputer spline)
    {
        if (follower == null)
            follower = GetComponent<SplineFollower>();
        follower.spline = spline;
        SetupFollower();
        
    }
    public virtual void SetupFollower()
    {
        if (follower == null) return;
        follower.motion.offset = Vector2.zero;
        follower.follow = false;
        follower.enabled = true;
        SplineSample result = new SplineSample();
        follower.Project(transform.position, result);
        Vector3 dist = (transform.position - result.position);
        follower.motion.applyPosition = true;
        follower.motion.applyRotation = true;
        float Xoffset = Vector3.Dot(dist,follower.result.right);
        follower.motion.offset = new Vector2(Xoffset, follower.motion.offset.y);
        follower.SetPercent(result.percent);

        follower.Project(transform.position, follower.result);

    }

    public async Task StartMoving(bool blendAnim = true, bool playAnim = true)
    {
        movementStart = new CancellationTokenSource();
        IsRunning = true;
        SetupFollower();
        if (blendAnim == true)
        {
            await BlendStart(movementStart.Token);
        }
        else
        {
            if (playAnim == true)
                mAnim.Play(AnimNames.DummyRun, BaseAnimLayer, (float)UnityEngine.Random.Range(0f, 1f));
            if (movementStart.Token.IsCancellationRequested == false)
                StartSplineMovement();
        }
    }
    public virtual void StartImmidiate(bool animate)
    {
        IsRunning = true;
        follower.enabled = true;
        follower.follow = true; 
        StartSplineMovement(animate);
    }

    protected async Task BlendStart(CancellationToken token)
    {
        List<Task> tasks = new List<Task>(2);
        tasks.Add( IdleRunStart( token) );
        tasks.Add( IdleRunAnimBlend( token) );
        await Task.WhenAll(tasks);
    }

    protected async Task IdleRunStart(CancellationToken token)
    {
        float time = 1f;
        float elapsed = 0f;
        float endModifier = 1;
        follower.follow = true;
        speedModifier = 0;
        StartSplineMovement();
        while (elapsed <= time && token.IsCancellationRequested == false)
        {
            speedModifier = Mathf.Lerp(0f, endModifier, elapsed / time);
            follower.followSpeed = mainSpeed * speedModifier;
            SetAnimSpeed(SpeedModifier);
            elapsed += Time.deltaTime;
            await Task.Yield();
        }
        speedModifier = 1;
        follower.followSpeed = mainSpeed * speedModifier;
        SetAnimSpeed(SpeedModifier);
    }

    protected async Task IdleRunAnimBlend(CancellationToken token)
    {
        mAnim.Play(AnimNames.IdleRunTree, BaseAnimLayer, UnityEngine.Random.Range(0,0.5f));

        float time = 1f;
        float elapsed = 0f;
        while(elapsed <= time && token.IsCancellationRequested == false)
        {
            mAnim.SetFloat("Blend",elapsed*(1/time));
            elapsed += Time.deltaTime;
            await Task.Yield();
        }
        mAnim.SetFloat("Blend", 1f);
        OnRunStarted?.Invoke();


    }
    protected virtual void StartSplineMovement(bool animate = true)
    {
        IsRunning = true;
        if(animate)
            mAnim.Play(AnimNames.DummyRun, BaseAnimLayer, (float)UnityEngine.Random.Range(0f, 1f));
        follower.followSpeed = mainSpeed * speedModifier;
        follower.enabled = true;
        follower.follow = true;
    }

    public async Task Accelerate(float time, float NewModifier, float Duration = 0f)
    {
        accelerationSource = new CancellationTokenSource();
        float startVal = speedModifier;
        await SpeedChanging(time, NewModifier, accelerationSource.Token);
        if (Duration != 0)
        {
            await Task.Delay((int)(Duration*1000));
            await SpeedChanging(time, startVal, accelerationSource.Token);
        }

    }
    public async Task SpeedChangingDirect(float time, float newFollowSpeed, CancellationToken token)
    {
        float timeElapsed = 0f;
        float startVal = follower.followSpeed;
        while (timeElapsed <= time && token.IsCancellationRequested == false)
        {
            
            follower.followSpeed = Mathf.Lerp(startVal,newFollowSpeed,timeElapsed/time);
            timeElapsed += Time.deltaTime;
            await Task.Yield();
        }
        if (token.IsCancellationRequested == false)
        {
            follower.followSpeed = newFollowSpeed;
        }
    }
    public async Task SpeedChanging(float time, float newModifier, CancellationToken token)
    {
        float timeElapsed = 0f;
        float startVal = speedModifier;
        while (timeElapsed <= time && token.IsCancellationRequested == false)
        {
            speedModifier = Mathf.Lerp(startVal, newModifier, timeElapsed / time);
            follower.followSpeed = mainSpeed * speedModifier;
            SetAnimSpeed(SpeedModifier);
            timeElapsed += Time.deltaTime;
            await Task.Yield();
        }
        if (token.IsCancellationRequested == false)
        {
            speedModifier = newModifier;
            follower.followSpeed = mainSpeed * speedModifier;
            SetAnimSpeed(SpeedModifier);
        }
    }


    protected virtual void SetAnimSpeed(float speed)
    {
        if(mAnim != null)
        {
            if (speed <= 1.5f && mAnim.speed >= 0)
                mAnim.speed = speed;
        }
    }

    public virtual void StopMoving()
    {   
        IsRunning = false;
        follower.enabled = false;
        follower.follow = false;
        if (movementRoutine != null)
            StopCoroutine(movementRoutine);
    }

    public virtual void PlayWinAnim()
    {
        mAnim.Play(AnimNames.Win, BaseAnimLayer, UnityEngine.Random.Range(0f, 1f));
    }
    public virtual void PlayDefeatAnim()
    {
        mAnim.Play(AnimNames.Defeat, BaseAnimLayer, UnityEngine.Random.Range(0f, 1f));

    }

    private void OnDisable()
    {
        if(accelerationSource != null)
            accelerationSource.Cancel();
    }
}