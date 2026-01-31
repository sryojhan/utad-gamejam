using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        Interactable target;
        IInteraction onGoingInteraction;
        readonly HashSet<Interactable> interactables = new();


        public void RegisterInteractable(Interactable interactable)
        {
            if (interactables.Contains(interactable))
                return;

            interactable.EnterNotificationRange();
            interactables.Add(interactable);
        }

        public void UnregisterInteractable(Interactable interactable)
        {
            if (!interactables.Contains(interactable))
                return;

            interactable.ExitNotificationRange();
            interactables.Remove(interactable);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (transform.IsChildOf(other.transform)) return;


            Interactable interactable = other.GetComponentInChildren<Interactable>();

            if (interactable) RegisterInteractable(interactable);
        }

        private void OnTriggerExit(Collider other)
        {
            Interactable interactable = other.GetComponentInChildren<Interactable>();

            if (interactable) UnregisterInteractable(interactable);
        }

        private void Update()
        {
            if (ManageCurrentInteraction()) return;

            Interactable newTarget = Nearest();

            if (newTarget != target)
            {
                if (target)
                {
                    target.RemoveAsInteractionTarget();
                }

                if (newTarget)
                {
                    newTarget.SetAsInteractionTarget();
                }

                target = newTarget;
            }

            if (target && InputController.South.IsPressed())
            {
                onGoingInteraction = target.BeginInteraction();

                if(onGoingInteraction != null)
                {
                    onGoingInteraction.Begin();
                }
                else
                {
                    target.EndInteraction();
                }
            }
        }


        bool ManageCurrentInteraction()
        {
            if (onGoingInteraction == null) return false;

            if (onGoingInteraction.IsDone())
            {
                target.EndInteraction();

                //target = null;
                onGoingInteraction = null;
            }

            return true;
        }


        Interactable Nearest()
        {
            if (interactables.Count == 0) return null;

            float closestDistance = float.PositiveInfinity;
            Interactable closest = null;

            foreach (Interactable interactable in interactables)
            {
                float distance = (interactable.transform.position - transform.position).sqrMagnitude;

                if (distance > interactable.InteractionRangeSqred)
                    continue;

                if (distance < closestDistance)
                {
                    closest = interactable;
                }
            }

            return closest;
        }

    }
}
