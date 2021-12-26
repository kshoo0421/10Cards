using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    [SerializeField] GameObject endTurnButton;
    public GameObject eTurn;

    public void StartGameClick()
    {
        GameManager.Inst.StartGame();
        Active(false);
        eTurn.SetActive(false);
    }

    public void Active(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

}
