using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIButton : MonoBehaviour
{
    public void GamePlayButton()
    {
        SceneManager.LoadScene(1);  // 현재 씬 로딩
    }

    public void DeckMakerButton()
    {
        SceneManager.LoadScene(2);  // 현재 씬 로딩
    }

    public void OptionButton()
    {
        SceneManager.LoadScene(3);  // 현재 씬 로딩
    }

    public void TutorialButton()
    {
        SceneManager.LoadScene(4);  // 현재 씬 로딩
    }

    public void ExitButton()
    {
        SceneManager.LoadScene(1);  // 현재 씬 로딩
    }

    public void BackButton()
    {
        SceneManager.LoadScene(0);  // 현재 씬 로딩
    }

}
