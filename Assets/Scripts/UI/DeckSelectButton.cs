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
    int[] percentInt = new int[20];
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
            Debug.Log("Å×½ºÆ®1");

            for (int i = 0; i < 20; i++)
            {
                if (p1Percent[i] == true)
                    percentInt[i] = 1;
                else
                    percentInt[i] = 0;
            }
            
            if (isP1Deck == true)
            {
                PlayerPrefs.SetInt("p1DeckPercent0", percentInt[0]);
                PlayerPrefs.SetInt("p1DeckPercent1", percentInt[1]);
                PlayerPrefs.SetInt("p1DeckPercent2", percentInt[2]);
                PlayerPrefs.SetInt("p1DeckPercent3", percentInt[3]);
                PlayerPrefs.SetInt("p1DeckPercent4", percentInt[4]);
                PlayerPrefs.SetInt("p1DeckPercent5", percentInt[5]);
                PlayerPrefs.SetInt("p1DeckPercent6", percentInt[6]);
                PlayerPrefs.SetInt("p1DeckPercent7", percentInt[7]);
                PlayerPrefs.SetInt("p1DeckPercent8", percentInt[8]);
                PlayerPrefs.SetInt("p1DeckPercent9", percentInt[9]);
                PlayerPrefs.SetInt("p1DeckPercent10", percentInt[10]);
                PlayerPrefs.SetInt("p1DeckPercent11", percentInt[11]);
                PlayerPrefs.SetInt("p1DeckPercent12", percentInt[12]);
                PlayerPrefs.SetInt("p1DeckPercent13", percentInt[13]);
                PlayerPrefs.SetInt("p1DeckPercent14", percentInt[14]);
                PlayerPrefs.SetInt("p1DeckPercent15", percentInt[15]);
                PlayerPrefs.SetInt("p1DeckPercent16", percentInt[16]);
                PlayerPrefs.SetInt("p1DeckPercent17", percentInt[17]);
                PlayerPrefs.SetInt("p1DeckPercent18", percentInt[18]);
                PlayerPrefs.SetInt("p1DeckPercent19", percentInt[19]);
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.SetInt("p2DeckPercent0", percentInt[0]);
                PlayerPrefs.SetInt("p2DeckPercent1", percentInt[1]);
                PlayerPrefs.SetInt("p2DeckPercent2", percentInt[2]);
                PlayerPrefs.SetInt("p2DeckPercent3", percentInt[3]);
                PlayerPrefs.SetInt("p2DeckPercent4", percentInt[4]);
                PlayerPrefs.SetInt("p2DeckPercent5", percentInt[5]);
                PlayerPrefs.SetInt("p2DeckPercent6", percentInt[6]);
                PlayerPrefs.SetInt("p2DeckPercent7", percentInt[7]);
                PlayerPrefs.SetInt("p2DeckPercent8", percentInt[8]);
                PlayerPrefs.SetInt("p2DeckPercent9", percentInt[9]);
                PlayerPrefs.SetInt("p2DeckPercent10", percentInt[10]);
                PlayerPrefs.SetInt("p2DeckPercent11", percentInt[11]);
                PlayerPrefs.SetInt("p2DeckPercent12", percentInt[12]);
                PlayerPrefs.SetInt("p2DeckPercent13", percentInt[13]);
                PlayerPrefs.SetInt("p2DeckPercent14", percentInt[14]);
                PlayerPrefs.SetInt("p2DeckPercent15", percentInt[15]);
                PlayerPrefs.SetInt("p2DeckPercent16", percentInt[16]);
                PlayerPrefs.SetInt("p2DeckPercent17", percentInt[17]);
                PlayerPrefs.SetInt("p2DeckPercent18", percentInt[18]);
                PlayerPrefs.SetInt("p2DeckPercent19", percentInt[19]);
                PlayerPrefs.Save();
            }
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
