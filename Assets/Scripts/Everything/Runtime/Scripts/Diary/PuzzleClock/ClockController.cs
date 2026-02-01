using Player;
using UnityEngine;
using UnityEngine.Windows;
using VInspector.Libs;

public class ClockController : MonoBehaviour
{

    public int targetMinute, targetHour;
    private int currentMinute, currentHour;
    public bool active;


    public AudioSource source;
    public AudioClip correct;

    public GameObject minute, hour;

    public GameObject baseClock;

    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minute.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -1 * (360 / 60 * currentMinute));
        hour.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -1 * (360 / 12 * currentHour + currentMinute / 2));
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {

            if (currentMinute >= 60)
            {
                currentMinute -= 60;
                currentHour++;
            }
            else if (currentMinute < 0)
            {
                currentMinute += 60;
                currentHour--;
            }
            if (currentHour > 12)
            {
                currentHour -= 12;
            }
            else if (currentHour < 1) 
            {
                currentHour += 12;
            }

            if (InputController.LeftStick.x != 0)
            {
                timer += Time.deltaTime;
                if (timer >= .5 || timer < .1)
                {
                    timer += 0.1f;
                    if (InputController.LeftStick.x < -.5)
                    {
                        currentMinute -= 5;
                        minute.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -1 * (360 / 60 * currentMinute));
                        hour.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -1 * (360 / 12 * currentHour + currentMinute / 2));
                    }
                    else if (InputController.LeftStick.x > .5)
                    {
                        currentMinute += 5;
                        minute.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -1 * (360 / 60 * currentMinute));
                        hour.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -1 * (360 / 12 * currentHour + currentMinute / 2));
                    }
                    if (timer >= 1) timer = .47f;
                }

            }
            else timer = 0;

        }
    }

    public void Open()
    {
        baseClock.SetActive(true);
        active = true;
        PlayerController.Movement.CanMove = false;
    }

    public void Close()
    {
        PlayerController.Movement.CanMove = true;
        if (currentHour == targetHour && currentMinute == targetMinute)
        {
            source.PlayOneShot(correct);
            //se desbloquea lo que sea
            Destroy(this); //para que no esté el minijuego por ahi cuando no se va a usar mas   

        }
        baseClock.SetActive(true);
        active = false;

    }

}
