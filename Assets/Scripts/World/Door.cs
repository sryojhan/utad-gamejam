using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Interactable))]
public class Door : MonoBehaviour, IInteraction
{
    public string id;

    private void Start()
    {
        GetComponent<Interactable>().SetInteraction(this);
    }

    public void Begin()
    {
        string other = DoorManager.GetPairScene(id);

        SceneManager.LoadScene(other);
    }

    public void ForceEnd()
    {
    }

    public bool IsDone()
    {
        return true;
    }

}
