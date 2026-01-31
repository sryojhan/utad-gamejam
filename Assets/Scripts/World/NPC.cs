using UnityEngine;


[RequireComponent(typeof(Interactable))]
public class NPC : MonoBehaviour, IInteraction
{
    public Dialogue dialogue;

    public void Begin()
    {
        DialogueController.instance.BeginDialogue(dialogue);
    }

    public void ForceEnd()
    {
        throw new System.NotImplementedException();
    }

    public bool IsDone()
    {
        return !DialogueController.instance.IsInDialogue;
    }

    private void Start()
    {
        GetComponent<Interactable>().SetInteraction(this);
    }
}
