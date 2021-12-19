using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuBackButton : MonoBehaviour
{
    [SerializeField] GameObject menuBackButton;

    public void MenuBackButtonOn()
    {
        menuBackButton.SetActive(true);
    }

    public void MenuBackButtonYes()
    {
        SceneManager.LoadScene(0);
    }

    public void MenuBackButtonNo()
    {
        menuBackButton.SetActive(false);
    }


}
