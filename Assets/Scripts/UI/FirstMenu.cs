using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstMenu : MonoBehaviour
{
    public GameObject bgmSet;
    public GameObject deckPercent;
    public GameObject bgm;
    int isBgmOn;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("BGMOn"))
            return;

        isBgmOn = PlayerPrefs.GetInt("BGMOn");

        if (isBgmOn == 1)
            return;
        else
            bgm.SetActive(false);
    }

    public void GameStart()
    {
        SceneManager.LoadScene(1);
        DontDestroyOnLoad(bgmSet);
        DontDestroyOnLoad(deckPercent);
    }
}