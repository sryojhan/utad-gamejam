using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class Motion : Singleton<Motion>
{
    protected override bool AutoInitialise => true;

    Dictionary<GameObject, MotionHandler> handlers;

    public MotionHandler CreateDefaultHandler(MotionSettings settings = null)
    {
        MotionHandler handler = new();
        return handler.Settings(settings);
    }


    public static MotionHandler Lerp(Action<float> onUpdate, MotionSettings settings = null)
    {
        MotionHandler mh = instance.CreateDefaultHandler();

        mh.UpdateCallback(onUpdate);
        mh.Settings(settings);

        return mh;
    }


    public static MotionHandler Move(Transform tr, Vector3 destination, MotionSettings settings = null)
    {
        MotionHandler mh = instance.CreateDefaultHandler();

        Vector3 initialPosition = tr.position;


        void Begin(GameObject _)
        {
            initialPosition = tr.position;
        }

        void Update(GameObject _, float i)
        {
            tr.position = Vector3.LerpUnclamped(initialPosition, destination, i);
        }

        mh.Target(tr.gameObject, begin: Begin, update: Update);
        mh.Settings(settings);

        return mh;
    }


    public static MotionHandler MoveLocal(Transform tr, Vector3 destination, MotionSettings settings = null)
    {
        MotionHandler mh = instance.CreateDefaultHandler();

        Vector3 initialPosition = tr.localPosition;

        void Begin(GameObject _)
        {
            initialPosition = tr.localPosition;
        }

        void Update(GameObject _, float i)
        {
            tr.localPosition = Vector3.LerpUnclamped(initialPosition, destination, i);
        }

        mh.Target(tr.gameObject, begin: Begin, update: Update);

        mh.Settings(settings);

        return mh;
    }


    public static MotionHandler MoveUI(RectTransform rt, Vector2 destination, MotionSettings settings = null)
    {
        MotionHandler mh = instance.CreateDefaultHandler();

        Vector2 initialPosition = rt.anchoredPosition;

        void Begin(GameObject _)
        {
            initialPosition = rt.anchoredPosition;
        }

        void Update(GameObject _, float i)
        {
            rt.anchoredPosition = Vector2.LerpUnclamped(initialPosition, destination, i);
        }

        mh.Target(rt.gameObject, begin: Begin, update: Update);

        mh.Settings(settings);

        return mh;
    }


    public static MotionHandler SizeUI(RectTransform rt, Vector2 finalSize, MotionSettings settings = null)
    {
        MotionHandler mh = instance.CreateDefaultHandler();

        Vector2 initialSize = rt.sizeDelta;

        void Begin(GameObject _)
        {
            initialSize = rt.sizeDelta;
        }

        void Update(GameObject _, float i)
        {
            rt.sizeDelta = Vector2.LerpUnclamped(initialSize, finalSize, i);
        }

        mh.Target(rt.gameObject, begin: Begin, update: Update);
        mh.Settings(settings);

        return mh;
    }



    public static MotionHandler Scale(Transform tr, Vector3 finalScale, MotionSettings settings = null)
    {
        MotionHandler mh = instance.CreateDefaultHandler();

        Vector3 initialScale = tr.localScale;

        void Begin(GameObject _)
        {
            initialScale = tr.localScale;
        }

        void Update(GameObject _, float i)
        {
            tr.localScale = Vector3.LerpUnclamped(initialScale, finalScale, i);
        }

        mh.Target(tr.gameObject, begin: Begin, update: Update);
        mh.Settings(settings);

        return mh;
    }





    //TODO: rotation


    public static class Utils
    {
        //TODO: cambiar esto para que devuelva un handler y poder cancelarlo

        public static void DelayedCall(float t, Action action)
        {
            IEnumerator internal_delay_call()
            {
                yield return new WaitForSeconds(t);
                action?.Invoke();
            }

            instance.StartCoroutine(internal_delay_call());
        }
    }

}

