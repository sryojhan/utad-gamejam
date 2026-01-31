using Febucci.UI;
using System.Collections;
using UnityEngine;

public class InitialSequence : MonoBehaviour
{
    public string[] sequence;
    public TextAnimatorPlayer textPlayer;

    IEnumerator Start()
    {
        foreach(string str in sequence)
        {
            textPlayer.ShowText(str);
            yield return new WaitUntil(InputController.South.IsJustPressed);
        }
    }


    public void End()
    {

    }
}
