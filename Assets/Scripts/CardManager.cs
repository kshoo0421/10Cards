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
    [SerializeField] TMP_Text TurnCardTMP;  // �� ī�� �� TMP
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
    int p1PutCount; // �� �Ͽ� ī�� ���� ����
    public bool[] p1Percent;    // �� �� ����
    public bool[] p2Percent; // ��� �� ����

    // ���� ����

    void Start()    // �� ���� �� ������ ����, AddCard, OnTurnStarted ȣ��
    {
        SetupItemBuffer();  // �� ���� �� ������ ����
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
            if(per == true)
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
        if (p1Turn == true) // �� ���̸�
            p1PutCount = 0; // ī�� ���� �� ����
    }
   
    public bool TryPutCard(bool isMine) // ī�� ���� �Լ�
    {
        if (isMine && p1PutCount >= 2)  // �� ī���ε�, �̹� �� ī�尡 2�� ������
            return false;   // false ��ȯ
        if (!isMine && p2Hands.Count <= 0)   // ��� ī���ε�, ��� ī�� �а� 0�̶��
            return false;   // false ��ȯ

        Card card = isMine ? selectCard : p2Hands[Random.Range(0, p2Hands.Count)];    // �� ī��� ������ ī��, ��� ī��� ī�� �� �ƹ��ų�
        Vector3 spawnPoint = isMine ? Utils.MousePos : p2EntitySpawnPoint.position;
        var targetCards = isMine ? p1Hands : p2Hands;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

        if (EntityManager.Inst.SpawnEntity(isMine, card.item, spawnPoint))    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
        {
            targetCards.Remove(card);   // Ÿ�� ī��(��)���� ī�� ����
            CardEffect(card.item.effectNumber, isMine, card.item);
            card.transform.DOKill();    // ���ֹ���
            DestroyImmediate(card.gameObject);
            if (isMine)
            {
                selectCard = null;
                p1PutCount++;
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
        p1DeckTMP.text = this.p1DeckCount.Count.ToString();
        p2DeckTMP.text = this.p2DeckCount.Count.ToString();
        
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
        TurnCardTMP.text = this.p1PutCount.ToString();
    }

    IEnumerator Delay3()  // ��� AI ����
    {
        Debug.Log("3�ʰ� ����մϴ�.");
        yield return delay3;
    }

    void CardEffect(int effectNumber, bool p1Turn, Item item)
    {
        switch (effectNumber)
        {
            case 1: // ����� : ��� �� 1���� �����ϴ�.
                Debug.Log("����� �ߵ�");
                StartCoroutine(Delay3());
                var cardObject01 = Instantiate(cardPrefab, !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card01 = cardObject01.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card01.Setup(PopItem(!p1Turn), !p1Turn);  // �� or ��� ī�� �̱�
                (!p1Turn ? p1Hands : p1Hands).Add(card01);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                SetOriginOrder(!p1Turn); // ī�� ���̾� ���� ����
                
                Vector3 spawnPoint01 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                var targetCards01 = !p1Turn ? p1Hands : p2Hands;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

                EntityManager.Inst.SpawnEntity(!p1Turn, card01.item, spawnPoint01);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                targetCards01.Remove(card01);   // Ÿ�� ī��(��)���� ī�� ����
                card01.transform.DOKill();    // ���ֹ���
                DestroyImmediate(card01.gameObject);
                
                break;
                
            case 2: // ���� : ��� �� 1���� �����ɴϴ�.
                Debug.Log("���� �ߵ�");
                StartCoroutine(Delay3());

                var cardObject02 = Instantiate(cardPrefab, p1Turn ?  p2HandSpawnPoint.position : p1HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card02 = cardObject02.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card02.Setup(PopItem(!p1Turn), p1Turn);  // �� or ��� ī�� �̱�
                (p1Turn ? p1Hands : p2Hands).Add(card02);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                SetOriginOrder(p1Turn); // ī�� ���̾� ���� ����
                CardAlignment(p1Turn);  // ī��� ��ġ ����

                break;

            case 3: // ���� : ��� �� 2���� ��밡 ����մϴ�.
                Debug.Log("���� �ߵ�");
                StartCoroutine(Delay3());

                for (int i = 0; i < 2; i++)
                {
                    var cardObject03 = Instantiate(cardPrefab, !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                    var card03 = cardObject03.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                    card03.Setup(PopItem(!p1Turn), !p1Turn);  // �� or ��� ī�� �̱�
                    (!p1Turn ? p1Hands : p2Hands).Add(card03);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                    SetOriginOrder(!p1Turn); // ī�� ���̾� ���� ����

                    Vector3 spawnPoint03 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                    var targetCards03 = !p1Turn ? p1Hands : p2Hands;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

                    EntityManager.Inst.SpawnEntity(!p1Turn, card03.item, spawnPoint03);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�

                    targetCards03.Remove(card03);   // Ÿ�� ī��(��)���� ī�� ����
                    Delay3();
                    CardEffect(card03.item.effectNumber, !p1Turn, card03.item);
                    card03.transform.DOKill();    // ���ֹ���
                    DestroyImmediate(card03.gameObject);
                }
                break;
                
            case 4: // ���� : �������� ��� �� 1���� �����ϴ�.
                Debug.Log("���� �ߵ�");
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
                    if (p1Turn)
                        selectCard = null;
                    CardAlignment(!p1Turn);
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
                AddCard(p1Turn);
                AddCard(p1Turn);
                break;

            case 10: // ���� ���� : �� �� 1���� ����մϴ�. 
                Debug.Log("���� ���� �ߵ�");
                StartCoroutine(Delay3());
                var cardObject10 = Instantiate(cardPrefab, p1Turn ? p1HandSpawnPoint.position : p1HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                var card10 = cardObject10.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                card10.Setup(PopItem(p1Turn), p1Turn);  // �� or ��� ī�� �̱�
                (p1Turn ? p1Hands : p2Hands).Add(card10);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                SetOriginOrder(p1Turn); // ī�� ���̾� ���� ����

                Vector3 spawnPoint10 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                var targetCards10 = p1Turn ? p1Hands : p2Hands;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

                EntityManager.Inst.SpawnEntity(!p1Turn, card10.item, spawnPoint10);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                targetCards10.Remove(card10);   // Ÿ�� ī��(��)���� ī�� ����
                Delay3();
                CardEffect(card10.item.effectNumber, p1Turn, card10.item);
                card10.transform.DOKill();    // ���ֹ���
                DestroyImmediate(card10.gameObject);

                break; 
            
            case 11: // ���� : �� �� 2���� ����մϴ�.
                Debug.Log("���� �ߵ�");
                StartCoroutine(Delay3());
                for (int i = 0; i<2; i++)
                {
                    var cardObject11 = Instantiate(cardPrefab, p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject������ (ī�� ��Ʈ, ī�� ������ ��ġ, ����)�� ������ ���� 
                    var card11 = cardObject11.GetComponent<Card>(); // card�� Card ��ũ��Ʈ ������ ����
                    card11.Setup(PopItem(p1Turn), p1Turn);  // �� or ��� ī�� �̱�
                    (p1Turn ? p1Hands : p2Hands).Add(card11);  // ������ �� ī��, �ƴϸ� ��� ī�� �߰�

                    SetOriginOrder(p1Turn); // ī�� ���̾� ���� ����

                    Vector3 spawnPoint11 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                    var targetCards11 = p1Turn ? p1Hands : p2Hands;    // Ÿ�� ī��� �� ���ʸ� �� ī���(��), �ƴϸ� ��� ī���(��)

                    EntityManager.Inst.SpawnEntity(!p1Turn, card11.item, spawnPoint11);    // ī�� �ߵ�; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                    targetCards11.Remove(card11);   // Ÿ�� ī��(��)���� ī�� ����
                    Delay3();
                    CardEffect(card11.item.effectNumber, p1Turn, card11.item);
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
                int p2HandCount = p2Hands.Count;
                Debug.Log(p2HandCount);
                for (int i = 0; i <= p2HandCount; i++)    // ��� ������
                {
                    Card card191 = p1Turn ? p2Hands[Random.Range(0, p2Hands.Count)] : p1Hands[Random.Range(0, p1Hands.Count)];    // �� ���̸� ��� �� ������, ��� ���̸� �� �� ������
                    Vector3 spawnPoint191 = p1Turn ? p2EntitySpawnPoint.position : p1EntitySpawnPoint.position;
                    var targetCards191 = p1Turn ? p2Cards : p1Hands;    // Ÿ�� ī��� �� ���ʸ� ��� ��, �ƴϸ� �� ��

                    if (EntityManager.Inst.SpawnEntity(!p1Turn, card191.item, spawnPoint191))    // ī�� ����; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                    {
                        targetCards191.Remove(card191);   // �п��� ī�� ����
                        card191.transform.DOKill();    // ���ֹ���
                        DestroyImmediate(card191.gameObject);
                        if (p1Turn)
                            selectCard = null;
                        CardAlignment(!p1Turn);
                    }
                }
                Debug.Log("��� ��û�� ��");

                int p1HandCount = p1Hands.Count-1;
                Debug.Log(p1HandCount);
                for (int i = 0; i <= p1HandCount; i++)    // ��� ������
                {
                    Card card192 = p1Turn ? p1Hands[Random.Range(0, p1Hands.Count)] : p2Hands[Random.Range(0, p2Hands.Count)];    // �� ���̸� �� �� ������, ��� ���̸� ��� �� ������
                    Vector3 spawnPoint192 = p1Turn ?  p1EntitySpawnPoint.position : p2EntitySpawnPoint.position;
                    var targetCards192 = p1Turn ? p1Hands : p2Cards;    // Ÿ�� ī��� �� ���ʸ� ��� ��, �ƴϸ� �� ��

                    if (EntityManager.Inst.SpawnEntity(p1Turn, card192.item, spawnPoint192))    // ī�� ����; ��, ������ ����, ���� ��ġ �Է��� �Ǹ�
                    {
                        targetCards192.Remove(card192);   // �п��� ī�� ����
                        card192.transform.DOKill();    // ���ֹ���
                        DestroyImmediate(card192.gameObject);
                        if (p1Turn)
                            selectCard = null;
                        CardAlignment(p1Turn);
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
            EntityManager.Inst.RemoveP1EmptyEntity();

        if (!onP1HandArea)
            TryPutCard(true);
    }

    void CardDrag() // ī�� �巡�� �Լ�
    {
        if (!onP1HandArea)  // �� ī�� ���� �����
        {
            if (eCardState != ECardState.CanMouseDrag)  // ���콺 �巡�� ������ ���°� �ƴ϶��(�巡�� �Ұ�)
                return; // �״�� ��ȯ

            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false); // ī�� ������, ��Ʈ�� ���X
            EntityManager.Inst.InsertP1EmptyEntity(Utils.MousePos.x);   // x�࿡ ���� �� ��ƼƼ ����(��ġ ��� �뵵)
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

        else if (!TurnManager.Inst.p1Turn || p1PutCount == 2)    // �� ���� �ƴϰų�, �̹� 1���� �°ų�, ��ƼƼ�� �� á����
            eCardState = ECardState.CanMouseOver;   // ī�� Ȯ�븸 ����

        else if (TurnManager.Inst.p1Turn && (p1PutCount == 0 || p1PutCount == 1))    // �� ���̸鼭 ���� �� ī�尡 ������
            eCardState = ECardState.CanMouseDrag;   // ���콺 �巡�� ����
    }
    #endregion

   
}