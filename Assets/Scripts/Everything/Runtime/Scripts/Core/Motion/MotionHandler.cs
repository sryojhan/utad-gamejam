using System;
using System.Collections;
using UnityEngine;


public class MotionHandler
{
    //Control data
    private bool isPlaying;
    private bool isPaused;
    private bool hasBeginLoop;
    private Coroutine coroutine;

    float progress;

    //Target data
    private bool hasTarget;
    private GameObject target;

    private Action<GameObject> targetBegin;
    private Action<GameObject, float> targetUpdate;
    private Action<GameObject> targetEnd;

    private MotionSettings settings;

    // Callbacks

    private event Action OnBegin;
    private event Action<float> OnUpdate;
    private event Action OnEnd;


    public MotionHandler Play()
    {
        if (isPlaying)
        {
            Debug.LogWarning("Play was called on already playing motion");
            Stop();
        }

        isPlaying = true;
        hasBeginLoop = false;
        progress = settings.playFrom;
        coroutine = Motion.instance.StartCoroutine(MainCoroutine());

        return this;
    }
    public void Stop()
    {
        isPlaying = false;

        if (coroutine != null)
        {
            Motion.instance.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void PlayFrom(float i)
    {
        if (i < 0 || i > 1)
        {
            //maybe this is what we want so dont throw error directly
            Debug.LogWarning("Play from out of range");
        }

        progress = i;
    }

    public void Pause()
    {
        if (!hasBeginLoop)
        {
            Debug.LogError("Tried to pause coroutine that has not began yet");
            return;
        }

        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void ResumeFrom(float i)
    {
        isPaused = false;

        if (hasBeginLoop)
        {
            if (i < 0 || i > 1)
            {
                //maybe this is what we want so dont throw error directly
                Debug.LogWarning("Play from out of range");
            }

            progress = i;
        }
        else
        {
            Debug.LogError("Tried to use 'ResumeFrom' when motion has not started");
        }
    }

    public float Progress => progress;
    public bool IsFinished => !isPlaying && hasBeginLoop;

    private IEnumerator MainCoroutine()
    {
        yield return new WaitForSeconds(settings.delay);

        CallbacksBegin();

        hasBeginLoop = true;
        CallbacksUpdate(0);

        float invDuration = 1.0f / settings.duration;
        for (; progress < 1; progress += invDuration * (settings.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime))
        {
            while (isPaused) //wait until pause is removed
                yield return null;

            CallbacksUpdate(settings.interpolation.Interpolate(progress));

            //wait for next frame
            yield return null;
        }

        CallbacksUpdate(1);

        progress = 1;
        CallbacksEnd();

        Stop();
    }

    private bool IsTargetValid()
    {
        if (!hasTarget) return false; //If no target
        {
            if (target == null)
            {
                //Error with target
                Debug.LogError("Target has been eliminated");
                //TODO: remove from Motion controller
                Stop();

                return false;
            }

            return true;
        }
    }


    private void CallbacksBegin()
    {
        if (IsTargetValid())
        {
            targetBegin?.Invoke(target);
        }
        OnBegin?.Invoke();
    }
    private void CallbacksUpdate(float i)
    {
        if (IsTargetValid())
        {
            targetUpdate?.Invoke(target, i);
        }
        OnUpdate?.Invoke(i);
    }
    private void CallbacksEnd()
    {
        if (IsTargetValid())
        {
            targetEnd?.Invoke(target);
        }

        OnEnd?.Invoke();
    }

    // Configuration api

    public MotionHandler UpdateCallback(Action<float> onUpdate)
    {
        OnUpdate += onUpdate;
        return this;
    }

    public MotionHandler BeginCallback(Action onBegin)
    {
        OnBegin += onBegin;
        return this;
    }

    public MotionHandler EndCallback(Action onEnd)
    {
        OnEnd += onEnd;
        return this;
    }


    public MotionHandler Settings(MotionSettings refSettings)
    {
        if (refSettings == null)
        {
            settings = new();
        }
        else
        {
            settings = MotionSettings.Clone(refSettings);
        }

        return this;
    }

    public MotionHandler Duration(float duration)
    {
        settings.duration = duration;
        return this;
    }

    public MotionHandler Interpolation(Interpolation interpolation)
    {
        settings.interpolation = interpolation;
        return this;
    }

    public MotionHandler DontAutoPlay()
    {
        throw new NotImplementedException("Dont auto play not yet implemented");
    }

    public MotionHandler Target(GameObject target, Action<GameObject, float> update = null, Action<GameObject> begin = null, Action<GameObject> end = null)
    {
        if (hasTarget)
        {
            Debug.LogError("This motion handler already has a defined target");
            return this;
        }

        hasTarget = true;
        this.target = target;

        targetBegin = begin;
        targetUpdate = update;
        targetEnd = end;

        return this;
    }



}
