using Febucci.UI.Core;
using System;
using TMPro;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/Configuration", fileName = "New configuration")]
    public class DialogueControllerConfiguration : ScriptableObject
    {
        [Header("Style")]

        [OverrideGUI] public Override<Color> color;
        [OverrideGUI] public Override<Color> outlineColor;

        [Header("Animation")]

        [OverrideGUI] public Override<float> textRevealSpeed;
        [OverrideGUI] public Override<TextRevealStyle> revealStyle;

        [Serializable]
        public class TextRevealStyle
        {
            //TODO: hacer otra clase para gestionar esto guay
            public enum Mode
            {
                Size, Horizontal, Vertical, Diagonal, Rotating, Fading, Offset, Random, Custom, None
            }

            public Mode mode;
            public string customMode;

            [OverrideGUI] public Override<float> mod_duration = new(1);
            [OverrideGUI] public Override<float> mod_amplitude = new(1);
            [OverrideGUI] public Override<bool> mod_bottom;

            public enum HorizontalDirection
            {
                Left, Middle, Right
            }

            [OverrideGUI] public Override<HorizontalDirection> mod_horizontalDirection;
        }


        public class TextBehaviourStyle
        {
            public enum Mode
            {
                None, Pendulum, Dangle, Fade, Rainbow, Rotating, Bounce, Sliding, Swinging, Waving, IncreasingSize, Shake, Wiggle
            }
        }


        [OverrideGUI] public Override<BuiltinAppearancesDataScriptable> textAnimatorReveal;
        [OverrideGUI] public Override<BuiltinBehaviorsDataScriptable> textAnimatorBehaviour;


        public void Apply(TextRef textRef, string text)
        {
            color.Apply((col) => textRef.TMP.color = col);
            textRevealSpeed.Apply((speed) => textRef.AnimatorPlayer.SetTypewriterSpeed(speed));

            textAnimatorReveal.Apply((val) => textRef.Animator.AssignSharedAppearancesData(val));
            textAnimatorBehaviour.Apply((val) => textRef.Animator.AssignSharedBehaviorsData(val));

            revealStyle.Apply((style) =>
            {
                string tag = style.mode switch
                {
                    TextRevealStyle.Mode.Size => "size",
                    TextRevealStyle.Mode.Random => "rdir",
                    TextRevealStyle.Mode.Horizontal => "horiexp",
                    TextRevealStyle.Mode.Vertical => "vertexp",
                    TextRevealStyle.Mode.Diagonal => "diagexp",
                    TextRevealStyle.Mode.Rotating => "rot",
                    TextRevealStyle.Mode.Fading => "fade",
                    TextRevealStyle.Mode.Offset => "offset",
                    TextRevealStyle.Mode.Custom => style.customMode,
                    TextRevealStyle.Mode.None => "",
                    _ => "",
                };

                string modifiers = "";

                style.mod_amplitude.Apply((value) =>
                {
                    modifiers += $" a={value}";
                });

                style.mod_duration.Apply((value) =>
                {
                    modifiers += $" d={value}";
                });

                style.mod_bottom.Apply((value) =>
                {
                    modifiers += $" bot={(value ? 1 : 0)}";
                });

                style.mod_horizontalDirection.Apply((value) =>
                {
                    int intVal = value == TextRevealStyle.HorizontalDirection.Left ? -1 :
                    value == TextRevealStyle.HorizontalDirection.Middle ? 0 : 1;

                    modifiers += $" x={intVal}";
                });



                if (!string.IsNullOrWhiteSpace(tag))
                {
                    text = $"{{{tag}{modifiers}}}{text}";
                }
            });



            textRef.Animator.SetText(text, true);
            textRef.AnimatorPlayer.StartShowingText();

        }

    }
}
