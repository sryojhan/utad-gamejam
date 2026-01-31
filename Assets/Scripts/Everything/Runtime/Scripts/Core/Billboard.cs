using UnityEngine;

public class Billboard : MonoBehaviour
{
    public enum Mode
    {
        LookAt, Align
    }

    public Transform target;
    public Mode mode;

    private void Reset()
    {
        target = Camera.main.transform;
    }

    [Button]
    private void Update()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(target);
                break;
            case Mode.Align:
                transform.rotation = Camera.main.transform.rotation;
                break;
        }
    }
}
