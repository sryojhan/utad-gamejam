using UnityEngine;
using UnityEngine.Events;



public class Interactable : MonoBehaviour
{
    [Header("Notification range")]
    private bool isInNotificationRange;
    public UnityEvent OnEnterNotificationRange;
    public UnityEvent OnExitNotificationRange;

    [Header("Interaction target")]
    private bool isInteractionTarget;
    public UnityEvent OnSetAsInteractionTarget;
    public UnityEvent OnRemoveAsInteractionTarget;

    [Header("Interaction")]
    [SerializeField]
    private float interactionRange = 1;

    private bool isInteractionOngoing;
    public string interactionPrompt = "Talk";

    public UnityEvent OnBeginInteraction;
    public UnityEvent OnEndInteraction;

    private IInteraction interaction;

    public void EnterNotificationRange()
    {
        isInNotificationRange = true;
        OnEnterNotificationRange.Invoke();
    }

    public void ExitNotificationRange()
    {
        isInNotificationRange = false;
        OnExitNotificationRange.Invoke();
    }

    public void SetAsInteractionTarget()
    {
        isInteractionTarget = true;
        OnSetAsInteractionTarget.Invoke();
    }

    public void RemoveAsInteractionTarget()
    {
        isInteractionTarget = false;
        OnRemoveAsInteractionTarget.Invoke();
    }


    public IInteraction BeginInteraction()
    {
        isInteractionOngoing = true;
        OnBeginInteraction.Invoke();

        return interaction;
    }

    public void EndInteraction()
    {
        isInteractionOngoing = false;
        OnEndInteraction.Invoke();
    }


    public void SetInteraction(IInteraction interaction)
    {
        this.interaction = interaction;
    }


    public float InteractionRangeSqred => interactionRange * interactionRange;
    public bool IsInteractionTarget => isInteractionTarget;
    public bool IsInsideNotificationRange => isInNotificationRange;
    public bool IsInteractionOngoing => isInteractionOngoing;
}
