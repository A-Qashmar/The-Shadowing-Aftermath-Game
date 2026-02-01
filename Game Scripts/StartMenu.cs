using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public GameObject controlsTextObject;  

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

 public void ShowControls()
{
    controlsTextObject.SetActive(true);

}

}