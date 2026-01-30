using Player;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public float maxJump = 1;
    public float jumpInterval = 1;

    Vector3 originalPosition;

    private float t = 0;


    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void Update()
    {
        float speed = PlayerController.Movement.Velocity.sqrMagnitude;

        if (speed <= .25f) transform.localPosition = originalPosition;
        else
        {
            t += Time.deltaTime * jumpInterval;

            transform.localPosition = originalPosition + Vector3.up *
                Mathf.Abs(Mathf.Sin(t) * maxJump);
        }
    }


}
