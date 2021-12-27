using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckPercent : MonoBehaviour
{
    public static DeckPercent Inst { get; private set; }
    private void Awake() => Inst = this;

    public bool p1Deck = true;
    public bool[] p1Percent;
    public bool[] p2Percent;
    int[] p1Int = new int[20];
    int[] p2Int = new int[20];

    public void DeckUpdate()
    {
        Debug.Log("Deck Update");
        Debug.Log("p1 2percent : " + PlayerPrefs.GetInt("p1DeckPercent2"));
        Debug.Log("p1 3percent : " + PlayerPrefs.GetInt("p1DeckPercent13"));
        Debug.Log("p1 12percent : " + PlayerPrefs.GetInt("p1DeckPercent12"));


        if (!PlayerPrefs.HasKey("p1DeckPercent0"))
        {
            if (!PlayerPrefs.HasKey("p2DeckPercent0"))
                return;
            else
            {
                p2Int[0] = PlayerPrefs.GetInt("p2DeckPercent0");
                p2Int[1] = PlayerPrefs.GetInt("p2DeckPercent1");
                p2Int[2] = PlayerPrefs.GetInt("p2DeckPercent2");
                p2Int[3] = PlayerPrefs.GetInt("p2DeckPercent3");
                p2Int[4] = PlayerPrefs.GetInt("p2DeckPercent4");
                p2Int[5] = PlayerPrefs.GetInt("p2DeckPercent5");
                p2Int[6] = PlayerPrefs.GetInt("p2DeckPercent6");
                p2Int[7] = PlayerPrefs.GetInt("p2DeckPercent7");
                p2Int[8] = PlayerPrefs.GetInt("p2DeckPercent8");
                p2Int[9] = PlayerPrefs.GetInt("p2DeckPercent9");
                p2Int[10] = PlayerPrefs.GetInt("p2DeckPercent10");
                p2Int[11] = PlayerPrefs.GetInt("p2DeckPercent11");
                p2Int[12] = PlayerPrefs.GetInt("p2DeckPercent12");
                p2Int[13] = PlayerPrefs.GetInt("p2DeckPercent13");
                p2Int[14] = PlayerPrefs.GetInt("p2DeckPercent14");
                p2Int[15] = PlayerPrefs.GetInt("p2DeckPercent15");
                p2Int[16] = PlayerPrefs.GetInt("p2DeckPercent16");
                p2Int[17] = PlayerPrefs.GetInt("p2DeckPercent17");
                p2Int[18] = PlayerPrefs.GetInt("p2DeckPercent18");
                p2Int[19] = PlayerPrefs.GetInt("p2DeckPercent19");

                for (int i = 0; i < 20; i++)
                {
                    if (p2Int[i] == 0)
                        p2Percent[i] = false;
                    else
                        p2Percent[i] = true;
                }
                return;
            }
        }
        p1Int[0] = PlayerPrefs.GetInt("p1DeckPercent0");
        p1Int[1] = PlayerPrefs.GetInt("p1DeckPercent1");
        p1Int[2] = PlayerPrefs.GetInt("p1DeckPercent2");
        p1Int[3] = PlayerPrefs.GetInt("p1DeckPercent3");
        p1Int[4] = PlayerPrefs.GetInt("p1DeckPercent4");
        p1Int[5] = PlayerPrefs.GetInt("p1DeckPercent5");
        p1Int[6] = PlayerPrefs.GetInt("p1DeckPercent6");
        p1Int[7] = PlayerPrefs.GetInt("p1DeckPercent7");
        p1Int[8] = PlayerPrefs.GetInt("p1DeckPercent8");
        p1Int[9] = PlayerPrefs.GetInt("p1DeckPercent9");
        p1Int[10] = PlayerPrefs.GetInt("p1DeckPercent10");
        p1Int[11] = PlayerPrefs.GetInt("p1DeckPercent11");
        p1Int[12] = PlayerPrefs.GetInt("p1DeckPercent12");
        p1Int[13] = PlayerPrefs.GetInt("p1DeckPercent13");
        p1Int[14] = PlayerPrefs.GetInt("p1DeckPercent14");
        p1Int[15] = PlayerPrefs.GetInt("p1DeckPercent15");
        p1Int[16] = PlayerPrefs.GetInt("p1DeckPercent16");
        p1Int[17] = PlayerPrefs.GetInt("p1DeckPercent17");
        p1Int[18] = PlayerPrefs.GetInt("p1DeckPercent18");
        p1Int[19] = PlayerPrefs.GetInt("p1DeckPercent19");
        for (int i = 0; i < 20; i++)
        {
            if (p1Int[i] == 0)
                p1Percent[i] = false;
            else
                p1Percent[i] = true;
        }


        if (!PlayerPrefs.HasKey("p2DeckPercent0"))
            return;

        p2Int[0] = PlayerPrefs.GetInt("p2DeckPercent0");
        p2Int[1] = PlayerPrefs.GetInt("p2DeckPercent1");
        p2Int[2] = PlayerPrefs.GetInt("p2DeckPercent2");
        p2Int[3] = PlayerPrefs.GetInt("p2DeckPercent3");
        p2Int[4] = PlayerPrefs.GetInt("p2DeckPercent4");
        p2Int[5] = PlayerPrefs.GetInt("p2DeckPercent5");
        p2Int[6] = PlayerPrefs.GetInt("p2DeckPercent6");
        p2Int[7] = PlayerPrefs.GetInt("p2DeckPercent7");
        p2Int[8] = PlayerPrefs.GetInt("p2DeckPercent8");
        p2Int[9] = PlayerPrefs.GetInt("p2DeckPercent9");
        p2Int[10] = PlayerPrefs.GetInt("p2DeckPercent10");
        p2Int[11] = PlayerPrefs.GetInt("p2DeckPercent11");
        p2Int[12] = PlayerPrefs.GetInt("p2DeckPercent12");
        p2Int[13] = PlayerPrefs.GetInt("p2DeckPercent13");
        p2Int[14] = PlayerPrefs.GetInt("p2DeckPercent14");
        p2Int[15] = PlayerPrefs.GetInt("p2DeckPercent15");
        p2Int[16] = PlayerPrefs.GetInt("p2DeckPercent16");
        p2Int[17] = PlayerPrefs.GetInt("p2DeckPercent17");
        p2Int[18] = PlayerPrefs.GetInt("p2DeckPercent18");
        p2Int[19] = PlayerPrefs.GetInt("p2DeckPercent19");

        for (int i = 0; i < 20; i++)
        {
            if (p2Int[i] == 0)
                p2Percent[i] = false;
            else
                p2Percent[i] = true;
        }
        return;
    }
}