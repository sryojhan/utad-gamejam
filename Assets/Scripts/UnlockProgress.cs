using UnityEngine;

public class UnlockProgress : MonoBehaviour
{
    public string id;
    public void Unlock()
    {
        Progress.Unlock(id);
    }
}
