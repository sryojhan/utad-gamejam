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

    public GameObject textContainer;
    public GameObject selectionContainer;

    private bool waitingForSelection;
    private void Awake()
    {
        EnsureInitialised();
    }
    private void Start()
    {
        textPlayer.onTextShowed.AddListener(TextEnd);
        text.text = "";
        waitingForSelection = false;
        gameObject.SetActive(false);

        //ShowDialogue(debugDialogue);
    }

    public void BeginDialogue(Dialogue dialogue)
    {
        if (inDialogue) return;

        inDialogue = true;
        gameObject.SetActive(true);
        text.text = "";
        speaker.text = "";

        textContainer.SetActive(true);
        selectionContainer.SetActive(false);

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

        textContainer.SetActive(true);
        selectionContainer.SetActive(false);

        speaker.text = dialogue.speaker;
        textPlayer.ShowText(currentDialogue.message);
    }

    private void TextEnd()
    {
        if (currentDialogue == null)
            throw new NullReferenceException("current dialogue is null");

        if (currentDialogue.hasOptions)
        {
            StartCoroutine(ActivateOptions(currentDialogue.options));
            return;
        }

        Dialogue next = currentDialogue.next;
        StartCoroutine(WaitForNext(next, true));
    }

    IEnumerator WaitForNext(Dialogue next, bool waitInput)
    {
        if (waitInput)
        {
            yield return new WaitUntil(InputController.South.IsJustPressed);
            InputController.South.ConsumeCache();
        }

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


    IEnumerator ActivateOptions(Dialogue.DialogueOption[] options)
    {
        yield return new WaitUntil(InputController.South.IsJustPressed);
        InputController.South.ConsumeCache();
        
        selectionContainer.SetActive(true);
        textContainer.SetActive(false);

        Transform contentChildLayout = selectionContainer.transform.GetChild(0);
        foreach (Transform tr in contentChildLayout)
        {
            tr.gameObject.SetActive(false);
        }


        for (int i = 0; i < options.Length; i++)
        {
            contentChildLayout.GetChild(i).GetComponent<DialogueSelection>().Initialise(options[i], this);
        }

        waitingForSelection = true;
    }


    public void SelectionChosen(Dialogue next)
    {
        waitingForSelection = false;
        StartCoroutine(WaitForNext(next, false));
    }
}
