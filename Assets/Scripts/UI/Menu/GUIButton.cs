using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIButton : MonoBehaviour
{
    [SerializeField] GameObject exitMenu;
    [SerializeField] GameObject DeckMenu;
    [SerializeField] GameObject ExtraMenu;
    public GameObject deckPercent;
    GameObject bgm;

    public void GamePlayButton()
    {
        SceneManager.LoadScene(2);
        bgm = GameObject.Find("BGM");
        bgm.SetActive(false);
        DontDestroyOnLoad(deckPercent);
    }

    public void DeckMakerButton()
    {
        DeckMenu.SetActive(true);
    }

    public void P1DeckMaker()
    {
        deckPercent = GameObject.Find("DeckPercent");
        deckPercent.GetComponent<DeckPercent>().p1Deck = true;
        SceneManager.LoadScene(3);
        DontDestroyOnLoad(deckPercent);
    }

    public void P2DeckMaker()
    {
        deckPercent = GameObject.Find("DeckPercent");
        deckPercent.GetComponent<DeckPercent>().p1Deck = false;
        SceneManager.LoadScene(4);
        DontDestroyOnLoad(deckPercent);
    }

    public void ExtraBtnOn()
    {
        ExtraMenu.SetActive(true);
    }

    public void ExtraBtnOff()
    {
        ExtraMenu.SetActive(false);
    }


    public void ExitDeck()
    {
        DeckMenu.SetActive(false);
    }

    public void TutorialButton()
    {
        SceneManager.LoadScene(5);
        DontDestroyOnLoad(deckPercent);
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
        SceneManager.LoadScene(1);
    }
}
