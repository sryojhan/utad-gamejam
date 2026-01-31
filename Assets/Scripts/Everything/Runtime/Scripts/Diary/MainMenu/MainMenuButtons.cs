using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuButtons : MonoBehaviour
{


    
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Credits()
    {

    }

    public void Exit()
    {
        Application.Quit();
    }

}
