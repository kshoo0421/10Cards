using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // TMP ���
using Random = UnityEngine.Random;  //System�� UnityEngine�� Random�� ���ļ� UnityEngine�� Random�� ����ϰڴٴ� �ǹ�
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    private void Awake() => Inst = this;

    [SerializeField] ItemSO itemSO; // �ܺ� "ItemSO(��������)"�� itemSO(��ũ��Ʈ) ����
    [SerializeField] GameObject cardPrefab; // �ܺ� "GameObject(��������)"�� cardPrefab(��ũ��Ʈ) ����
    [SerializeField] List<Card> myCards;    // �� ī�� ����Ʈ�� myCards(��ũ��Ʈ) ����
    [SerializeField] List<Card> otherCards; // ��� ī�� ����Ʈ�� otherCards(��ũ��Ʈ) ����
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] Transform otherCardSpawnPoint;
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;
    [SerializeField] Transform otherCardLeft;
    [SerializeField] Transform otherCardRight;  // cardSpawnPoint, myCardLeft, myCardRight, otherCardLeft, otherCardRight�� ��ġ�� ��ũ��Ʈ ����
    [SerializeField] ECardState eCardState; // ���콺 Ŭ��/�巡�� ���� Ȯ�ο�
    [SerializeField] TMP_Text myDeckTMP;  // �� �� TMP
    [SerializeField] TMP_Text otherDeckTMP;  // ��� �� TMP
    [SerializeField] Transform myEntitySpawnPoint;
    [SerializeField] Transform otherEntitySpawnPoint;
    [SerializeField] TMP_Text TurnCardTMP;  // �� ī�� �� TMP
    [SerializeField] TMP_Text MaxCardTMP;  // �ִ� ī�� �� TMP
    [SerializeField] GameObject myBackCard;  // �� ī�� �� TMP
    [SerializeField] GameObject otherBackCard;  // �ִ� ī�� �� TMP

    public List<Item> myDeckCount;  // ������ ���� ����Ʈ ����
    public List<Item> otherDeckCount;  // ������ ���� ����Ʈ ����
    Card selectCard;    // ������ ī�� ���� ����
    bool isMyCardDrag;  // �� ī�� �巡�� ���� ���� ����
    bool onMyCardArea;  // �� ī�� ����(ī�� Ȯ�� ����)
    enum ECardState { Nothing, CanMouseOver, CanMouseDrag } // ECardState�� 1. �ƹ� �͵� �ȵǴ� ���, 2. Ȯ�븸 �Ǵ� ���, 3. �巡�ױ��� �Ǵ� ���� ����
    int myPutCount; // �� �Ͽ� ī�� ���� ����
    public bool[] myPercent;    // �� �� ����
    public bool[] otherPercent; // ��� �� ����

    // ���� ����

    void Start()    // �� ���� �� ������ ����, AddCard, OnTurnStarted ȣ��
    {
        SetupItemBuffer();  // �� ���� �� ������ ����
        TurnManager.OnAddCard += AddCard;   // TurnManager�� OnAddCard�� ȣ���ϸ� AddCard�Լ� ȣ��
        TurnManager.OnTurnStarted += OnTurnStarted; // TurnManager�� OnTurnStarted�� ȣ���ϸ� OnTurnStarted�Լ� ȣ��
    }

    void Update()   // �巡��, DetectCardArea, ī�� ���� ���� ����
    {
        if (isMyCardDrag)   // �� ī�� �巡�� O
            CardDrag(); // ī�� �巡��

        DetectCardArea();   // ī�� ���� �ν�

        SetECardState();    // ī�� ���� ���� ����

        CountNumbering();   // �ʵ忡 ���� ǥ��
    }

    void OnDestroy()
    {
        TurnManager.OnAddCard -= AddCard;   // TurnManager�� OnAddCard ȣ�� ����ϸ� AddCard �Լ� ȣ�� ���
        TurnManager.OnTurnStarted -= OnTurnStarted; // TurnManager�� OnTurnStarted ȣ�� ����ϸ� OnTurnStarted �Լ� ȣ�� ���
    }

    // ���� �Լ�

    void AddCard(bool isMine)   // �� ī������ �ƴ���
    {
        var cardObject = Instantiate(cardPrefab, isMine ? cardSpawnPoint.position : otherCardSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
        var card = cardObject.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
        card.Setup(PopItem(isMine), isMine);  // �� or ��� ī�� �̱�
        (isMine ? myCards : otherCards).Add(card);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

        SetOriginOrder(isMine); // ī�� ���̾� ���� ����
        CardAlignment(isMine);  // ī��� ��ġ ����

    }

    void SetOriginOrder(bool isMine)    // ���̾� ����
    {
        int Count = isMine ? myCards.Count : otherCards.Count; // �� ī�� �� �� or ��� ī�� �� ��
        for (int i = 0; i < Count; i++)  // ���� ī��(��) ��ü ���̾� ����
        {
            var targetCard = isMine ? myCards[i] : otherCards[i];   // �� ī�� or ��� ī��
            targetCard?.GetComponent<Order>().SetOriginOrder(i);    // ?�� Nullable(Null�� ��� ����); ī��� ��ġ ����
        }
    }
    
    void CardAlignment(bool isMine) // �� ��ġ ����(�� ī�� or ��� ��ġ)
    {
    
        List<PRS> originCardPRSs = new List<PRS>(); // ī�� ����Ʈ�� ��ġ, ȸ��, ũ��
        if (isMine) // �� ī��
            originCardPRSs = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one * 10f);  // �޿� ��ġ, ī��(��) ��, ������, ũ��
        else     // ��� ī��
            originCardPRSs = RoundAlignment(otherCardLeft, otherCardRight, otherCards.Count, -0.5f, Vector3.one * 10f);    // ���� ����
    

        var targetCards = isMine ? myCards : otherCards;    // ����(�� or ���)
        for (int i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];

            targetCard.originPRS = originCardPRSs[i];   // ���� ī����� ��ġ
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f); // �� �ӵ��� �����̱�
        }
    }
    
    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)    // ī�� ����Ʈ�� ���� ��ġ ����
    {
        float[] objLerps = new float[objCount]; // ����Ʈ�� ����
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break; // 1�� ��ġ
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break; // 2�� ��ġ 
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break; // 3�� ��ġ
            default:    // �� �̿�
                float interval = 1f / (objCount - 1);   // ���� = 1 / (�� ���� -1)
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i; // �� ī����� x�� ��ġ ����
                break;
        }

        for (int i = 0; i < objCount; i++)    // y�� ����
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);   // ��ġ : �������� �̵�
            var targetRot = Utils.QI;   // ȸ��
            if (objCount >= 4)   // 4�� �̻��� ��
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));  // curve�� : ���� ������ ���� ����
                curve = height >= 0 ? curve : -curve;   // ���̸� ���밪����
                targetPos.y += curve;   // 
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;
    }
    
    public Item PopItem(bool isMine) // ����Ʈ���� �̱�(����)
    {
        if (isMine == true)
        {
            Item item = myDeckCount[0];  // �������� ù ��°������
            myDeckCount.RemoveAt(0); // ������ ���ۿ��� ù ��° ī�� ����
            return item;    // �Լ� ����
        }
        else
        {
            Item item = otherDeckCount[0];  // �������� ù ��°������
            otherDeckCount.RemoveAt(0); // ������ ���ۿ��� ù ��° ī�� ����
            return item;    // �Լ� ����
        }
    }

    void SetupItemBuffer() // ������ ���� ����
    {
        myDeckCount = new List<Item>(20);   // ������ ���۴� ������ ����Ʈ(20��)�� ����Ʈ(�� ����Ʈ)
        
        for (int i = 0; i < 20; i++) // �������� �� ����(20)��ŭ �ݺ�
        {
            Item item = itemSO.items[i];    // item = �����۸���Ʈ�� i��° ������
            bool per = myPercent[i];
            if(per == true)
                myDeckCount.Add(item);   // ����Ʈ�� ī�� �߰�
        }

        for (int i = 0; i < myDeckCount.Count; i++)    // ���� ������ ����; ������ ������ ������ŭ
        {
            int rand = Random.Range(i, myDeckCount.Count);   // rand�� �� �������� ���� �� �� ������
            Item temp = myDeckCount[i];  // i��° ������ �ӽ� ����
            myDeckCount[i] = myDeckCount[rand];   // ���� ��ġ�� �������� i��°��
            myDeckCount[rand] = temp;    // i��° �������� �� ������ġ�� �ٲ�
        }

        otherDeckCount = new List<Item>(20);
        for (int i = 0; i < itemSO.items.Length; i++) // �������� �� ����(100)��ŭ �ݺ�
        {
            Item item = itemSO.items[i];    // item = �����۸���Ʈ�� i��° ������
            bool per = myPercent[i];
            if (per == true)
                otherDeckCount.Add(item);   // ����Ʈ�� ī�� �߰�
        }

        for (int i = 0; i < otherDeckCount.Count; i++)    // ��벨
        {
            int rand = Random.Range(i, otherDeckCount.Count);   
            Item temp = otherDeckCount[i];
            otherDeckCount[i] = otherDeckCount[rand];
            otherDeckCount[rand] = temp;  
        }
        
    }

    void OnTurnStarted(bool myTurn) // �� ���� ��
    {
        if (myTurn == true) // �� ���̸�
            myPutCount = 0; // ī�� ���� �� ����
    }
   
    public bool TryPutCard(bool isMine) // ī�� ���� �Լ�
    {
        if (isMine && myPutCount >= 2)  // �� ī���ε�, �̹� �� ī�尡 2�� ������
            return false;   // false ��ȯ
        if (!isMine && otherCards.Count <= 0)   // ��� ī���ε�, ��� ī�� �а� 0�̶��
            return false;   // false ��ȯ

        Card card = isMine ? selectCard : otherCards[Random.Range(0, otherCards.Count)];    // �� ī��� ������ ī��, ��� ī��� ī�� �� �ƹ��ų�
        Vector3 spawnPoint = isMine ? Utils.MousePos : otherEntitySpawnPoint.position;
        var targetCards = isMine ? myCards : otherCards;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

        if (EntityManager.Inst.SpawnEntity(isMine, card.item, spawnPoint))    // ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
        {
            targetCards.Remove(card);   // Ÿ�� ī��(��)���� ī�� ����
            card.transform.DOKill();    // ���ֹ���
            DestroyImmediate(card.gameObject);
            if (isMine)
            {
                selectCard = null;
                myPutCount++;
            }
            CardAlignment(isMine);
            return true;
        }
        else
        {
            targetCards.ForEach(x => x.GetComponent<Order>().SetMostFrontOrder(false));
            CardAlignment(isMine);
            return false;
        }
    }

    void CountNumbering()   // �� �� ī��Ʈ
    {
        myDeckTMP.text = this.myDeckCount.Count.ToString();
        otherDeckTMP.text = this.otherDeckCount.Count.ToString();
        if (this.myDeckCount.Count == 0)
        {
            myBackCard.SetActive(false);
            StartCoroutine(GameManager.Inst.GameOver(false));
        }
        else if (this.otherDeckCount.Count == 0)
        { 
            otherBackCard.SetActive(false);
            StartCoroutine(GameManager.Inst.GameOver(true));
        }
        TurnCardTMP.text = this.myPutCount.ToString();
    }

    // #region ���콺 ����

    #region MyCard

    public void CardMouseOver(Card card)    // ���콺 �ø���
    {
        if (eCardState == ECardState.Nothing)
            return;
        
        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)    // ���콺 �ø��� ����
    {
        EnlargeCard(false, card); 
    }
    
    public void CardMouseDown() // Ŭ�� ��
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;
        
        isMyCardDrag = true;
    }

    public void CardMouseUp()   // Ŭ�� ����
    {
        isMyCardDrag = false;

        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (onMyCardArea)
            EntityManager.Inst.RemoveMyEmptyEntity();

        if (!onMyCardArea)
            TryPutCard(true);
    }

    void CardDrag() // ī�� �巡�� �Լ�
    {
        if (!onMyCardArea)  // �� ī�� ���� �����
        {
            if (eCardState != ECardState.CanMouseDrag)  // ���콺 �巡�� ������ ���°� �ƴ϶��(�巡�� �Ұ�)
                return; // �״�� ��ȯ

            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false); // ī�� ������, ��Ʈ�� ���X
            EntityManager.Inst.InsertMyEmptyEntity(Utils.MousePos.x);   // x�࿡ ���� �� ��ƼƼ ����(��ġ ��� �뵵)
        }
    }

    void EnlargeCard(bool isEnlarge, Card card) // ī�� Ȯ��
    {
        if (isEnlarge)
        {
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, -65f, -10f);

            card.MoveTransform(new PRS(enlargePos, Utils.QI, Vector3.one * 13.5f), false);
        }
        else
            card.MoveTransform(card.originPRS, false);

        card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }

    void DetectCardArea()   // ī�� ���� �ν�
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);    // 2D�� ���콺 ��ġ �ν�
        int layer = LayerMask.NameToLayer("MyCardArea");    // �ν��ϴ� ���̾� �̸� "MyCardArea"
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);   // onMyCardArea�� �浹�ϴ� ���� ����
    }

    void SetECardState()    // ī�� ���� ���� ����
    {
        if (TurnManager.Inst.isLoading) // �ε� ���̸�
            eCardState = ECardState.Nothing;    // ī�� ���� �ȵ�

        else if (!TurnManager.Inst.myTurn || myPutCount == 2)    // �� ���� �ƴϰų�, �̹� 1���� �°ų�, ��ƼƼ�� �� á����
            eCardState = ECardState.CanMouseOver;   // ī�� Ȯ�븸 ����

        else if (TurnManager.Inst.myTurn && (myPutCount == 0 || myPutCount == 1))    // �� ���̸鼭 ���� �� ī�尡 ������
            eCardState = ECardState.CanMouseDrag;   // ���콺 �巡�� ����
    }
    #endregion
}