using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMOnOff : MonoBehaviour
{
    GameObject bgm;
    bool isOn = true;
    public GameObject XImage;

    void BGMBtn()
    {
        if (isOn == true)
        {
            bgm = GameObject.Find("BGM");
            bgm.SetActive(false);
            XImage.SetActive(true);
            isOn = false;

        }
        else
        {
            GameObject.Find("BGMSet").transform.Find("BGM").gameObject.SetActive(true);
            XImage.SetActive(false);
            isOn = true;
        }

    }
}
