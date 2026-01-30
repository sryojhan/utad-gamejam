using Febucci.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    public TextMeshProUGUI speaker;
    public TextMeshProUGUI text;
    public TextAnimatorPlayer textPlayer;

    private Dialogue currentDialogue;

    private bool inDialogue;
    public bool IsInDialogue => inDialogue;

    public Dialogue debugDialogue;

    private void Awake()
    {
        EnsureInitialised();
    }
    private void Start()
    {
        textPlayer.onTextShowed.AddListener(TextEnd);
        text.text = "";
        gameObject.SetActive(false);

        //ShowDialogue(debugDialogue);
    }

    public void BeginDialogue(Dialogue dialogue)
    {
        if (inDialogue) return;

        inDialogue = true;
        gameObject.SetActive(true);
        text.text = "";

        StartCoroutine(RevealAnimation(() => { ShowDialogue(dialogue); }));
    }

    IEnumerator RevealAnimation(Action begin)
    {
        yield return null;
        begin();
    }

    private void ShowDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;

        speaker.text = dialogue.speaker;
        textPlayer.ShowText(currentDialogue.message);
    }

    private void TextEnd()
    {
        if (currentDialogue == null)
            throw new NullReferenceException("current dialogue is null");

        Dialogue next = currentDialogue.next;

        StartCoroutine(WaitForNext(next));
    }

    IEnumerator WaitForNext(Dialogue next)
    {
        yield return new WaitUntil(InputController.South.IsJustPressed);

        if (next != null)
        {
            ShowDialogue(next);
        }
        else
        {
            currentDialogue = null;
            inDialogue = false;
            text.text = "";
            gameObject.SetActive(false);
        }
    }
}
