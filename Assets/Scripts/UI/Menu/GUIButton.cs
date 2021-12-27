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
    public GameObject bgmXImage;
    GameObject bgm;
    int isBgmOn = 1;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("BGMOn"))
            return;

        isBgmOn = PlayerPrefs.GetInt("BGMOn");
        bgm = GameObject.Find("BGM");

        if (isBgmOn == 1)
        {
            bgm.SetActive(true);
            bgmXImage.SetActive(false);
            return;
        }
        else
        {
            bgm.SetActive(false);
            bgmXImage.SetActive(true);
        }
    }

    public void BGMButton()
    {
        if(isBgmOn == 1)
        {
            bgm = GameObject.Find("BGM");
            bgm.SetActive(false);
            bgmXImage.SetActive(true);
            isBgmOn = 0;
            PlayerPrefs.SetInt("BGMOn", 0);
            PlayerPrefs.Save();
        }
        else
        {
            GameObject.Find("BGMSet").transform.Find("BGM").gameObject.SetActive(true);
            bgmXImage.SetActive(false);
            isBgmOn = 1;
            PlayerPrefs.SetInt("BGMOn", 1);
            PlayerPrefs.Save();
        }
    }

    public void GamePlayButton()
    {
        SceneManager.LoadScene(2);
        bgm = GameObject.Find("BGM");
        bgm.SetActive(false);
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
    }

    public void P2DeckMaker()
    {
        deckPercent = GameObject.Find("DeckPercent");
        deckPercent.GetComponent<DeckPercent>().p1Deck = false;
        SceneManager.LoadScene(4);
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
