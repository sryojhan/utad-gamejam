using System.Collections;
using TMPro;
using UnityEngine;

public class PadLock : MonoBehaviour
{

    public TextMeshProUGUI display;
    public int currentNumber;
    public int numbers;
    public int password;

    void Start()
    {
        
    }

    public void InsertNumber(int number)
    {
        currentNumber = currentNumber * 10 + number;
        numbers++;
        if (numbers >= 4) StartCoroutine(CheckAnswer());
    }

    public void Success()
    {

    }

    public IEnumerator CheckAnswer()
    {
        yield return new WaitForSeconds(1);

        if(currentNumber == password)
        {
            display.text = "Correct!";
            display.color = Color.green;
            Success();
        }
        else
        {
            display.text = "Incorrect";
            display.color = Color.red;
            currentNumber = 0;
            numbers = 0;

            yield return new WaitForSeconds(1);

            display.text = "";

        }



    }

}
