using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIButton : MonoBehaviour
{
    public void GamePlayButton()
    {
        SceneManager.LoadScene(1);  // ���� �� �ε�
    }

    public void DeckMakerButton()
    {
        SceneManager.LoadScene(2);  // ���� �� �ε�
    }

    public void OptionButton()
    {
        SceneManager.LoadScene(3);  // ���� �� �ε�
    }

    public void TutorialButton()
    {
        SceneManager.LoadScene(4);  // ���� �� �ε�
    }

    public void ExitButton()
    {
        SceneManager.LoadScene(1);  // ���� �� �ε�
    }

    public void BackButton()
    {
        SceneManager.LoadScene(0);  // ���� �� �ε�
    }

}
