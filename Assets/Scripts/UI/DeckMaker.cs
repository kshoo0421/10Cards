using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckMaker : MonoBehaviour
{
    public static DeckMaker Inst { get; private set; }
    private void Awake() => Inst = this;

    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject cardPrefab;
    public Image cardOX0;
    public Image cardOX1;
    public Image cardOX2;
    public Image cardOX3;
    public Image cardOX4;
    public Image cardOX5;
    public Image cardOX6;
    public Image cardOX7;
    public Image cardOX8;
    public Image cardOX9;
    public Image cardOX10;
    public Image cardOX11;
    public Image cardOX12;
    public Image cardOX13;
    public Image cardOX14;
    public Image cardOX15;
    public Image cardOX16;
    public Image cardOX17;
    public Image cardOX18;
    public Image cardOX19;
    public Sprite OImage;
    public Sprite XImage;
    public int cardNumber;

    public void TouchCard0()
    {
        cardNumber = 0;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>(); 
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard1()
    {
        cardNumber = 1;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }
    
     public void TouchCard2()
    {
        cardNumber = 2;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard3()
    {
        cardNumber = 3;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard4()
    {
        cardNumber = 4;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard5()
    {
        cardNumber = 5;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard6()
    {
        cardNumber = 6;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard7()
    {
        cardNumber = 7;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard8()
    {
        cardNumber = 8;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard9()
    {
        cardNumber = 9;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard10()
    {
        cardNumber = 10;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard11()
    {
        cardNumber = 11;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard12()
    {
        cardNumber = 12;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard13()
    {
        cardNumber = 13;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard14()
    {
        cardNumber = 14;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard15()
    {
        cardNumber = 15;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard16()
    {
        cardNumber = 16;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard17()
    {
        cardNumber = 17;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard18()
    {
        cardNumber = 18;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void TouchCard19()
    {
        cardNumber = 19;
        cardPrefab.SetActive(true);
        var card = cardPrefab.GetComponent<Card>();
        card.Setup(itemSO.items[cardNumber], true);
        card.GetComponent<Order>().SetOriginOrder(1);
    }

    public void OXUpdate()
    {
        if (DeckSelectButton.Inst.p1Percent[0] == true)
            cardOX0.GetComponent<Image>().sprite = OImage;
        else
            cardOX0.GetComponent<Image>().sprite = XImage;
        
        if (DeckSelectButton.Inst.p1Percent[1] == true)
            cardOX1.GetComponent<Image>().sprite = OImage;
        else
            cardOX1.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[2] == true)
            cardOX2.GetComponent<Image>().sprite = OImage;
        else
            cardOX2.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[3] == true)
            cardOX3.GetComponent<Image>().sprite = OImage;
        else
            cardOX3.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[4] == true)
            cardOX4.GetComponent<Image>().sprite = OImage;
        else
            cardOX4.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[5] == true)
            cardOX5.GetComponent<Image>().sprite = OImage;
        else
            cardOX5.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[6] == true)
            cardOX6.GetComponent<Image>().sprite = OImage;
        else
            cardOX6.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[7] == true)
            cardOX7.GetComponent<Image>().sprite = OImage;
        else
            cardOX7.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[8] == true)
            cardOX8.GetComponent<Image>().sprite = OImage;
        else
            cardOX8.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[9] == true)
            cardOX9.GetComponent<Image>().sprite = OImage;
        else
            cardOX9.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[10] == true)
            cardOX10.GetComponent<Image>().sprite = OImage;
        else
            cardOX10.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[11] == true)
            cardOX11.GetComponent<Image>().sprite = OImage;
        else
            cardOX11.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[12] == true)
            cardOX12.GetComponent<Image>().sprite = OImage;
        else
            cardOX12.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[13] == true)
            cardOX13.GetComponent<Image>().sprite = OImage;
        else
            cardOX13.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[14] == true)
            cardOX14.GetComponent<Image>().sprite = OImage;
        else
            cardOX14.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[15] == true)
            cardOX15.GetComponent<Image>().sprite = OImage;
        else
            cardOX15.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[16] == true)
            cardOX16.GetComponent<Image>().sprite = OImage;
        else
            cardOX16.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[17] == true)
            cardOX17.GetComponent<Image>().sprite = OImage;
        else
            cardOX17.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[18] == true)
            cardOX18.GetComponent<Image>().sprite = OImage;
        else
            cardOX18.GetComponent<Image>().sprite = XImage;

        if (DeckSelectButton.Inst.p1Percent[19] == true)
            cardOX19.GetComponent<Image>().sprite = OImage;
        else
            cardOX19.GetComponent<Image>().sprite = XImage;
    }
}
