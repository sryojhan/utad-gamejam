using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/Actor", fileName = "New actor")]
    public class DialogueActor : ScriptableObject
    {
        public new string name;

        public Sprite defaultSprite;
        public SpriteBank reactions;
    }
}
