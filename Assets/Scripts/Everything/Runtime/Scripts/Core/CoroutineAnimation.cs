using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Todo: cambiar el nombre a motion

/*
 TODO: Entonces. Ahora Motion hará:

    - Clase estática (o un singleton) que maneja todas las corrutinas (dont destroy on load)
    - MotionHandler clase intermedia con la informacion de la corrutina. Información para poder encadenar, interrumpir, pausar
    - MotionData: clase generica serializable para almacenar los datos de una animacion simple. Duracion, interpolado, lo que hay ya ahora vaya
    - Añadir shake, circlelerp, smoothdamp y curve animation (hay alguna forma de configurar animation curves para que el valor minimo y maximo en x sea de 0 a 1?
    - Soporte para recorrer un spline
 */

[System.Serializable]
public class CoroutineAnimation
{
    public delegate void OnValueUpdate(float value);
    public delegate void Callback();

    public float duration = 1;
    public float delay = 0;

    public Interpolation interpolation;

    private float progress = 0;
    private Coroutine coroutine = null;
    //TODO: cambiarlo para que soporte usar la misma clase en varias entidades

    //TODO: lerping float


    //TODO: se debe ejecutar el onBegin antes de acceder a los valores de la corrutina por si el valor inicial cambia en el onBegin

    public virtual void Play(MonoBehaviour behaviour, OnValueUpdate onUpdate = null, Callback onBegin = null, Callback onEnd = null, bool useUnscaledTime = false, float? customDuration = null)
    {
        if(coroutine != null)
        {
            behaviour.StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = behaviour.StartCoroutine(
            Coroutine(onUpdate, onBegin, onEnd, useUnscaledTime, customDuration ?? duration)
            );
    }

    public IEnumerator Coroutine(OnValueUpdate update, Callback begin, Callback onFinish, bool useUnscaledTime, float duration)
    {
        yield return new WaitForSeconds(delay);
        
        begin?.Invoke();
        update?.Invoke(interpolation.Interpolate(0));

        progress = 0;
        float invDuration = 1.0f / duration;

        for(float i = 0; i < 1; i += invDuration * (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime))
        {
            progress = i;
            update?.Invoke(interpolation.Interpolate(i));
            yield return null;
        }

        update?.Invoke(interpolation.Interpolate(1.0f));

        progress = 1;
        coroutine = null;

        onFinish?.Invoke();
    }

    public bool IsFinished => coroutine == null;

    public float GetProgess()
    {
        return progress;
    }

    public void Stop(MonoBehaviour behaviour)
    {
        if(coroutine != null)
        {
            behaviour.StopCoroutine(coroutine);
            coroutine = null;
        }
    }


    public void Lerp(MonoBehaviour obj, SmartFloat number, float target, OnValueUpdate onUpdate = null, Callback onBegin = null, Callback onEnd = null, bool useUnscaledTime = false)
    {
        float initial = number;

        void OnUpdate(float i)
        {
            number.Value = Mathf.LerpUnclamped(initial, target, i);
            onUpdate?.Invoke(i);
        }

        Play(obj, OnUpdate, onBegin, onEnd, useUnscaledTime: useUnscaledTime);
    }


    public void MoveTo(MonoBehaviour obj, RectTransform tr, Vector2 finalPosition, OnValueUpdate onUpdate = null, Callback onBegin = null, Callback onEnd = null, bool useUnscaledTime = false)
    {
        Vector2 initialPosition = tr.anchoredPosition;

        void OnUpdate(float i)
        {
            tr.anchoredPosition = Vector2.LerpUnclamped(initialPosition, finalPosition, i);

            onUpdate?.Invoke(i);
        }

        Play(obj, OnUpdate, onBegin, onEnd, useUnscaledTime: useUnscaledTime);
    }


    public void MoveTo(MonoBehaviour obj, Transform tr, Vector3 finalPosition, OnValueUpdate onUpdate = null, Callback onBegin = null, Callback onEnd = null, bool local = true, bool useUnscaledTime = false, float? customDuration = null)
    {
        Vector3 initialPosition = local ? tr.localPosition : tr.position;

        void OnUpdate(float i)
        {
            if(local)
                tr.localPosition = Vector2.LerpUnclamped(initialPosition, finalPosition, i);
            else
                tr.position = Vector3.LerpUnclamped(initialPosition, finalPosition, i);

            onUpdate?.Invoke(i);
        }

        Play(obj, OnUpdate, onBegin, onEnd, useUnscaledTime: useUnscaledTime, customDuration: customDuration);
    }

    //TODO: puede ser negativa la escala

    public void ScaleTo(MonoBehaviour obj, Transform tr, Vector3 finalScale, OnValueUpdate onUpdate = null, Callback onBegin = null, Callback onEnd = null, bool useUnscaledTime = false)
    {
        Vector3 initialScale = tr.localScale;

        void OnUpdate(float i)
        {
            tr.localScale = Vector3.LerpUnclamped(initialScale, finalScale, i);

            onUpdate?.Invoke(i);
        }
        
        Play(obj, OnUpdate, onBegin, onEnd, useUnscaledTime: useUnscaledTime);
    }
    

    public void RotateTo(MonoBehaviour obj, Transform tr, Quaternion finalRotation, OnValueUpdate onUpdate = null, Callback onBegin = null, Callback onEnd = null, bool useUnscaledTime = false)
    {
        Quaternion initialRotation = tr.localRotation;

        void OnUpdate(float i)
        {
            tr.localRotation = Quaternion.LerpUnclamped(initialRotation, finalRotation, i);

            onUpdate?.Invoke(i);
        }

        Play(obj, OnUpdate, onBegin, onEnd, useUnscaledTime: useUnscaledTime);
    }



    /// <summary>
    /// The duration param will now serve as the speed. 
    /// </summary>
    public void MoveAtConstantPace(MonoBehaviour obj, Transform tr, Vector3 finalPosition, OnValueUpdate onUpdate = null, Callback onBegin = null, Callback onEnd = null, bool local = true, bool useUnscaledTime = false)
    {
        Vector3 initialPosition = local ? tr.localPosition : tr.position;

        float distance = Vector3.Distance(initialPosition, finalPosition);
        float duration = distance / this.duration;

        MoveTo(
            obj: obj,
            tr: tr,
            finalPosition: finalPosition,
            onUpdate: onUpdate,
            onBegin: onBegin,
            onEnd: onEnd,
            useUnscaledTime: useUnscaledTime,
            local: local,
            customDuration: duration
        );
    }

}

