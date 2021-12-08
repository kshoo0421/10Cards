using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    [SerializeField] GameObject endTurnButton;

    public void StartGameClick()
    {
        GameManager.Inst.StartGame();
        Active(false);
        // ActiveEndButton(true);
    }

    /*
    public void ActiveEndButton(bool eb)
    {
        endTurnButton.SetActive(eb);
    }
    */

    public void Active(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

}
