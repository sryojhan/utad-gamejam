using UnityEngine;


[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public string speaker = "Detective";
    public string message = "Hola buenas";

    public Sprite icon;
    public bool isLeft;

    public string activateEvent = "";

    public bool hasOptions;
    public Dialogue[] options;

    public Dialogue next;
}
