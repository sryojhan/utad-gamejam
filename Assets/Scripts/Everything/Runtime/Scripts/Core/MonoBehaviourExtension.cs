using UnityEngine;

public static class MonobehaviourExtension
{
    public static T GetOrAddComponent<T>(this MonoBehaviour obj) where T : MonoBehaviour
    {
        T comp = obj.GetComponent<T>();

        if (!comp) comp = obj.gameObject.AddComponent<T>();

        return comp;
    }

    public static void SetPositionX(this Transform tr, float xPos)
    {
        Vector3 pos = tr.position;
        pos.x = xPos;
        tr.position = pos;
    }

    public static void SetPositionY(this Transform tr, float yPos)
    {
        Vector3 pos = tr.position;
        pos.y = yPos;
        tr.position = pos;
    }

    public static void SetPositionZ(this Transform tr, float zPos)
    {
        Vector3 pos = tr.position;
        pos.z = zPos;
        tr.position = pos;
    }

    public static void VoidVerticalVelocity(this Rigidbody rb)
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0;
        rb.linearVelocity = velocity;
    }

    public static void MovePositionY(this Rigidbody rb, float yPos)
    {
        Vector3 pos = rb.position;
        pos.y = yPos;

        rb.MovePosition(pos);
    }


}
