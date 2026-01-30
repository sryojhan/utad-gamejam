using System;
using System.Collections.Generic;
using UnityEngine;



[DefaultExecutionOrder(-2000)]
public class EventBus : Singleton<EventBus>
{
    protected override bool AutoInitialise => true;

    private readonly Dictionary<Type, Delegate> subscribers = new();

    public static void Subscribe<EventType>(Action<EventType> evt) where EventType : class
    {
        Type type = typeof(EventType);

        if (!instance.subscribers.TryGetValue(type, out var existing))
            instance.subscribers[type] = evt;
        else
            instance.subscribers[type] = Delegate.Combine(existing, evt);
    }

    public static void Subscribe<EventType>(Action paramlessAction) where EventType : class
    {
        Type type = typeof(EventType);

        if (!instance.subscribers.TryGetValue(type, out var existing))
            instance.subscribers[type] = paramlessAction;
        else
            instance.subscribers[type] = Delegate.Combine(existing, paramlessAction);
    }


    public static void Unsubscribe<EventType>(Action<EventType> evt) where EventType : class
    {
        if (!IsInitialised()) return;

        var type = typeof(EventType);

        if (!instance.subscribers.TryGetValue(type, out var existing)) return;

        var newDel = Delegate.Remove(existing, evt);

        if (newDel == null) instance.subscribers.Remove(type);
        else instance.subscribers[type] = newDel;
    }

    public static void Unsubscribe<EventType>(Action paramlessAction) where EventType : class
    {
        if (!IsInitialised()) return;

        var type = typeof(EventType);

        if (!instance.subscribers.TryGetValue(type, out var existing)) return;

        var newDel = Delegate.Remove(existing, paramlessAction);

        if (newDel == null) instance.subscribers.Remove(type);
        else instance.subscribers[type] = newDel;
    }


    public static void Raise<EventType>(EventType data = null) where EventType : class
    {
        Type type = typeof(EventType);

        if (!instance.subscribers.TryGetValue(type, out Delegate evt))
        {
            return;
        }
        if (evt is Action<EventType> action) action?.Invoke(data);
        else (evt as Action)?.Invoke();
    }



    /*
     * 
     * TODO: mover esto a otra clase
     * 
   * Los eventos se pueden lanzar de distinta forma
   * 
   * 
   *  - Inmediata, lanzados con un Raise normal
   *  - Proximo frame, lanzados con un RaiseNextFrame
   *  - Pasado un tiempo, lanzados con un RaiseAfter
   *  - Dada una condición
   */

    public abstract class ProgrammableEvent
    {
        public bool IsCancelled { get; private set; } = false;
        public void Cancel()
        {
            IsCancelled = true;
        }

        public Action DeferredEvent;

        public abstract void Update();
        public abstract bool IsConditionMet();
    }

    private readonly List<ProgrammableEvent> remainingEvents = new();

    private void Update()
    {
        remainingEvents.RemoveAll(evt => evt.IsCancelled);

        remainingEvents.ForEach(evt => evt.Update());
        remainingEvents.RemoveAll(evt =>
        {
            if(evt.IsConditionMet())
            {
                evt.DeferredEvent();
                return true;
            }
            return false;
        });
    }


    public static void RaiseNextFrame<EvtData>(EvtData data = null) where EvtData : class
    {
        RaiseAfterFrames(1, data);
    }


    public static void RaiseAfterFrames<EvtData>(uint frameCount, EvtData data = null) where EvtData : class
    {
        WaitFramesEvent framesEvent = new(frameCount)
        {
            DeferredEvent = () =>
            {
                Raise<EvtData>(data);
            }
        };

        instance.remainingEvents.Add(framesEvent);
    }


    public static void RaiseAfterSeconds<EvtData>(float seconds, EvtData data = null) where EvtData : class
    {
        WaitSecondsEvent secondsEvent = new(seconds)
        {
            DeferredEvent = () =>
            {
                Raise<EvtData>(data);
            }
        };

        instance.remainingEvents.Add(secondsEvent);

    }

    public static void RaiseIfCondition<EvtData>(WaitConditionEvent.Condition condition, EvtData data = null) where EvtData : class
    {
        WaitConditionEvent conditionEvent = new(condition)
        {
            DeferredEvent = () =>
            {
                Raise<EvtData>(data);
            }
        };

        instance.remainingEvents.Add(conditionEvent);
    }




    public class WaitFramesEvent : ProgrammableEvent
    {
        public uint RemainingFrames { get; private set; } = 0;
        public WaitFramesEvent(uint count)
        {
            RemainingFrames = count;
        }

        public override void Update()
        {
            if (RemainingFrames > 0)
                RemainingFrames--;
        }

        public override bool IsConditionMet()
        {
            return RemainingFrames == 0;
        }
    }


    public class WaitSecondsEvent : ProgrammableEvent
    {
        public float RemainingSeconds { get; private set; } = 0;
        public WaitSecondsEvent(float seconds)
        {
            RemainingSeconds = seconds;
        }

        public override void Update()
        {
            RemainingSeconds -= Time.deltaTime;
        }

        public override bool IsConditionMet()
        {
            return RemainingSeconds <= 0;
        }
    }

    public class WaitConditionEvent : ProgrammableEvent
    {
        public delegate bool Condition();

        public Condition ConditionCallback { get; private set; }

        public WaitConditionEvent(Condition condition)
        {
            ConditionCallback = condition;
        }

        public override void Update()
        {

        }

        public override bool IsConditionMet()
        {
            return ConditionCallback();
        }
    }
}

