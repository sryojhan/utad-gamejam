using UnityEngine;


[RequireComponent(typeof(Animator))]
public class StopMotionAnimator : MonoBehaviour
{
    //TODO: mejorar un poquito este script
    public Animator animator;
    [Header("Stop motion")]
    public float frameRate = 12.0f;
    public bool enableStopMotion = true;

    private float timer;
    private float _frameDuration;

    [Tooltip("Dont skip event frames")]
    public bool snapToEvents = true;


    [Header("Rotation")]
    [SerializeField]
    bool enableAngleSnap = true;

    private bool overrideAngleSnap;

    public float angleStep = 30;

    void Awake()
    {
        overrideAngleSnap = enableAngleSnap;

        CalculateFrameDuration();
    }

    void OnValidate()
    {
        if (animator == null) animator = GetComponent<Animator>();
        CalculateFrameDuration();
    }

    void Update()
    {
        ManageStopMotion();
    }
    private void LateUpdate()
    {
        ManageAngleSnap();
    }

    void ManageStopMotion()
    {
        if (!enableStopMotion)
        {
            if (!animator.enabled) animator.enabled = true;
            return;
        }

        if (animator.enabled) animator.enabled = false;

        // Acumulamos tiempo real
        timer += Time.deltaTime;

        // Bucle para procesar frames pendientes (por si hay lag)
        while (timer >= _frameDuration)
        {
            float stepToTake = _frameDuration;
            bool didSnap = false;

            if (snapToEvents)
            {
                float distToEvent = GetClosestEvent(stepToTake);

                // Si hay un evento más cerca que nuestro paso normal...
                if (distToEvent > 0)
                {
                    // Ajustamos el paso para caer justo encima (más el epsilon)
                    stepToTake = distToEvent;
                    didSnap = true;
                }
            }

            // 1. Avanzamos el Animator
            animator.Update(stepToTake);

            // 2. Restamos el tiempo consumido al acumulador
            timer -= stepToTake;

            // 3. Si hicimos snap, ROMPEMOS el bucle aquí.
            // Queremos que Unity renderice este frame exacto para que el jugador vea el impacto.
            if (didSnap)
            {
                break;
            }
        }
    }

    private float GetClosestEvent(float maxCheckTime)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (clipInfo.Length == 0) return -1f;

        AnimationClip clip = clipInfo[0].clip;

        // 1. Calculamos la velocidad real (Velocidad del Estado * Velocidad Global del Animator)
        // Usamos Abs porque si la animación va hacia atrás (-1), la lógica de eventos cambia,
        // pero para Stop Motion asumiremos que queremos saber "cuánto avanza" en magnitud.
        float effectiveSpeed = Mathf.Abs(stateInfo.speed * animator.speed);

        // Si la velocidad es 0 (pausa), no vamos a llegar a ningún evento.
        if (effectiveSpeed < 0.0001f) return -1f;

        // 2. Convertimos el tiempo de "Juego" a tiempo de "Clip"
        // Si la velocidad es 2, en 0.1s de juego avanzamos 0.2s de clip.
        float predictedClipDelta = maxCheckTime * effectiveSpeed;

        float currentAnimTime;
        if (stateInfo.loop)
            currentAnimTime = (stateInfo.normalizedTime % 1f) * clip.length;
        else
            currentAnimTime = Mathf.Clamp01(stateInfo.normalizedTime) * clip.length;

        // Usamos el delta proyectado corregido por la velocidad
        float futureAnimTime = currentAnimTime + predictedClipDelta;
        float closestDistanceInGameTime = -1f;

        foreach (var evt in clip.events)
        {
            float eventTime = evt.time;
            float distInClip = -1f;

            // Caso A: Evento normal hacia adelante
            if (eventTime > currentAnimTime && eventTime <= futureAnimTime)
            {
                distInClip = eventTime - currentAnimTime;
            }
            // Caso B: Loop
            else if (stateInfo.loop && futureAnimTime > clip.length && eventTime < (futureAnimTime % clip.length))
            {
                distInClip = (clip.length - currentAnimTime) + eventTime;
            }

            if (distInClip >= 0)
            {
                // 3. ¡IMPORTANTE! Deshacemos la conversión de velocidad.
                // Necesitamos devolver cuánto tiempo REAL de juego (timer) falta para llegar.
                // TiempoJuego = TiempoClip / Velocidad
                float distInGameTime = distInClip / effectiveSpeed;

                if (closestDistanceInGameTime == -1 || distInGameTime < closestDistanceInGameTime)
                {
                    closestDistanceInGameTime = distInGameTime;
                }
            }
        }

        if (closestDistanceInGameTime > 0) closestDistanceInGameTime += 0.001f;

        return closestDistanceInGameTime;
    }



    void ManageAngleSnap()
    {
        if (!enableAngleSnap || !overrideAngleSnap) return;
        float cameraAngle = Camera.main.transform.eulerAngles.y;
        float parentAngle = transform.parent.eulerAngles.y;

        float relativeAngle = parentAngle - cameraAngle;
        float snappedRelative = Mathf.Round(relativeAngle / angleStep) * angleStep;

        float finalAngle = snappedRelative + cameraAngle;
        transform.rotation = Quaternion.Euler(transform.parent.eulerAngles.x, finalAngle, transform.parent.eulerAngles.z);
    }

    public void EnableAngleSnap()
    {
        overrideAngleSnap = true;
        transform.localRotation = Quaternion.identity;
    }

    public void DisableAngleSnap()
    {
        overrideAngleSnap = false;
        transform.localRotation = Quaternion.identity;
    }


    private void CalculateFrameDuration()
    {
        if (frameRate > 0)
            _frameDuration = 1.0f / frameRate;
        else
            _frameDuration = 0f;
    }


    public void SetStopMotionEffect(bool isActive)
    {
        enableStopMotion = isActive;

        if (!isActive)
        {
            animator.enabled = true;
        }
    }

    public void SetFrameRate(float newFps)
    {
        frameRate = newFps;
        CalculateFrameDuration();
    }

}