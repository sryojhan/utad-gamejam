using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSelection : MonoBehaviour
{
    public Color backgroundWhenUnlocked = Color.yellow;

    TextMeshProUGUI text;
    Image image;
    Button button;

    private Color defaultColor;

    bool initialisedReferences;
    private void Start()
    {
        if (initialisedReferences) return;

        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        defaultColor = image.color;

        initialisedReferences = true;
    }

    public void Initialise(Dialogue.DialogueOption data, DialogueController controller)
    {
        Start();

        bool wasLocked = false;

        if (!string.IsNullOrEmpty(data.requisite))
        {
            bool ret = Progress.IsUnlocked(data.requisite);

            if (!ret)
            {
                gameObject.SetActive(false);
                return;
            }
            wasLocked = true;
        }

        gameObject.SetActive(true);

        image.color = wasLocked ? backgroundWhenUnlocked : defaultColor;
        text.text = data.displayText;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {  controller.SelectionChosen(data.next); });
    }
}
