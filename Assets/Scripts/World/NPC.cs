using UnityEngine;


[RequireComponent(typeof(Interactable))]
public class NPC : MonoBehaviour, IInteraction
{
    public Dialogue[] dialogue;

    public bool loopDialogues = false;

    public PersistentData<int> currentDialogue = new();

    public void Begin()
    {
        DialogueController.instance.BeginDialogue(dialogue[currentDialogue.value]);


        if (currentDialogue.value == dialogue.Length - 1)
        {
            if (loopDialogues) currentDialogue.value = 0;
        }
        else
        {
            currentDialogue.value++;
        }
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
        currentDialogue.Load();
        GetComponent<Interactable>().SetInteraction(this);
    }

    private void OnDestroy()
    {
        if (Progress.IsInitialised())
            currentDialogue.Save();
    }
}
