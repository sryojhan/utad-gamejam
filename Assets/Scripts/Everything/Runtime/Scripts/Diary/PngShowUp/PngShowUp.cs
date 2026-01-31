using UnityEngine;
using UnityEngine.UI;

public class PngShowUp : MonoBehaviour
{
    public GameObject instance;
    private GameObject temp;
    public Sprite testImg;

    public void Show(Sprite img)
    {
        if(temp != null) Destroy(temp);
        temp = Instantiate(instance, this.transform);
        temp.GetComponent<Image>().sprite = img;
    }

    // Update is called once per frame
    void Update()
    {
       if (InputController.West.IsJustPressed())
        {
            Destroy(temp);
        }
    }
}
