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

    [SerializeField] ECardState eCardState; // ���콺 Ŭ��/�巡�� ���� Ȯ�ο�
    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<Card> p1Hands;
    [SerializeField] List<Card> p2Hands;
    [SerializeField] Transform p1HandSpawnPoint;
    [SerializeField] Transform p2HandSpawnPoint;
    [SerializeField] Transform p1HandLeft;
    [SerializeField] Transform p1HandRight;
    [SerializeField] Transform p2HandLeft;
    [SerializeField] Transform p2HandRight;
    [SerializeField] TMP_Text p1DeckTMP;  // �� �� TMP
    [SerializeField] TMP_Text p2DeckTMP;  // ��� �� TMP
    [SerializeField] Transform p1EntitySpawnPoint;
    [SerializeField] Transform p2EntitySpawnPoint;
    [SerializeField] TMP_Text currentCardTMP;
    [SerializeField] TMP_Text MaxCardTMP;  // �ִ� ī�� �� TMP
    [SerializeField] GameObject p1BackCard;  // �� ī�� �� TMP
    [SerializeField] GameObject p2BackCard;  // �ִ� ī�� �� TMP

    WaitForSeconds delay1 = new WaitForSeconds(1);  // delay1�� 1�� ���
    WaitForSeconds delay2 = new WaitForSeconds(2);  // delay2�� 2�� ���
    WaitForSeconds delay3 = new WaitForSeconds(3);  // delay2�� 2�� ���


    public List<Item> p1DeckCount;  // ������ ���� ����Ʈ ����
    public List<Item> p2DeckCount;  // ������ ���� ����Ʈ ����
    Card selectCard;    // ������ ī�� ���� ����
    bool isP1HandDrag;  // �� ī�� �巡�� ���� ���� ����
    bool onP1HandArea;  // �� ī�� ����(ī�� Ȯ�� ����)
    enum ECardState { Nothing, CanMouseOver, CanMouseDrag } // ECardState�� 1. �ƹ� �͵� �ȵǴ� ���, 2. Ȯ�븸 �Ǵ� ���, 3. �巡�ױ��� �Ǵ� ���� ����
    public int maxCount;
    public int p1PutCount; // �� �Ͽ� ī�� ���� ����
    public int p2PutCount; // ��� �Ͽ� ī�� ���� ����
    public bool[] p1Percent;    // �� �� ����
    public bool[] p2Percent; // ��� �� ����
    public bool cardEffect07;

    // ���� ����
    void Start()    // �� ���� �� ������ ����, AddCard, OnTurnStarted ȣ��
    {
        SetupItemBuffer();  // �� ���� �� ������ ����
        maxCount = 1;
        p1PutCount = 0;
        p2PutCount = 0;
        cardEffect07 = false;
        TurnManager.OnAddCard += AddCard;   // TurnManager�� OnAddCard�� ȣ���ϸ� AddCard�Լ� ȣ��
        TurnManager.OnTurnStarted += OnTurnStarted; // TurnManager�� OnTurnStarted�� ȣ���ϸ� OnTurnStarted�Լ� ȣ��
    }

    void Update()   // �巡��, DetectCardArea, ī�� ���� ���� ����
    {
        if (isP1HandDrag)   // �� ī�� �巡�� O
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
        var cardObject = Instantiate(cardPrefab, isMine ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
        var card = cardObject.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
        card.Setup(PopItem(isMine), isMine);  // �� or ��� ī�� �̱�
        (isMine ? p1Hands : p2Hands).Add(card);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

        SetOriginOrder(isMine); // ī�� ���̾� ���� ����
        CardAlignment(isMine);  // ī��� ��ġ ����
    }

    void SetOriginOrder(bool isMine)    // ���̾� ����
    {
        int Count = isMine ? p1Hands.Count : p2Hands.Count; // �� �� �� �� or ��� ī�� �� ��
        for (int i = 0; i < Count; i++)  // ���� ī��(��) ��ü ���̾� ����
        {
            var targetCard = isMine ? p1Hands[i] : p2Hands[i];   // �� ī�� or ��� ī��
            targetCard?.GetComponent<Order>().SetOriginOrder(i);    // ?�� Nullable(Null�� ��� ����); ī��� ��ġ ����
        }
    }

    void CardAlignment(bool isMine) // �� ��ġ ����(�� ī�� or ��� ��ġ)
    {

        List<PRS> originCardPRSs = new List<PRS>(); // ī�� ����Ʈ�� ��ġ, ȸ��, ũ��
        if (isMine) // �� ī��
            originCardPRSs = RoundAlignment(p1HandLeft, p1HandRight, p1Hands.Count, 0.5f, Vector3.one * 10f);  // �޿� ��ġ, ī��(��) ��, ������, ũ��
        else     // ��� ī��
            originCardPRSs = RoundAlignment(p2HandLeft, p2HandRight, p2Hands.Count, -0.5f, Vector3.one * 10f);    // ���� ����


        var targetCards = isMine ? p1Hands : p2Hands;    // ����(�� or ���)
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
            Item item = p1DeckCount[0];  // �������� ù ��°������
            p1DeckCount.RemoveAt(0); // ������ ���ۿ��� ù ��° ī�� ����
            return item;    // �Լ� ����
        }
        else
        {
            Item item = p2DeckCount[0];  // �������� ù ��°������
            p2DeckCount.RemoveAt(0); // ������ ���ۿ��� ù ��° ī�� ����
            return item;    // �Լ� ����
        }
    }

    void SetupItemBuffer() // ������ ���� ����
    {
        p1DeckCount = new List<Item>(20);   // ������ ���۴� ������ ����Ʈ(20��)�� ����Ʈ(�� ����Ʈ)

        for (int i = 0; i < 20; i++) // �������� �� ����(20)��ŭ �ݺ�
        {
            Item item = itemSO.items[i];    // item = �����۸���Ʈ�� i��° ������
            bool per = p1Percent[i];
            if (per == true)
                p1DeckCount.Add(item);   // ����Ʈ�� ī�� �߰�
        }

        for (int i = 0; i < p1DeckCount.Count; i++)    // ���� ������ ����; ������ ������ ������ŭ
        {
            int rand = Random.Range(i, p1DeckCount.Count);   // rand�� �� �������� ���� �� �� ������
            Item temp = p1DeckCount[i];  // i��° ������ �ӽ� ����
            p1DeckCount[i] = p1DeckCount[rand];   // ���� ��ġ�� �������� i��°��
            p1DeckCount[rand] = temp;    // i��° �������� �� ������ġ�� �ٲ�
        }

        p2DeckCount = new List<Item>(20);
        for (int i = 0; i < itemSO.items.Length; i++) // �������� �� ����(100)��ŭ �ݺ�
        {
            Item item = itemSO.items[i];    // item = �����۸���Ʈ�� i��° ������
            bool per = p2Percent[i];
            if (per == true)
                p2DeckCount.Add(item);   // ����Ʈ�� ī�� �߰�
        }

        for (int i = 0; i < p2DeckCount.Count; i++)    // ��벨
        {
            int rand = Random.Range(i, p2DeckCount.Count);
            Item temp = p2DeckCount[i];
            p2DeckCount[i] = p2DeckCount[rand];
            p2DeckCount[rand] = temp;
        }
    }

    void OnTurnStarted(bool p1Turn) // �� ���� ��
    {
        CurrentCardNumbering(p1Turn);
        if (cardEffect07 == true)
            maxCount = 1;
    }

    public bool TryPutCard(bool isMine) // ī�� ���� �Լ�
    {
        if (isMine && p1PutCount >= maxCount)  // �� ī���ε�, �̹� �� ī�尡 2�� ������
            return false;   // false ��ȯ
        if ((!isMine && p2Hands.Count <= 0) || (!isMine && p2PutCount >= maxCount))   // ��� ī���ε�, ��� ī�� �а� 0�̶��
            return false;   // false ��ȯ

        Card card = isMine ? selectCard : p2Hands[Random.Range(0, p2Hands.Count)];    // �� ī��� ������ ī��, ��� ī��� ī�� �� �ƹ��ų�
        Vector3 spawnPoint = isMine ? Utils.MousePos : p2EntitySpawnPoint.position;
        var targetCards = isMine ? p1Hands : p2Hands;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

        if (EntityManager.Inst.SpawnEntity(isMine, card.item, spawnPoint))    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
        {
            targetCards.Remove(card);   // Ÿ�� ī��(��)���� ī�� ����
            card.transform.DOKill();    // ���ֹ���
            DestroyImmediate(card.gameObject);
            CardEffect(card.item.effectNumber, isMine, card.item);
            if (isMine)
            {
                selectCard = null;
                p1PutCount++;
            }
            else
                p2PutCount++;

            CurrentCardNumbering(isMine);
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
        
    public void CountNumbering()   // �� ī��Ʈ
    {
        p1DeckTMP.text = this.p1DeckCount.Count.ToString();
        p2DeckTMP.text = this.p2DeckCount.Count.ToString();
        MaxCardTMP.text = this.maxCount.ToString();

        if (this.p1DeckCount.Count == 0)
        {
            p1BackCard.SetActive(false);
            StartCoroutine(GameManager.Inst.GameOver(false));
        }
        else if (this.p2DeckCount.Count == 0)
        {
            p2BackCard.SetActive(false);
            StartCoroutine(GameManager.Inst.GameOver(true));
        }
    }

    void CurrentCardNumbering(bool myTurn)
    {
        currentCardTMP.text = (myTurn ? p1PutCount : p2PutCount).ToString();
    }

    IEnumerator Delay3()
    {
        yield return delay3;
    }

    void CardEffect(int effectNumber, bool p1Turn, Item item)
    {
        string player = p1Turn ? "player1" : "player2"; 
        switch (effectNumber)
        {
            case 1: // ����� : ��� �� 1���� �����ϴ�.
                Debug.Log(player + "����� �ߵ�");
                StartCoroutine(Delay3());
                var cardObject01 = Instantiate(cardPrefab, !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card01 = cardObject01.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card01.Setup(PopItem(!p1Turn), !p1Turn);  // �� or ��� ī�� �̱�
                Vector3 spawnPoint01 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, card01.item, spawnPoint01);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                break;
                
            case 2: // ���� : ��� �� 1���� �����ɴϴ�.
                Debug.Log(player + "���� �ߵ�");
                StartCoroutine(Delay3());

                var cardObject02 = Instantiate(cardPrefab, p1Turn ?  p2HandSpawnPoint.position : p1HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card02 = cardObject02.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card02.Setup(PopItem(!p1Turn), p1Turn);  // �� or ��� ī�� �̱�
                (p1Turn ? p1Hands : p2Hands).Add(card02);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                SetOriginOrder(p1Turn); // ī�� ���̾� ���� ����
                CardAlignment(p1Turn);  // ī��� ��ġ ����

                break;

            case 3: // ���� : ��� �� 2���� ��밡 ����մϴ�. 
                Debug.Log(player + "���� �ߵ�");

                break;
                
            case 4: // ���� : �������� ��� �� 1���� �����ϴ�.
                Debug.Log(player + "���� �ߵ�");
                StartCoroutine(Delay3());

                if (p1Turn && (p1Turn ? p2Hands.Count : p1Hands.Count) <= 0)   // ������ ī�尡 ������
                    break;

                Card card04 = p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];    // �� ���̸� ��� �� ������, ��� ���̸� �� �� ������
                Vector3 spawnPoint04 = p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;
                var targetCards04 = p1Turn ? p2Hands : p1Hands;    // Ÿ�� ī��� �� ���ʸ� ��� ��, �ƴϸ� �� ��

                if (EntityManager.Inst.SpawnEntity(!p1Turn, card04.item, spawnPoint04))    // ī�� ����; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                {
                    targetCards04.Remove(card04);   // �п��� ī�� ����
                    card04.transform.DOKill();    // ���ֹ���
                    DestroyImmediate(card04.gameObject);
                    card04 = null;
                    CardAlignment(!p1Turn);
                }
                break;

            case 5: // �Ƽ� : �������� ��� �� 1���� ���� ����մϴ�.
                Debug.Log(player + "�Ƽ� �ߵ�");
                
                StartCoroutine(Delay3());
                
                if ((p1Turn ? p2Hands : p1Hands).Count <= 0)   // ��� �а� ������
                    return;   // ��ȯ

                Card card05 = p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];    // �� ī��� ������ ī��, ��� ī��� ī�� �� �ƹ��ų�
                Vector3 spawnPoint05 = p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;
                var targetCards05 = p1Turn ? p2Hands : p1Hands; 

                if (EntityManager.Inst.SpawnEntity(p1Turn, card05.item, spawnPoint05))    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                {
                    targetCards05.Remove(card05);   // Ÿ�� ī��(��)���� ī�� ����
                    card05.transform.DOKill();    // ���ֹ���
                    DestroyImmediate(card05.gameObject);
                    CardEffect(card05.item.effectNumber, p1Turn, card05.item);
                    CardAlignment(!p1Turn);
                    return;
                }
                break;

            case 6: // ����̱� : ��� �� 1���� �����ɴϴ�.
                Debug.Log(player + "����̱� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 7: // ���� : ���� ���� �Ͽ� 1�常 ����� �� �ֽ��ϴ�.
                Debug.Log(player + "���� �ߵ�");
                StartCoroutine(Delay3());
                if (cardEffect07 == true)
                    TurnManager.Inst.turnCount = 1;
                else
                    cardEffect07 = true;
                break;

            case 8: // �����մϴ�. : ��� �����Կ��� �������� ī�� 1���� �����ɴϴ�.
                Debug.Log(player + "�����մϴ�. �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 9: // �޸��� : �� �� 2���� �̽��ϴ�.
                Debug.Log(player + "�޸��� �ߵ�");
                StartCoroutine(Delay3());
                AddCard(p1Turn);
                AddCard(p1Turn);
                break;

            case 10: // ���� ���� : �� �� 1���� ����մϴ�. 
                Debug.Log(player + "���� ���� �ߵ�");
               
                break; 
            
            case 11: // ���� : �� �� 2���� ����մϴ�.
                Debug.Log(player + "���� �ߵ�");
               
                break;

            case 12: // ��Ȱ��ǰ : �� ���� �� ī�带 �ֽ��ϴ�.
                Debug.Log(player + "��Ȱ��ǰ �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 13: // ���� : �� �и� ��� ������, �׸�ŭ ��� ���� �����ϴ�.
                Debug.Log(player + "���� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 14: // �и����� : �� �����Կ��� �������� ī�� 1���� �����ɴϴ�.
                Debug.Log(player + "�и����� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 15: // �ſ�ġ�� : ���� �Ͽ� ��밡 ����ߴ� ī����� ����մϴ�.
                Debug.Log(player + "�ſ�ġ�� �ߵ�");
                StartCoroutine(Delay3());

                break;

            case 16: // ��λ��� : ���� �� 1���� �̽��ϴ�.
                Debug.Log(player + "��λ��� �ߵ�");
                StartCoroutine(Delay3());
                AddCard(false);
                AddCard(true);
                break;

            case 17: // ���� ���� : ���� �� 1���� �����ϴ�.
                // ���� �߻� : P2�� ��� �� ��� ���ϴ� ī�� 1�� ����, P1�� ������ �ǳ� ���콺 �ø��� ����
                Debug.Log(player + "���� ���� �ߵ�");
                StartCoroutine(Delay3());

                // ��� �� ������
                var cardObject171 = Instantiate(cardPrefab, !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card171 = cardObject171.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card171.Setup(PopItem(!p1Turn), !p1Turn);  // �� or ��� ī�� �̱�
                Vector3 spawnPoint171 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, card171.item, spawnPoint171);

                var cardObject172 = Instantiate(cardPrefab, p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card172 = cardObject171.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card172.Setup(PopItem(p1Turn), p1Turn);  // �� or ��� ī�� �̱�
                Vector3 spawnPoint172 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(p1Turn, card172.item, spawnPoint172);

                /*
                 * 
                var cardObject171 = Instantiate(cardPrefab, !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card171 = cardObject171.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card171.Setup(PopItem(!p1Turn), !p1Turn);  // �� ����Ʈ ����
                Vector3 spawnPoint171 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, card171.item, spawnPoint171);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�

                // �� �� ������
                var cardObject172 = Instantiate(cardPrefab, p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card172 = cardObject172.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card172.Setup(PopItem(p1Turn), p1Turn);  // �� ����Ʈ ����
                Vector3 spawnPoint172 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(p1Turn, card172.item, spawnPoint172);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է�
                */
                break;

            case 18: // ��ȯ : ������ �п��� �������� 1���� �����ɴϴ�.
                Debug.Log(player + "��ȯ �ߵ�");
                StartCoroutine(Delay3());
                break;

            case 19: // ��û�� : ���� �и� ��� �����ϴ�. 
                Debug.Log(player + "��û�� �ߵ�");
                StartCoroutine(Delay3());
                break;

            case 20: // ���� �� : ���� �������� ��� �� 1���� ����մϴ�. 
                Debug.Log(player + "���� �� �ߵ�");
                StartCoroutine(Delay3());
                if ((p1Turn ? p2Hands : p1Hands).Count >= 1)   // ��� �а� ������
                {
                    Card card201 = p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];    // �� ī��� ������ ī��, ��� ī��� ī�� �� �ƹ��ų�
                    Vector3 spawnPoint201 = p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;
                    var targetCards201 = p1Turn ? p2Hands : p1Hands;

                    if (EntityManager.Inst.SpawnEntity(p1Turn, card201.item, spawnPoint201))    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                    {
                        targetCards201.Remove(card201);   // Ÿ�� ī��(��)���� ī�� ����
                        card201.transform.DOKill();    // ���ֹ���
                        DestroyImmediate(card201.gameObject);
                        CardEffect(card201.item.effectNumber, p1Turn, card201.item);
                        CardAlignment(!p1Turn);
                        return;
                    }
                }
                if ((!p1Turn ? p2Hands : p1Hands).Count <= 0)
                    return;

                Card card202 = !p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];    // �� ī��� ������ ī��, ��� ī��� ī�� �� �ƹ��ų�
                Vector3 spawnPoint202 = !p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;
                var targetCards202 = !p1Turn ? p2Hands : p1Hands;

                if (EntityManager.Inst.SpawnEntity(!p1Turn, card202.item, spawnPoint202))    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                {
                    targetCards202.Remove(card202);   // Ÿ�� ī��(��)���� ī�� ����
                    card202.transform.DOKill();    // ���ֹ���
                    DestroyImmediate(card202.gameObject);
                    CardEffect(card202.item.effectNumber, !p1Turn, card202.item);
                    CardAlignment(p1Turn);
                    return;
                }

                break;
                
            default:
                Debug.Log("ī�尡 ��Ͽ� �����ϴ�.");
                break;
        }
    }


    // #region ���콺 ����

#region P1Hands

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

        isP1HandDrag = true;
    }

    public void CardMouseUp()   // Ŭ�� ����
    {
        isP1HandDrag = false;

        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (onP1HandArea)
            return;
        else
            TryPutCard(true);
    }

    void CardDrag() // ī�� �巡�� �Լ�
    {
        if (!onP1HandArea)  // �� ī�� ���� �����
        {
            if (eCardState != ECardState.CanMouseDrag)  // ���콺 �巡�� ������ ���°� �ƴ϶��(�巡�� �Ұ�)
                return; // �״�� ��ȯ

            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false); // ī�� ������, ��Ʈ�� ���X
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
        int layer = LayerMask.NameToLayer("P1HandArea");    // �ν��ϴ� ���̾� �̸� "P1HandArea"
        onP1HandArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);   // onP1HandArea�� �浹�ϴ� ���� ����
    }

    void SetECardState()    // ī�� ���� ���� ����
    {
        if (TurnManager.Inst.isLoading) // �ε� ���̸�
            eCardState = ECardState.Nothing;    // ī�� ���� �ȵ�

        else if (!TurnManager.Inst.p1Turn || p1PutCount == maxCount)    // �� ���� �ƴϰų�, �̹� 1���� �°ų�, ��ƼƼ�� �� á����
            eCardState = ECardState.CanMouseOver;   // ī�� Ȯ�븸 ����

        else if (TurnManager.Inst.p1Turn && (p1PutCount < maxCount))    // �� ���̸鼭 ���� �� ī�尡 ������
            eCardState = ECardState.CanMouseDrag;   // ���콺 �巡�� ����
    }
    #endregion

   
}