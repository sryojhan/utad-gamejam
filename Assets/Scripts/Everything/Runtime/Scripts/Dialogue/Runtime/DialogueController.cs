using UnityEngine;

namespace Dialogue
{
      
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] TextRef textRef;

        [SerializeField] DialogueGraph dialogue;
        [SerializeField] DialogueControllerConfiguration configuration;

        private void Start()
        {
            InitialiseDialogue();
        }

        [Button]
        void InitialiseDialogue()
        {
            if (!Application.isPlaying) return;

            configuration.Apply(textRef, dialogue.Begin().GetMessage());

        }


    }
}
