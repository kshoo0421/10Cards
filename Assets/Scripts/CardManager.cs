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
    WaitForSeconds delay6 = new WaitForSeconds(6);  // delay2�� 2�� ���
    WaitForSeconds delay9 = new WaitForSeconds(9);  // delay2�� 2�� ���
   
    GameObject deckPercent;
    public List<Card> p1Hands;
    public List<Card> p2Hands;
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
        deckPercent = GameObject.Find("DeckPercent");
        p1Percent = deckPercent.GetComponent<DeckPercent>().p1Percent;
        p2Percent = deckPercent.GetComponent<DeckPercent>().p2Percent;
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
            Item item = p1DeckCount[0];
            p1DeckCount.RemoveAt(0);
            return item;
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
        for (int i = 0; i < itemSO.items.Length; i++) // �������� �� ����(20)��ŭ �ݺ�
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
            StartCoroutine(CardEffect(card.item.effectNumber, isMine, card.item));
            if (isMine)
            {
                selectCard = null;
                p1PutCount++;
            }
            else
                p2PutCount++;
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
    
    IEnumerator CardEffect(int effectNumber, bool p1Turn, Item item)
    {
        TurnManager.Inst.isLoading = true;
        string player = p1Turn ? "player1" : "player2";
        
        Vector3 effectPos = new Vector3(-30f, 0, 0);
        var effectObject = Instantiate(cardPrefab, effectPos, Utils.QI);
        var effectCard = effectObject.GetComponent<Card>();

        effectCard.Setup(item, true);
        // int entitiesCount = (p1Turn ? EntityManager.Inst.p1Entities : EntityManager.Inst.p2Entities).Count;
        effectCard.GetComponent<Order>().SetOrder(100);
        effectCard.MoveTransform(new PRS(effectPos, Utils.QI, Vector3.one * 17.5f), false);

        yield return delay1;
        switch (effectNumber)
        {
            case 1: // ����� : ��� �� 1���� �����ϴ�.
                Debug.Log(player + "����� �ߵ�");
                Vector3 spawnPoint01 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint01);
                break;
                
            case 2: // ���� : ��� �� 1���� �����ɴϴ�.
                Debug.Log(player + "���� �ߵ�");
                var cardObject02 = Instantiate(cardPrefab, p1Turn ?  p2HandSpawnPoint.position : p1HandSpawnPoint.position, Utils.QI);
                var card02 = cardObject02.GetComponent<Card>();
                card02.Setup(PopItem(!p1Turn), p1Turn); 
                (p1Turn ? p1Hands : p2Hands).Add(card02);

                SetOriginOrder(p1Turn);
                CardAlignment(p1Turn);

                break;

            case 3: // ���� : ��� �� 2���� ��밡 ����մϴ�. 
                Debug.Log(player + "���� �ߵ�");

                effectCard.transform.DOKill();    // ���ֹ���
                DestroyImmediate(effectCard.gameObject);

                Vector3 spawnPoint031 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                Item card031 = PopItem(!p1Turn);
                if (EntityManager.Inst.SpawnEntity(!p1Turn, card031, spawnPoint031))
                    StartCoroutine(CardEffect(card031.effectNumber, !p1Turn, card031));
                if (card031.effectNumber == 3 || card031.effectNumber == 11)
                    yield return delay9;
                else if (card031.effectNumber == 5 || card031.effectNumber == 10 || card031.effectNumber == 20)
                    yield return delay6;
                else yield return delay3;

                Vector3 spawnPoint032 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                Item card032 = PopItem(!p1Turn);
                if (EntityManager.Inst.SpawnEntity(!p1Turn, card032, spawnPoint032))
                    StartCoroutine(CardEffect(card032.effectNumber, !p1Turn, card032));
                if (card032.effectNumber == 3 || card032.effectNumber == 11)
                    yield return delay9;
                else if (card032.effectNumber == 5 || card032.effectNumber == 10 || card032.effectNumber == 20)
                    yield return delay6;
                else yield return delay3;


                break;
                
            case 4: // ���� : �������� ��� �� 1���� �����ϴ�.
                Debug.Log(player + "���� �ߵ�");
                if (p1Turn && (p1Turn ? p2Hands.Count : p1Hands.Count) <= 0)
                    break;

                Card card04 = p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];
                Vector3 spawnPoint04 = p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;
                var targetCards04 = p1Turn ? p2Hands : p1Hands;

                if (EntityManager.Inst.SpawnEntity(!p1Turn, card04.item, spawnPoint04))
                {
                    targetCards04.Remove(card04);
                    card04.transform.DOKill();
                    DestroyImmediate(card04.gameObject);
                    card04 = null;
                    CardAlignment(!p1Turn);
                }
                break;

            case 5: // �Ƽ� : �������� ��� �� 1���� ���� ����մϴ�.
                Debug.Log(player + "�Ƽ� �ߵ�");
                if ((p1Turn ? p2Hands : p1Hands).Count <= 0)
                    break;   // ��ȯ
                effectCard.transform.DOKill();    // ���ֹ���
                DestroyImmediate(effectCard.gameObject);

                Card card05 = p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];
                Vector3 spawnPoint05 = p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;
                var targetCards05 = p1Turn ? p2Hands : p1Hands; 

                if (EntityManager.Inst.SpawnEntity(p1Turn, card05.item, spawnPoint05))
                {
                    targetCards05.Remove(card05);
                    card05.transform.DOKill();
                    DestroyImmediate(card05.gameObject);
                    StartCoroutine(CardEffect(card05.item.effectNumber, p1Turn, card05.item));
                    if (card05.item.effectNumber == 3 || card05.item.effectNumber == 11)
                        yield return delay9;
                    else if (card05.item.effectNumber == 5 || card05.item.effectNumber == 10 || card05.item.effectNumber == 20)
                        yield return delay6;
                    else yield return delay3;
                    CardAlignment(!p1Turn);
                }
                break;

            case 6: // ����̱� : ��� �� 1���� �����ɴϴ�.
                Debug.Log(player + "����̱� �ߵ�");
                
                Card card06 = p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];
                Vector3 spawnPoint06 = p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;

                card06.Setup(card06.item, p1Turn);
                (p1Turn ? p1Hands : p2Hands).Add(card06);
                (!p1Turn ? p1Hands : p2Hands).Remove(card06);

                SetOriginOrder(p1Turn);
                SetOriginOrder(!p1Turn);
                CardAlignment(p1Turn);
                CardAlignment(!p1Turn);
                break;

            case 7: // ���� : ���� ���� �Ͽ� 1�常 ����� �� �ֽ��ϴ�.
                Debug.Log(player + "���� �ߵ�");
                if (cardEffect07 == true)
                    TurnManager.Inst.turnCount = 1;
                else
                    cardEffect07 = true;
                break;

            case 8: // ���� : ���� �и� 2�� �̽��ϴ�. 
                Debug.Log(player + "���� �ߵ�");
                AddCard(false);
                AddCard(false);
                AddCard(true);
                AddCard(true);
                break;

            case 9: // �޸��� : �� �� 2���� �̽��ϴ�.
                Debug.Log(player + "�޸��� �ߵ�");
                AddCard(p1Turn);
                AddCard(p1Turn);
                break;

            case 10: // ���� ���� : �� �� 1���� ����մϴ�. 
                Debug.Log(player + "���� ���� �ߵ�");
                effectCard.transform.DOKill();    // ���ֹ���
                DestroyImmediate(effectCard.gameObject);

                Vector3 spawnPoint10 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                Item card10 = PopItem(p1Turn);
                if(EntityManager.Inst.SpawnEntity(p1Turn, card10, spawnPoint10))
                    StartCoroutine(CardEffect(card10.effectNumber, p1Turn, card10));
                if (card10.effectNumber == 3 || card10.effectNumber == 11)
                    yield return delay9;
                else if (card10.effectNumber == 5 || card10.effectNumber == 10 || card10.effectNumber == 20)
                    yield return delay6;
                else yield return delay3;

                break; 
            
            case 11: // ���� : �� �� 2���� ����մϴ�.
                Debug.Log(player + "���� �ߵ�");
                effectCard.transform.DOKill();    // ���ֹ���
                DestroyImmediate(effectCard.gameObject);

                Vector3 spawnPoint111 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                Item card111 = PopItem(p1Turn);
                if (EntityManager.Inst.SpawnEntity(p1Turn, card111, spawnPoint111))
                    StartCoroutine(CardEffect(card111.effectNumber, p1Turn, card111));
                if (card111.effectNumber == 3 || card111.effectNumber == 11)
                    yield return delay9;
                else if (card111.effectNumber == 5 || card111.effectNumber == 10 || card111.effectNumber == 20)
                    yield return delay6;
                else yield return delay3;

                Vector3 spawnPoint112 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                Item card112 = PopItem(p1Turn);
                if (EntityManager.Inst.SpawnEntity(p1Turn, card112, spawnPoint112))
                    StartCoroutine(CardEffect(card112.effectNumber, p1Turn, card112));
                if (card112.effectNumber == 3 || card112.effectNumber == 11)
                    yield return delay9;
                else if (card112.effectNumber == 5 || card112.effectNumber == 10 || card112.effectNumber == 20)
                    yield return delay6;
                else yield return delay3;
                break;

            case 12: // ���� : �� �и� 1�� ������ ��� �� 2���� �����ϴ�.
                Debug.Log(player + "���� �ߵ�");
                if ((p1Turn ? p2Hands.Count : p1Hands.Count) >= 1)
                {
                    Card card12 = p1Turn ? p1Hands[Random.Range(0, p1Hands.Count)] : p2Hands[Random.Range(0, p2Hands.Count)];
                    Vector3 spawnPoint121 = p1Turn ? p1EntitySpawnPoint.position : p2EntitySpawnPoint.position;
                    var targetCards12 = p1Turn ? p1Hands : p2Hands;

                    if (EntityManager.Inst.SpawnEntity(p1Turn, card12.item, spawnPoint121))
                    {
                        targetCards12.Remove(card12);
                        card12.transform.DOKill();
                        DestroyImmediate(card12.gameObject);
                        card12 = null;
                        CardAlignment(p1Turn);
                    }
                }

                Vector3 spawnPoint122 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint122);
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint122);

                break;

            case 13: // ���� : �� �и� ��� ������, �׸�ŭ ��� ���� �����ϴ�.
                Debug.Log(player + "���� �ߵ�");
                
                int HandsCount13 = p1Turn ? p1Hands.Count : p2Hands.Count;
                Vector3 spawnPoint131 = p1Turn ? p1EntitySpawnPoint.position : p2EntitySpawnPoint.position;
                var targetCards13 = p1Turn ? p1Hands : p2Hands;

                for (int i = 0; i < HandsCount13; i++)
                {
                    Card card13 = p1Turn ? p1Hands[HandsCount13 - i - 1] : p2Hands[HandsCount13 - i - 1];

                    if (EntityManager.Inst.SpawnEntity(p1Turn, card13.item, spawnPoint131))
                    {
                       targetCards13.Remove(card13);
                        card13.transform.DOKill(); 
                        DestroyImmediate(card13.gameObject);
                        card13 = null;
                        CardAlignment(p1Turn);
                    }
                }
                for (int i = 0; i < HandsCount13; i++)
                {
                    Vector3 spawnPoint132 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                    EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint132);
                }
                break;

            case 14: // ���� : �� ���� 1�� ������ ��� �� 2���� �����ϴ�. 
                Debug.Log(player + "���� �ߵ�");
                Vector3 spawnPoint141 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(p1Turn, PopItem(p1Turn), spawnPoint141);
                
                Vector3 spawnPoint142 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint142);
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint142);
                break;

            case 15: // �纸 : ���� �� 1���� �������ϴ�. 
                Debug.Log(player + "�纸 �ߵ�");
                var cardObject151 = Instantiate(cardPrefab, p1Turn ? p2HandSpawnPoint.position : p1HandSpawnPoint.position, Utils.QI);
                var card151 = cardObject151.GetComponent<Card>();
                card151.Setup(PopItem(!p1Turn), p1Turn);
                (p1Turn ? p1Hands : p2Hands).Add(card151);

                var cardObject152 = Instantiate(cardPrefab, p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI);
                var card152 = cardObject152.GetComponent<Card>();
                card152.Setup(PopItem(p1Turn), !p1Turn);
                (!p1Turn ? p1Hands : p2Hands).Add(card152);

                SetOriginOrder(p1Turn);
                CardAlignment(p1Turn);

                SetOriginOrder(!p1Turn);
                CardAlignment(!p1Turn);
                break;

            case 16: // ��λ��� : ���� �� 1���� �̽��ϴ�.
                Debug.Log(player + "��λ��� �ߵ�");
                AddCard(false);
                AddCard(true);
                break;

            case 17: // ���� ���� : ���� �� 1���� �����ϴ�.
                Debug.Log(player + "���� ���� �ߵ�");
                Vector3 spawnPoint171 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint171);
                Vector3 spawnPoint172 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(p1Turn, PopItem(p1Turn), spawnPoint172);
                break;

            case 18: // ��ȯ : ������ �п��� �������� 1���� �����ɴϴ�.
                Debug.Log(player + "��ȯ �ߵ�");
                if ((p1Turn ? p2Hands.Count : p1Hands.Count) >= 1)
                {
                    Card card181 = p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];
                    Vector3 spawnPoint181 = p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;

                    card181.Setup(card181.item, p1Turn);
                    (p1Turn ? p1Hands : p2Hands).Add(card181); 
                    (!p1Turn ? p1Hands : p2Hands).Remove(card181);
                }
                if ((p1Turn ? p1Hands.Count : p2Hands.Count) >= 1)
                {
                    Card card182 = !p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];
                    Vector3 spawnPoint182 = !p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;

                    card182.Setup(card182.item, !p1Turn);
                    (!p1Turn ? p1Hands : p2Hands).Add(card182); 
                    (p1Turn ? p1Hands : p2Hands).Remove(card182); 
                }
                SetOriginOrder(p1Turn);
                SetOriginOrder(!p1Turn);
                CardAlignment(p1Turn);
                CardAlignment(!p1Turn);
                break;

            case 19: // ��û�� : ���� �и� ��� �����ϴ�. 
                Debug.Log(player + "��û�� �ߵ�");
                
                int HandsCount191 = p1Turn ? p1Hands.Count : p2Hands.Count;
                Vector3 spawnPoint191 = p1Turn ? p1EntitySpawnPoint.position : p2EntitySpawnPoint.position;
                var targetCards191 = p1Turn ? p1Hands : p2Hands;

                for (int i = 0; i < HandsCount191; i++)
                {
                    Card card191 = p1Turn ? p1Hands[HandsCount191 - i - 1] : p2Hands[HandsCount191 - i - 1];

                    if (EntityManager.Inst.SpawnEntity(p1Turn, card191.item, spawnPoint191))
                    {
                        targetCards191.Remove(card191);
                        card191.transform.DOKill(); 
                        DestroyImmediate(card191.gameObject);
                        card191 = null;
                        CardAlignment(p1Turn);
                    }
                }

                int HandsCount192 = !p1Turn ? p1Hands.Count : p2Hands.Count;
                Vector3 spawnPoint192 = !p1Turn ? p1EntitySpawnPoint.position : p2EntitySpawnPoint.position;
                var targetCards192 = !p1Turn ? p1Hands : p2Hands;

                for (int i = 0; i < HandsCount192; i++)
                {
                    Card card192 = !p1Turn ? p1Hands[HandsCount192 - i - 1] : p2Hands[HandsCount192 - i - 1];

                    if (EntityManager.Inst.SpawnEntity(!p1Turn, card192.item, spawnPoint192))
                    {
                        targetCards192.Remove(card192);
                        card192.transform.DOKill(); 
                        DestroyImmediate(card192.gameObject);
                        card192 = null;
                        CardAlignment(!p1Turn);
                    }
                }
                break;

            case 20: // ���� �� : ���� �������� ��� �� 1���� ����մϴ�. 
                Debug.Log(player + "���� �� �ߵ�");
                effectCard.transform.DOKill();    // ���ֹ���
                DestroyImmediate(effectCard.gameObject);

                if ((p1Turn ? p2Hands : p1Hands).Count >= 1)
                {
                    Card card201 = p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];
                    Vector3 spawnPoint201 = p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;
                    var targetCards201 = p1Turn ? p2Hands : p1Hands;

                    if (EntityManager.Inst.SpawnEntity(p1Turn, card201.item, spawnPoint201)) 
                    {
                        targetCards201.Remove(card201); 
                        card201.transform.DOKill();  
                        DestroyImmediate(card201.gameObject);
                        StartCoroutine(CardEffect(card201.item.effectNumber, p1Turn, card201.item));
                        if (card201.item.effectNumber == 3 || card201.item.effectNumber == 11)
                            yield return delay9;
                        else if (card201.item.effectNumber == 5 || card201.item.effectNumber == 10 || card201.item.effectNumber == 20)
                            yield return delay6;
                        else yield return delay3;
                        CardAlignment(!p1Turn);
                    }
                }
                if ((!p1Turn ? p2Hands : p1Hands).Count <= 0)
                    break;

                Card card202 = !p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];   
                Vector3 spawnPoint202 = !p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;
                var targetCards202 = !p1Turn ? p2Hands : p1Hands;

                if (EntityManager.Inst.SpawnEntity(!p1Turn, card202.item, spawnPoint202)) 
                {
                    targetCards202.Remove(card202);
                    card202.transform.DOKill();  
                    DestroyImmediate(card202.gameObject);
                    StartCoroutine(CardEffect(card202.item.effectNumber, !p1Turn, card202.item));
                    if (card202.item.effectNumber == 3 || card202.item.effectNumber == 11)
                        yield return delay9;
                    else if (card202.item.effectNumber == 5 || card202.item.effectNumber == 10 || card202.item.effectNumber == 20)
                        yield return delay6;
                    else yield return delay3;
                    CardAlignment(p1Turn);
                }
                break;
            default:
                Debug.Log("ī�尡 ��Ͽ� �����ϴ�.");
                break;
        }
        yield return delay1;

        if (effectNumber != 3 && effectNumber != 5 && effectNumber != 10 && effectNumber != 11 && effectNumber != 20)
        {
            effectCard.transform.DOKill();    // ���ֹ���
            DestroyImmediate(effectCard.gameObject);
        }

        TurnManager.Inst.isLoading = false;
        CurrentCardNumbering(p1Turn);
        CardAlignment(p1Turn);
        yield return delay1;
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

            card.MoveTransform(new PRS(enlargePos, Utils.QI, Vector3.one * 17.5f), false);
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

        else
            return;
    }
    #endregion

   
}