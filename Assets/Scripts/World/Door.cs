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
        string other = DoorManager.GetPair(id);
        string scene = DoorManager.GetSceneFromDoor(other);

        Progress.instance.last_door_id = other;
        SceneManager.LoadScene(scene);
    }

    public void ForceEnd()
    {
    }

    public bool IsDone()
    {
        return true;
    }

}
