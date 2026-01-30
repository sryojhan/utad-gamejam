using Player;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public float maxJump = 1;
    public float jumpInterval = 1;

    Vector3 originalPosition;

    private float t = 0;

    private bool facingRight;

    public MotionSettings rotationSettings;

    private void Start()
    {
        facingRight = true;
        originalPosition = transform.localPosition;
    }



    MotionHandler rotation;

    public void Update()
    {
        Vector3 velocity = PlayerController.Movement.Velocity;
        float speed = velocity.sqrMagnitude;

        if (speed <= .25f) transform.localPosition = originalPosition;
        else
        {
            t += Time.deltaTime * jumpInterval;

            transform.localPosition = originalPosition + Vector3.up *
                Mathf.Abs(Mathf.Sin(t) * maxJump);
        }


        if (velocity.x != 0)
        {

            bool facingRightNow = velocity.x >= 0;

            if (facingRight != facingRightNow)
            {
                rotation?.Stop();

                rotation = Motion.Rotate(transform, Quaternion.Euler(0, facingRightNow ? 0 : 180, 0)).Settings(rotationSettings).Play();

                facingRight = facingRightNow;
            }
        }

    }


}
