using Player;
using UnityEngine;
using UnityEngine.UI;

public class PadLockDiary : MonoBehaviour
{
    
    public static bool PadLockDiaryUnlocked = false;

    public GameObject padLock, diary;

    public int password;



    public Sprite[] pages;
    public int currentPage;
    public GameObject next, prev;


    private void Start()
    {
        CheckPages();
            padLock.GetComponent<PadLock>().password = password;
        Close();
    }

    public void Open()
    {
        PlayerController.Movement.CanMove = false;
        if (!PadLockDiaryUnlocked)
        {
            padLock.SetActive(true);
        }
        else diary.SetActive(true);
    }

    public void ReOpen()
    {
        Close();
        Open();
    }

    public void CheckPages()
    {
        if (currentPage <= 0) prev.SetActive(false);
        else prev.SetActive(true);
        if (currentPage >= pages.Length) next.SetActive(false);
        else next.SetActive(true);
    }

    public void Close()
    {
        PlayerController.Movement.CanMove = true;
        padLock.SetActive(false);
        diary.SetActive(false);
    }

    public void TurnPage(bool nextPage)
    {
        if (nextPage) currentPage++;
        else currentPage--;
        diary.GetComponent<Image>().sprite = pages[currentPage];
            CheckPages();
    }

}
