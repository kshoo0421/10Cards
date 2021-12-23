using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstMenu : MonoBehaviour
{
    public GameObject bgm;

    public void GameStart()
    {
        SceneManager.LoadScene(1);
        DontDestroyOnLoad(bgm);
    }
}