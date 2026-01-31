using Player;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public float maxJump = 1;
    public float jumpInterval = 1;

    Vector3 originalPosition;

    private float t = 0;

    private bool isMoving;
    private bool facingRight;


    public MotionSettings rotationSettings;

    private ParticleSystem steps;

    private void Start()
    {
        facingRight = true;
        isMoving = false;

        originalPosition = transform.localPosition;

        steps = transform.parent.GetComponentInChildren<ParticleSystem>();
    }



    MotionHandler rotation;

    public void Update()
    {
        Vector3 velocity = PlayerController.Movement.Velocity;
        float speed = velocity.sqrMagnitude;

        bool isMovingNow = speed > .25f;

        if (isMovingNow != isMoving)
        {
            if (!isMovingNow)
            {
                t = 0;
                transform.localPosition = originalPosition;

                steps.Stop();
            }
            else
            {
                steps.Play();
            }

            isMoving = isMovingNow;
        }

        if (isMoving)
        {
            t += Time.deltaTime * jumpInterval;

            transform.localPosition = originalPosition + Vector3.up *
                Mathf.Abs(Mathf.Sin(t) * maxJump);

            steps.transform.forward = velocity.normalized;
        }


        if (velocity.x != 0)
        {

            bool facingRightNow = velocity.x >= 0;

            if (facingRight != facingRightNow)
            {
                rotation?.Stop();

                rotation = Motion.Rotate(transform,
                    Quaternion.Euler(0, facingRightNow ? 0 : 180, 0)).Settings(rotationSettings).Play();

                facingRight = facingRightNow;
            }
        }

    }


}
