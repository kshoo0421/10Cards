using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIButton : MonoBehaviour
{
    [SerializeField] GameObject exitMenu;
    [SerializeField] GameObject DeckMenu;
    public GameObject deckPercent;
    public GameObject bgm;

    private void Start()
    {
        bgm.SetActive(true);
    }

    public void GamePlayButton()
    {
        SceneManager.LoadScene(2);
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
        DontDestroyOnLoad(bgm);
    }

    public void P2DeckMaker()
    {
        deckPercent = GameObject.Find("DeckPercent");
        deckPercent.GetComponent<DeckPercent>().p1Deck = false;
        SceneManager.LoadScene(4);
        DontDestroyOnLoad(deckPercent);
        DontDestroyOnLoad(bgm);
    }

    public void ExitDeck()
    {
        DeckMenu.SetActive(false);
    }

    public void OptionButton()
    {
        SceneManager.LoadScene(5);
        DontDestroyOnLoad(deckPercent);
        DontDestroyOnLoad(bgm);
    }

    public void TutorialButton()
    {
        SceneManager.LoadScene(6);
        DontDestroyOnLoad(deckPercent);
        DontDestroyOnLoad(bgm);
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
