using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Door : MonoBehaviour, IInteraction
{
    private void Start()
    {
        GetComponent<Interactable>().SetInteraction(this);
    }

    public void Begin()
    {
        print("hola");
    }

    public void ForceEnd()
    {
    }

    public bool IsDone()
    {
        return true;
    }

}
