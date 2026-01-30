using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class Utils
{
    public static float GetAngle(Vector2 position, Vector2 origin)
    {
        Vector2 dir = (position - origin).normalized;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }


    public static Vector3 ProjectIn3D(Vector2 dir2d, float verticalValue = 0)
    {
        Vector3 dir3d = Vector3.zero;

        dir3d.x = dir2d.x;
        dir3d.y = verticalValue;
        dir3d.z = dir2d.y;

        return dir3d;
    }

    public static Vector2 AdjustToCamera(Vector2 dir)
    {
        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 ret = forward * dir.y + right * dir.x;

        return new Vector2(ret.x, ret.z);
    }

    public static Vector3 Flatten(Vector3 v)
    {
        v.y = 0;
        return v;
    }


    public static T Choose<T>(params T[] values)
    {
        int r = Random.Range(0, values.Length);
        return values[r];
    }


    public static void Repeat(int count, System.Action action)
    {
        for (int i = 0; i < count; i++)
            action?.Invoke();
    }


    public static void Delay(float time, System.Action action)
    {
        Motion.Utils.DelayedCall(time, action);
    }


    public static class Editor
    {

        public static void SetDirty(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(obj);
#endif
        }
    }

}
