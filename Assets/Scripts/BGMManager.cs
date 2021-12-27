using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioSource tryAudio;
    public AudioSource clickAudio;
    public AudioSource drawAudio;
    public AudioSource junkAudio;
    public AudioSource winAudio;
    public AudioSource loseAudio;
    public AudioSource myTurnAudio;
    public AudioSource endTurnAudio;

    public GameObject eBgmXImage;
    GameObject eBgm;
    int isEBgmOn = 1;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("EBGMOn"))
            return;

        isEBgmOn = PlayerPrefs.GetInt("EBGMOn");

        if (isEBgmOn == 1)
        {
            eBGMOn();
            eBgmXImage.SetActive(false);
            return;
        }
        else
        {
            eBGMOff();
            eBgmXImage.SetActive(true);
        }
    }

    public void BGMButton()
    {
        if (isEBgmOn == 1)
        {
            eBGMOff();
            eBgmXImage.SetActive(true);
            isEBgmOn = 0;
            PlayerPrefs.SetInt("EBGMOn", 0);
            PlayerPrefs.Save();
        }
        else
        {
            eBGMOn();
            eBgmXImage.SetActive(false);
            isEBgmOn = 1;
            PlayerPrefs.SetInt("EBGMOn", 1);
            PlayerPrefs.Save();
        }
    }

    void eBGMOn()
    {
        tryAudio.volume = 1.0f;
        clickAudio.volume = 1.0f;
        drawAudio.volume = 1.0f;
        junkAudio.volume = 1.0f;
        winAudio.volume = 1.0f;
        loseAudio.volume = 1.0f;
        myTurnAudio.volume = 1.0f;
        endTurnAudio.volume = 1.0f;
    }

    void eBGMOff()
    {
        Debug.Log("ebgm off");
        tryAudio.volume = 0f ;
        clickAudio.volume = 0f ;
        drawAudio.volume = 0f;
        junkAudio.volume = 0f;
        winAudio.volume = 0f;
        loseAudio.volume = 0f;
        myTurnAudio.volume = 0f;
        endTurnAudio.volume = 0f;
    }
}
