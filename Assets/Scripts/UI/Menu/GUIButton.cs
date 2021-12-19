using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIButton : MonoBehaviour
{
    [SerializeField] GameObject exitMenu;

    public void GamePlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void DeckMakerButton()
    {
        SceneManager.LoadScene(2); 
    }

    public void OptionButton()
    {
        SceneManager.LoadScene(3); 
    }

    public void TutorialButton()
    {
        SceneManager.LoadScene(4); 
    }

    public void ExitButton()
    {
        exitMenu.SetActive(true);
    }

    public void ExitButtonNo()
    {
        exitMenu.SetActive(false);
    }

    public void ExitButtonYes()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        SceneManager.LoadScene(0);
    }

}
