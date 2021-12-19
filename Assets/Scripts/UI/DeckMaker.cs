using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckMaker : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Sprite deckO;
    [SerializeField] Sprite deckX;
    
    public int cardNumber;
    public int selectCardNumber;

    public void TouchCard()
    {

    }

    public void SelectButton()
    {
        selectCardNumber = cardNumber;
        
    }
}
