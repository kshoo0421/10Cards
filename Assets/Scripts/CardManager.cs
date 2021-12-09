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
    [SerializeField] List<Card> myCards;    // �� �� ����Ʈ�� myCards(��ũ��Ʈ) ����
    [SerializeField] List<Card> otherCards; // ��� �� ����Ʈ�� otherCards(��ũ��Ʈ) ����
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
    
    WaitForSeconds delay1 = new WaitForSeconds(1);  // delay1�� 1�� ���
    WaitForSeconds delay2 = new WaitForSeconds(2);  // delay2�� 2�� ���
    WaitForSeconds delay3 = new WaitForSeconds(3);  // delay2�� 2�� ���


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
        int Count = isMine ? myCards.Count : otherCards.Count; // �� �� �� �� or ��� ī�� �� ��
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
            bool per = otherPercent[i];
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

        if (EntityManager.Inst.SpawnEntity(isMine, card.item, spawnPoint))    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
        {
            targetCards.Remove(card);   // Ÿ�� ī��(��)���� ī�� ����
            CardEffect(card.item.effectNumber, isMine, card.item);
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
        else   // �з� �ǵ��ư���
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

    IEnumerator Delay3()  // ��� AI ����
    {
        Debug.Log("3�ʰ� ����մϴ�.");
        yield return delay3;
    }

    void CardEffect(int effectNumber, bool myTurn, Item item)
    {
        switch (effectNumber)
        {
            case 1: // ����� : ��� �� 1���� �����ϴ�.
                Debug.Log("����� �ߵ�");
                StartCoroutine(Delay3());
                var cardObject01 = Instantiate(cardPrefab, !myTurn ? cardSpawnPoint.position : otherCardSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card01 = cardObject01.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card01.Setup(PopItem(!myTurn), !myTurn);  // �� or ��� ī�� �̱�
                (!myTurn ? myCards : otherCards).Add(card01);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                SetOriginOrder(!myTurn); // ī�� ���̾� ���� ����
                
                Vector3 spawnPoint01 = !myTurn ? cardSpawnPoint.position : otherCardSpawnPoint.position;
                var targetCards01 = !myTurn ? myCards : otherCards;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

                EntityManager.Inst.SpawnEntity(!myTurn, card01.item, spawnPoint01);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                targetCards01.Remove(card01);   // Ÿ�� ī��(��)���� ī�� ����
                card01.transform.DOKill();    // ���ֹ���
                DestroyImmediate(card01.gameObject);
                
                break;
                
            case 2: // ���� : ��� �� 1���� �����ɴϴ�.
                Debug.Log("���� �ߵ�");
                StartCoroutine(Delay3());

                var cardObject02 = Instantiate(cardPrefab, myTurn ?  otherCardSpawnPoint.position : cardSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card02 = cardObject02.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card02.Setup(PopItem(!myTurn), myTurn);  // �� or ��� ī�� �̱�
                (myTurn ? myCards : otherCards).Add(card02);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                SetOriginOrder(myTurn); // ī�� ���̾� ���� ����
                CardAlignment(myTurn);  // ī��� ��ġ ����

                break;

            case 3: // ���� : ��� �� 2���� ��밡 ����մϴ�.
                Debug.Log("���� �ߵ�");
                StartCoroutine(Delay3());

                for (int i = 0; i < 2; i++)
                {
                    var cardObject03 = Instantiate(cardPrefab, !myTurn ? cardSpawnPoint.position : otherCardSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                    var card03 = cardObject03.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                    card03.Setup(PopItem(!myTurn), !myTurn);  // �� or ��� ī�� �̱�
                    (!myTurn ? myCards : otherCards).Add(card03);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                    SetOriginOrder(!myTurn); // ī�� ���̾� ���� ����

                    Vector3 spawnPoint03 = !myTurn ? cardSpawnPoint.position : otherCardSpawnPoint.position;
                    var targetCards03 = !myTurn ? myCards : otherCards;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

                    EntityManager.Inst.SpawnEntity(!myTurn, card03.item, spawnPoint03);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�

                    targetCards03.Remove(card03);   // Ÿ�� ī��(��)���� ī�� ����
                    Delay3();
                    CardEffect(card03.item.effectNumber, !myTurn, card03.item);
                    card03.transform.DOKill();    // ���ֹ���
                    DestroyImmediate(card03.gameObject);
                }
                break;
                
            case 4: // ���� : �������� ��� �� 1���� �����ϴ�.
                Debug.Log("���� �ߵ�");
                StartCoroutine(Delay3());

                if (myTurn && (myTurn ? otherCards.Count : myCards.Count) <= 0)   // ������ ī�尡 ������
                    break;

                Card card04 = myTurn ? otherCards[Random.Range(0, otherCards.Count)] : myCards[Random.Range(0, myCards.Count)];    // �� ���̸� ��� �� ������, ��� ���̸� �� �� ������
                Vector3 spawnPoint04 = myTurn ? otherEntitySpawnPoint.position : myEntitySpawnPoint.position;
                var targetCards04 = myTurn ? otherCards : myCards;    // Ÿ�� ī��� �� ���ʸ� ��� ��, �ƴϸ� �� ��

                if (EntityManager.Inst.SpawnEntity(!myTurn, card04.item, spawnPoint04))    // ī�� ����; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                {
                    targetCards04.Remove(card04);   // �п��� ī�� ����
                    card04.transform.DOKill();    // ���ֹ���
                    DestroyImmediate(card04.gameObject);
                    if (myTurn)
                        selectCard = null;
                    CardAlignment(!myTurn);
                }
                break;

            case 5: // �Ƽ� : ��� �� 1���� ���� ����մϴ�.
                Debug.Log("�Ƽ� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 6: // ����̱� : ��� �� 1���� �����ɴϴ�.
                Debug.Log("����̱� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 7: // ���� : ���� ���� �Ͽ� 1�常 ����� �� �ֽ��ϴ�.
                Debug.Log("���� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 8: // �����մϴ�. : ��� �����Կ��� �������� ī�� 1���� �����ɴϴ�.
                Debug.Log("�����մϴ�. �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 9: // �޸��� : �� �� 2���� �̽��ϴ�.
                Debug.Log("�޸��� �ߵ�");
                StartCoroutine(Delay3());
                AddCard(myTurn);
                AddCard(myTurn);
                break;

            case 10: // ���� ���� : �� �� 1���� ����մϴ�. 
                Debug.Log("���� ���� �ߵ�");
                StartCoroutine(Delay3());
                var cardObject10 = Instantiate(cardPrefab, myTurn ? cardSpawnPoint.position : otherCardSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card10 = cardObject10.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card10.Setup(PopItem(myTurn), myTurn);  // �� or ��� ī�� �̱�
                (myTurn ? myCards : otherCards).Add(card10);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                SetOriginOrder(myTurn); // ī�� ���̾� ���� ����

                Vector3 spawnPoint10 = myTurn ? cardSpawnPoint.position : otherCardSpawnPoint.position;
                var targetCards10 = myTurn ? myCards : otherCards;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

                EntityManager.Inst.SpawnEntity(!myTurn, card10.item, spawnPoint10);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                targetCards10.Remove(card10);   // Ÿ�� ī��(��)���� ī�� ����
                Delay3();
                CardEffect(card10.item.effectNumber, myTurn, card10.item);
                card10.transform.DOKill();    // ���ֹ���
                DestroyImmediate(card10.gameObject);

                break; 
            
            case 11: // ���� : �� �� 2���� ����մϴ�.
                Debug.Log("���� �ߵ�");
                StartCoroutine(Delay3());
                for (int i = 0; i<2; i++)
                {
                    var cardObject11 = Instantiate(cardPrefab, myTurn ? cardSpawnPoint.position : otherCardSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                    var card11 = cardObject11.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                    card11.Setup(PopItem(myTurn), myTurn);  // �� or ��� ī�� �̱�
                    (myTurn ? myCards : otherCards).Add(card11);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                    SetOriginOrder(myTurn); // ī�� ���̾� ���� ����

                    Vector3 spawnPoint11 = myTurn ? cardSpawnPoint.position : otherCardSpawnPoint.position;
                    var targetCards11 = myTurn ? myCards : otherCards;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

                    EntityManager.Inst.SpawnEntity(!myTurn, card11.item, spawnPoint11);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                    targetCards11.Remove(card11);   // Ÿ�� ī��(��)���� ī�� ����
                    Delay3();
                    CardEffect(card11.item.effectNumber, myTurn, card11.item);
                    card11.transform.DOKill();    // ���ֹ���
                    DestroyImmediate(card11.gameObject);
                }

                break;

            case 12: // ��Ȱ��ǰ : �� ���� �� ī�带 �ֽ��ϴ�.
                Debug.Log("��Ȱ��ǰ �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 13: // ���� : �� �и� ��� ������, �׸�ŭ ��� ���� �����ϴ�.
                Debug.Log("���� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 14: // �и����� : �� �����Կ��� �������� ī�� 1���� �����ɴϴ�.
                Debug.Log("�и����� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 15: // �ſ�ġ�� : ���� �Ͽ� ��밡 ����ߴ� ī����� ����մϴ�.
                Debug.Log("�ſ�ġ�� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 16: // ��λ��� : ���� �� 1���� �̽��ϴ�.
                Debug.Log("��λ��� �ߵ�");
                StartCoroutine(Delay3());
                AddCard(true);
                AddCard(false);
                break;

            case 17: // ���� ���� : ���� �� 1���� �����ϴ�.
                Debug.Log("���� ���� �ߵ�");
                StartCoroutine(Delay3());
                break;

            case 18: // ��ȯ : ������ �п��� �������� 1���� �����ɴϴ�.
                Debug.Log("��ȯ �ߵ�");
                StartCoroutine(Delay3());
                break;

            case 19: // ��û�� : ���� �и� ��� �����ϴ�. 
                Debug.Log("��û�� �ߵ�");
                StartCoroutine(Delay3());
                /*
                int otherCardCount = otherCards.Count;
                Debug.Log(otherCardCount);
                for (int i = 0; i <= otherCardCount; i++)    // ��� ������
                {
                    Card card191 = myTurn ? otherCards[Random.Range(0, otherCards.Count)] : myCards[Random.Range(0, myCards.Count)];    // �� ���̸� ��� �� ������, ��� ���̸� �� �� ������
                    Vector3 spawnPoint191 = myTurn ? otherEntitySpawnPoint.position : myEntitySpawnPoint.position;
                    var targetCards191 = myTurn ? otherCards : myCards;    // Ÿ�� ī��� �� ���ʸ� ��� ��, �ƴϸ� �� ��

                    if (EntityManager.Inst.SpawnEntity(!myTurn, card191.item, spawnPoint191))    // ī�� ����; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                    {
                        targetCards191.Remove(card191);   // �п��� ī�� ����
                        card191.transform.DOKill();    // ���ֹ���
                        DestroyImmediate(card191.gameObject);
                        if (myTurn)
                            selectCard = null;
                        CardAlignment(!myTurn);
                    }
                }
                Debug.Log("��� ��û�� ��");

                int myCardCount = myCards.Count-1;
                Debug.Log(myCardCount);
                for (int i = 0; i <= myCardCount; i++)    // ��� ������
                {
                    Card card192 = myTurn ? myCards[Random.Range(0, myCards.Count)] : otherCards[Random.Range(0, otherCards.Count)];    // �� ���̸� �� �� ������, ��� ���̸� ��� �� ������
                    Vector3 spawnPoint192 = myTurn ?  myEntitySpawnPoint.position : otherEntitySpawnPoint.position;
                    var targetCards192 = myTurn ? myCards : otherCards;    // Ÿ�� ī��� �� ���ʸ� ��� ��, �ƴϸ� �� ��

                    if (EntityManager.Inst.SpawnEntity(myTurn, card192.item, spawnPoint192))    // ī�� ����; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                    {
                        targetCards192.Remove(card192);   // �п��� ī�� ����
                        card192.transform.DOKill();    // ���ֹ���
                        DestroyImmediate(card192.gameObject);
                        if (myTurn)
                            selectCard = null;
                        CardAlignment(myTurn);
                    }
                }
                */
                break;

            case 20: // ���� �� : ���� �������� ��� �� 1���� ����մϴ�. 
                Debug.Log("���� �� �ߵ�");
                StartCoroutine(Delay3());
                break;
                
            default:
                Debug.Log("ī�尡 ��Ͽ� �����ϴ�.");
                break;
        }
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