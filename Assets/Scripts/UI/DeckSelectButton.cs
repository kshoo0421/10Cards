using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckSelectButton : MonoBehaviour
{
    public static DeckSelectButton Inst { get; private set; }
    private void Awake() => Inst = this;

    [SerializeField] GameObject exitMenu;
    [SerializeField] GameObject errorMenu;
    [SerializeField] GameObject page1;
    [SerializeField] GameObject page2;
    [SerializeField] GameObject page1Button;
    [SerializeField] GameObject page2Button;
    [SerializeField] TMP_Text countTMP;
    GameObject deckPercent;

    public int selectCardNumber;
    bool isP1Deck;
    public bool[] p1Percent;
    int totalCardCount = 10;

    private void Start()
    {
        deckPercent = GameObject.Find("DeckPercent");
        isP1Deck = deckPercent.GetComponent<DeckPercent>().p1Deck;
        
        p1Percent = (isP1Deck ? deckPercent.GetComponent<DeckPercent>().p1Percent : deckPercent.GetComponent<DeckPercent>().p2Percent);
        DeckMaker.Inst.OXUpdate();
        countTMP.text = totalCardCount.ToString();
    }

    public void SelectButton()
    {
        selectCardNumber = DeckMaker.Inst.cardNumber;

        if (p1Percent[selectCardNumber] == false)
        {
            p1Percent[selectCardNumber] = true;
            totalCardCount++;
        }
        else
        {
            p1Percent[selectCardNumber] = false;
            totalCardCount--;
        }
        DeckMaker.Inst.OXUpdate();
        countTMP.text = totalCardCount.ToString();
    }

    public void ExitButton()
    {
        exitMenu.SetActive(true);
    }

    public void ExitButtonYes()
    {
        if (totalCardCount == 10)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            errorMenu.SetActive(true);
        }
    }

    public void menuBack()
    {
        exitMenu.SetActive(false);
        errorMenu.SetActive(false);
    }

    public void nextPage()
    {
        page1.SetActive(false);
        page2.SetActive(true);
        page1Button.SetActive(true);
        page2Button.SetActive(false);
    }

    public void backPage()
    {
        page1.SetActive(true);
        page2.SetActive(false);
        page1Button.SetActive(false);
        page2Button.SetActive(true);
    }
}
