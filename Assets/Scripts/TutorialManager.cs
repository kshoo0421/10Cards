using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;
    public GameObject page5;
    public GameObject page6;
    public GameObject page7;
    public GameObject page8;
    public GameObject page9;
    public GameObject page10;
    public GameObject page11;
    public GameObject page12;
    public GameObject page13;
    public GameObject page14;
    public GameObject page15;
    [SerializeField] TMP_Text currentTMP;
    int current = 1;

    public void nb1()
    {
        page1.SetActive(false);
        page2.SetActive(true);
        current = 2;
        currentTMP.text = current.ToString();
    }

    public void nb2()
    {
        page2.SetActive(false);
        page3.SetActive(true);
        current = 3;
        currentTMP.text = current.ToString();
    }

    public void nb3()
    {
        page3.SetActive(false);
        page4.SetActive(true);
        current = 4;
        currentTMP.text = current.ToString();
    }

    public void nb4()
    {
        page4.SetActive(false);
        page5.SetActive(true);
        current = 5;
        currentTMP.text = current.ToString();
    }

    public void nb5()
    {
        page5.SetActive(false);
        page6.SetActive(true);
        current = 6;
        currentTMP.text = current.ToString();
    }

    public void nb6()
    {
        page6.SetActive(false);
        page7.SetActive(true);
        current = 7;
        currentTMP.text = current.ToString();
    }

    public void nb7()
    {
        page7.SetActive(false);
        page8.SetActive(true);
        current = 8;
        currentTMP.text = current.ToString();
    }

    public void nb8()
    {
        page8.SetActive(false);
        page9.SetActive(true);
        current = 9;
        currentTMP.text = current.ToString();
    }

    public void nb9()
    {
        page9.SetActive(false);
        page10.SetActive(true);
        current = 10;
        currentTMP.text = current.ToString();
    }

    public void nb10()
    {
        page10.SetActive(false);
        page11.SetActive(true);
        current = 11;
        currentTMP.text = current.ToString();
    }

    public void nb11()
    {
        page11.SetActive(false);
        page12.SetActive(true);
        current = 12;
        currentTMP.text = current.ToString();
    }

    public void nb12()
    {
        page12.SetActive(false);
        page13.SetActive(true);
        current = 13;
        currentTMP.text = current.ToString();
    }

    public void nb13()
    {
        page13.SetActive(false);
        page14.SetActive(true);
        current = 14;
        currentTMP.text = current.ToString();
    }

    public void nb14()
    {
        page14.SetActive(false);
        page15.SetActive(true);
        current = 15;
        currentTMP.text = current.ToString();
    }

    public void bb2()
    {
        page2.SetActive(false);
        page1.SetActive(true);
        current = 1;
        currentTMP.text = current.ToString();
    }

    public void bb3()
    {
        page3.SetActive(false);
        page2.SetActive(true);
        current = 2;
        currentTMP.text = current.ToString();
    }

    public void bb4()
    {
        page4.SetActive(false);
        page3.SetActive(true);
        current = 3;
        currentTMP.text = current.ToString();
    }

    public void bb5()
    {
        page5.SetActive(false);
        page4.SetActive(true);
        current = 4;
        currentTMP.text = current.ToString();
    }

    public void bb6()
    {
        page6.SetActive(false);
        page5.SetActive(true);
        current = 5;
        currentTMP.text = current.ToString();
    }

    public void bb7()
    {
        page7.SetActive(false);
        page6.SetActive(true);
        current = 6;
        currentTMP.text = current.ToString();
    }

    public void bb8()
    {
        page8.SetActive(false);
        page7.SetActive(true);
        current = 7;
        currentTMP.text = current.ToString();
    }

    public void bb9()
    {
        page9.SetActive(false);
        page8.SetActive(true);
        current = 8;
        currentTMP.text = current.ToString();
    }

    public void bb10()
    {
        page10.SetActive(false);
        page9.SetActive(true);
        current = 9;
        currentTMP.text = current.ToString();
    }

    public void bb11()
    {
        page11.SetActive(false);
        page10.SetActive(true);
        current = 10;
        currentTMP.text = current.ToString();
    }

    public void bb12()
    {
        page12.SetActive(false);
        page11.SetActive(true);
        current = 11;
        currentTMP.text = current.ToString();
    }

    public void bb13()
    {
        page13.SetActive(false);
        page12.SetActive(true);
        current = 12;
        currentTMP.text = current.ToString();
    }

    public void bb14()
    {
        page14.SetActive(false);
        page13.SetActive(true);
        current = 13;
        currentTMP.text = current.ToString();
    }

    public void bb15()
    {
        page15.SetActive(false);
        page14.SetActive(true);
        current = 14;
        currentTMP.text = current.ToString();
    }
}
