using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // TMP 사용
using Random = UnityEngine.Random;  //System과 UnityEngine의 Random이 겹쳐서 UnityEngine의 Random을 사용하겠다는 의미
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    private void Awake() => Inst = this;

    [SerializeField] ECardState eCardState; // 마우스 클릭/드래그 여부 확인용
    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform p1HandSpawnPoint;
    [SerializeField] Transform p2HandSpawnPoint;
    [SerializeField] Transform p1HandLeft;
    [SerializeField] Transform p1HandRight;
    [SerializeField] Transform p2HandLeft;
    [SerializeField] Transform p2HandRight;
    [SerializeField] TMP_Text p1DeckTMP;  // 내 덱 TMP
    [SerializeField] TMP_Text p2DeckTMP;  // 상대 덱 TMP
    [SerializeField] Transform p1EntitySpawnPoint;
    [SerializeField] Transform p2EntitySpawnPoint;
    [SerializeField] TMP_Text currentCardTMP;
    [SerializeField] TMP_Text MaxCardTMP;  // 최대 카드 수 TMP
    [SerializeField] GameObject p1BackCard;  // 낸 카드 수 TMP
    [SerializeField] GameObject p2BackCard;  // 최대 카드 수 TMP

    WaitForSeconds delay1 = new WaitForSeconds(1);  // delay1은 1초 대기
    WaitForSeconds delay2 = new WaitForSeconds(2);  // delay2는 2초 대기
    WaitForSeconds delay3 = new WaitForSeconds(3);  // delay2는 2초 대기
    WaitForSeconds delay6 = new WaitForSeconds(6);  // delay2는 2초 대기
    WaitForSeconds delay9 = new WaitForSeconds(9);  // delay2는 2초 대기
   
    GameObject deckPercent;
    public List<Card> p1Hands;
    public List<Card> p2Hands;
    public List<Item> p1DeckCount;  // 아이템 버퍼 리스트 선언
    public List<Item> p2DeckCount;  // 아이템 버퍼 리스트 선언
    Card selectCard;    // 선택한 카드 변수 선언
    bool isP1HandDrag;  // 내 카드 드래그 여부 변수 선언
    bool onP1HandArea;  // 내 카드 영역(카드 확대 영역)
    enum ECardState { Nothing, CanMouseOver, CanMouseDrag } // ECardState는 1. 아무 것도 안되는 경우, 2. 확대만 되는 경우, 3. 드래그까지 되는 경우로 나뉨
    public int maxCount;
    public int p1PutCount; // 내 턴에 카드 놓기 제한
    public int p2PutCount; // 상대 턴에 카드 놓기 제한
    public bool[] p1Percent;    // 내 덱 구성
    public bool[] p2Percent; // 상대 덱 구성
    public bool cardEffect07;
    
    // 게임 진행
    void Start()    // 덱 조정 및 아이템 섞기, AddCard, OnTurnStarted 호출
    {
        deckPercent = GameObject.Find("DeckPercent");
        p1Percent = deckPercent.GetComponent<DeckPercent>().p1Percent;
        p2Percent = deckPercent.GetComponent<DeckPercent>().p2Percent;
        SetupItemBuffer();  // 덱 조정 및 아이템 섞기
        maxCount = 1;
        p1PutCount = 0;
        p2PutCount = 0;
        cardEffect07 = false;
        TurnManager.OnAddCard += AddCard;   // TurnManager가 OnAddCard를 호출하면 AddCard함수 호출
        TurnManager.OnTurnStarted += OnTurnStarted; // TurnManager가 OnTurnStarted를 호출하면 OnTurnStarted함수 호출
    }

    void Update()   // 드래그, DetectCardArea, 카드 선택 가능 유무
    {
        if (isP1HandDrag)   // 내 카드 드래그 O
            CardDrag(); // 카드 드래그

        DetectCardArea();   // 카드 범위 인식

        SetECardState();    // 카드 선택 가능 유무

        CountNumbering();   // 필드에 숫자 표시
    }

    void OnDestroy()
    {
        TurnManager.OnAddCard -= AddCard;   // TurnManager가 OnAddCard 호출 취소하면 AddCard 함수 호출 취소
        TurnManager.OnTurnStarted -= OnTurnStarted; // TurnManager가 OnTurnStarted 호출 취소하면 OnTurnStarted 함수 호출 취소
    }

    // 관련 함수
    void AddCard(bool isMine)   // 내 카드인지 아닌지
    {
        var cardObject = Instantiate(cardPrefab, isMine ? p1HandSpawnPoint.position : p2HandSpawnPoint.position, Utils.QI); // cardObject변수는 (카드 세트, 카드 리스폰 위치, 각도)의 정보를 가짐 
        var card = cardObject.GetComponent<Card>(); // card는 Card 스크립트 내용의 변수
        card.Setup(PopItem(isMine), isMine);  // 나 or 상대 카드 뽑기
        (isMine ? p1Hands : p2Hands).Add(card);  // 내꺼면 내 카드, 아니면 상대 카드 추가

        SetOriginOrder(isMine); // 카드 레이어 순서 정렬
        CardAlignment(isMine);  // 카드들 위치 정렬
    }

    void SetOriginOrder(bool isMine)    // 레이어 정렬
    {
        int Count = isMine ? p1Hands.Count : p2Hands.Count; // 내 패 총 수 or 상대 카드 총 수
        for (int i = 0; i < Count; i++)  // 가진 카드(패) 전체 레이어 정렬
        {
            var targetCard = isMine ? p1Hands[i] : p2Hands[i];   // 내 카드 or 상대 카드
            targetCard?.GetComponent<Order>().SetOriginOrder(i);    // ?는 Nullable(Null값 사용 가능); 카드들 위치 정렬
        }
    }

    void CardAlignment(bool isMine) // 패 위치 정렬(내 카드 or 상대 위치)
    {

        List<PRS> originCardPRSs = new List<PRS>(); // 카드 리스트의 위치, 회전, 크기
        if (isMine) // 내 카드
            originCardPRSs = RoundAlignment(p1HandLeft, p1HandRight, p1Hands.Count, 0.5f, Vector3.one * 10f);  // 왼오 위치, 카드(패) 수, 상한점, 크기
        else     // 상대 카드
            originCardPRSs = RoundAlignment(p2HandLeft, p2HandRight, p2Hands.Count, -0.5f, Vector3.one * 10f);    // 위와 동일


        var targetCards = isMine ? p1Hands : p2Hands;    // 손패(나 or 상대)
        for (int i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];

            targetCard.originPRS = originCardPRSs[i];   // 손패 카드들의 위치
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f); // 이 속도로 움직이기
        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)    // 카드 리스트의 원형 위치 정렬
    {
        float[] objLerps = new float[objCount]; // 리스트로 정렬
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break; // 1장 위치
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break; // 2장 위치 
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break; // 3장 위치
            default:    // 그 이외
                float interval = 1f / (objCount - 1);   // 간격 = 1 / (총 갯수 -1)
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i; // 각 카드들의 x축 위치 지정
                break;
        }

        for (int i = 0; i < objCount; i++)    // y축 지정
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);   // 위치 : 선형으로 이동
            var targetRot = Utils.QI;   // 회전
            if (objCount >= 4)   // 4개 이상일 때
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));  // curve값 : 원의 방정식 통해 도출
                curve = height >= 0 ? curve : -curve;   // 높이를 절대값으로
                targetPos.y += curve;   // 
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;
    }

    public Item PopItem(bool isMine) // 리스트에서 뽑기(제거)
    {
        if (isMine == true)
        {
            Item item = p1DeckCount[0];
            p1DeckCount.RemoveAt(0);
            return item;
        }
        else
        {
            Item item = p2DeckCount[0];  // 아이템이 첫 번째순서면
            p2DeckCount.RemoveAt(0); // 아이템 버퍼에서 첫 번째 카드 제거
            return item;    // 함수 종료
        }
    }

    void SetupItemBuffer() // 아이템 버퍼 세팅
    {
        p1DeckCount = new List<Item>(20);   // 아이템 버퍼는 아이템 리스트(20개)의 리스트(새 리스트)

        for (int i = 0; i < 20; i++) // 아이템의 총 갯수(20)만큼 반복
        {
            Item item = itemSO.items[i];    // item = 아이템리스트의 i번째 아이템
            bool per = p1Percent[i];
            if (per == true)
                p1DeckCount.Add(item);   // 리스트에 카드 추가
        }

        for (int i = 0; i < p1DeckCount.Count; i++)    // 내꺼 아이템 섞기; 아이템 버퍼의 갯수만큼
        {
            int rand = Random.Range(i, p1DeckCount.Count);   // rand는 총 갯수보다 적은 수 중 랜덤값
            Item temp = p1DeckCount[i];  // i번째 아이템 임시 저장
            p1DeckCount[i] = p1DeckCount[rand];   // 랜덤 위치의 아이템을 i번째로
            p1DeckCount[rand] = temp;    // i번째 아이템은 그 랜덤위치로 바꿈
        }

        p2DeckCount = new List<Item>(20);
        for (int i = 0; i < itemSO.items.Length; i++) // 아이템의 총 갯수(20)만큼 반복
        {
            Item item = itemSO.items[i];    // item = 아이템리스트의 i번째 아이템
            bool per = p2Percent[i];
            if (per == true)
                p2DeckCount.Add(item);   // 리스트에 카드 추가
        }

        for (int i = 0; i < p2DeckCount.Count; i++)    // 상대꺼
        {
            int rand = Random.Range(i, p2DeckCount.Count);
            Item temp = p2DeckCount[i];
            p2DeckCount[i] = p2DeckCount[rand];
            p2DeckCount[rand] = temp;
        }
    }

    void OnTurnStarted(bool p1Turn) // 턴 시작 시
    {
        CurrentCardNumbering(p1Turn);
        if (cardEffect07 == true)
            maxCount = 1;
    }

    public bool TryPutCard(bool isMine) // 카드 놓기 함수
    {
        if (isMine && p1PutCount >= maxCount)  // 내 카드인데, 이미 낸 카드가 2를 넘으면
            return false;   // false 반환
        if ((!isMine && p2Hands.Count <= 0) || (!isMine && p2PutCount >= maxCount))   // 상대 카드인데, 상대 카드 패가 0이라면
            return false;   // false 반환
        Card card = isMine ? selectCard : p2Hands[Random.Range(0, p2Hands.Count)];    // 내 카드면 선택한 카드, 상대 카드면 카드 중 아무거나
        Vector3 spawnPoint = isMine ? Utils.MousePos : p2EntitySpawnPoint.position;
        var targetCards = isMine ? p1Hands : p2Hands;    // 타겟 카드는 내 차례면 내 카드들(패), 아니면 상대 카드들(패)
        if (EntityManager.Inst.SpawnEntity(isMine, card.item, spawnPoint))    // 카드 발동; 턴, 아이템 내용, 스폰 위치 입력이 되면
        {
            targetCards.Remove(card);   // 타겟 카드(패)에서 카드 삭제
            card.transform.DOKill();    // 없애버림
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
        else   // 패로 되돌아가기
        {
            targetCards.ForEach(x => x.GetComponent<Order>().SetMostFrontOrder(false));
            CardAlignment(isMine);
            return false;
        }
    }
        
    public void CountNumbering()   // 덱 카운트
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
            case 1: // 뒤통수 : 상대 덱 1장을 버립니다.
                Debug.Log(player + "뒤통수 발동");
                Vector3 spawnPoint01 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint01);
                break;
                
            case 2: // 도벽 : 상대 덱 1장을 가져옵니다.
                Debug.Log(player + "도벽 발동");
                var cardObject02 = Instantiate(cardPrefab, p1Turn ?  p2HandSpawnPoint.position : p1HandSpawnPoint.position, Utils.QI);
                var card02 = cardObject02.GetComponent<Card>();
                card02.Setup(PopItem(!p1Turn), p1Turn); 
                (p1Turn ? p1Hands : p2Hands).Add(card02);

                SetOriginOrder(p1Turn);
                CardAlignment(p1Turn);

                break;

            case 3: // 도박 : 상대 덱 2장을 상대가 사용합니다. 
                Debug.Log(player + "도박 발동");

                effectCard.transform.DOKill();    // 없애버림
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
                
            case 4: // 위협 : 무작위로 상대 패 1장을 버립니다.
                Debug.Log(player + "위협 발동");
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

            case 5: // 훈수 : 무작위로 상대 패 1장을 내가 사용합니다.
                Debug.Log(player + "훈수 발동");
                if ((p1Turn ? p2Hands : p1Hands).Count <= 0)
                    break;   // 반환
                effectCard.transform.DOKill();    // 없애버림
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

            case 6: // 제비뽑기 : 상대 패 1장을 가져옵니다.
                Debug.Log(player + "제비뽑기 발동");
                
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

            case 7: // 수갑 : 상대는 다음 턴에 1장만 사용할 수 있습니다.
                Debug.Log(player + "수갑 발동");
                if (cardEffect07 == true)
                    TurnManager.Inst.turnCount = 1;
                else
                    cardEffect07 = true;
                break;

            case 8: // 경쟁 : 서로 패를 2장 뽑습니다. 
                Debug.Log(player + "경쟁 발동");
                AddCard(false);
                AddCard(false);
                AddCard(true);
                AddCard(true);
                break;

            case 9: // 달리기 : 내 덱 2장을 뽑습니다.
                Debug.Log(player + "달리기 발동");
                AddCard(p1Turn);
                AddCard(p1Turn);
                break;

            case 10: // 밑장 빼기 : 내 덱 1장을 사용합니다. 
                Debug.Log(player + "밑장 빼기 발동");
                effectCard.transform.DOKill();    // 없애버림
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
            
            case 11: // 대출 : 내 덱 2장을 사용합니다.
                Debug.Log(player + "대출 발동");
                effectCard.transform.DOKill();    // 없애버림
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

            case 12: // 결제 : 내 패를 1장 버리고 상대 덱 2장을 버립니다.
                Debug.Log(player + "결제 발동");
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

            case 13: // 올인 : 내 패를 모두 버리고, 그만큼 상대 덱을 버립니다.
                Debug.Log(player + "올인 발동");
                
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

            case 14: // 저주 : 내 덱을 1장 버리고 상대 덱 2장을 버립니다. 
                Debug.Log(player + "저주 발동");
                Vector3 spawnPoint141 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(p1Turn, PopItem(p1Turn), spawnPoint141);
                
                Vector3 spawnPoint142 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint142);
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint142);
                break;

            case 15: // 양보 : 서로 덱 1장을 가져갑니다. 
                Debug.Log(player + "양보 발동");
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

            case 16: // 상부상조 : 서로 덱 1장을 뽑습니다.
                Debug.Log(player + "상부상조 발동");
                AddCard(false);
                AddCard(true);
                break;

            case 17: // 형님 먼저 : 서로 덱 1장을 버립니다.
                Debug.Log(player + "형님 먼저 발동");
                Vector3 spawnPoint171 = !p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(!p1Turn, PopItem(!p1Turn), spawnPoint171);
                Vector3 spawnPoint172 = p1Turn ? p1HandSpawnPoint.position : p2HandSpawnPoint.position;
                EntityManager.Inst.SpawnEntity(p1Turn, PopItem(p1Turn), spawnPoint172);
                break;

            case 18: // 교환 : 서로의 패에서 무작위로 1장을 가져옵니다.
                Debug.Log(player + "교환 발동");
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

            case 19: // 대청소 : 서로 패를 모두 버립니다. 
                Debug.Log(player + "대청소 발동");
                
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

            case 20: // 남의 떡 : 서로 무작위로 상대 패 1장을 사용합니다. 
                Debug.Log(player + "남의 떡 발동");
                effectCard.transform.DOKill();    // 없애버림
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
                Debug.Log("카드가 목록에 없습니다.");
                break;
        }
        yield return delay1;

        if (effectNumber != 3 && effectNumber != 5 && effectNumber != 10 && effectNumber != 11 && effectNumber != 20)
        {
            effectCard.transform.DOKill();    // 없애버림
            DestroyImmediate(effectCard.gameObject);
        }

        TurnManager.Inst.isLoading = false;
        CurrentCardNumbering(p1Turn);
        CardAlignment(p1Turn);
        yield return delay1;
    }

    // #region 마우스 설정

    #region P1Hands

    public void CardMouseOver(Card card)    // 마우스 올리기
    {
        if (eCardState == ECardState.Nothing)
            return;
        
        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)    // 마우스 올리기 해제
    {
        EnlargeCard(false, card); 
    }
    
    public void CardMouseDown() // 클릭 중
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;

        isP1HandDrag = true;
    }

    public void CardMouseUp()   // 클릭 해제
    {
        isP1HandDrag = false;

        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (onP1HandArea)
            return;
        else
            TryPutCard(true);
    }

    void CardDrag() // 카드 드래그 함수
    {
        if (!onP1HandArea)  // 내 카드 영역 벗어나면
        {
            if (eCardState != ECardState.CanMouseDrag)  // 마우스 드래그 가능한 상태가 아니라면(드래그 불가)
                return; // 그대로 반환

            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false); // 카드 움직임, 두트윈 사용X
        }
    }

    void EnlargeCard(bool isEnlarge, Card card) // 카드 확대
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

    void DetectCardArea()   // 카드 범위 인식
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);    // 2D로 마우스 위치 인식
        int layer = LayerMask.NameToLayer("P1HandArea");    // 인식하는 레이어 이름 "P1HandArea"
        onP1HandArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);   // onP1HandArea에 충돌하는 영역 설정
    }

    void SetECardState()    // 카드 선택 가능 유무
    {
        if (TurnManager.Inst.isLoading) // 로딩 중이면
            eCardState = ECardState.Nothing;    // 카드 선택 안됨

        else if (!TurnManager.Inst.p1Turn || p1PutCount == maxCount)    // 내 턴이 아니거나, 이미 1장을 냈거나, 엔티티가 다 찼으면
            eCardState = ECardState.CanMouseOver;   // 카드 확대만 가능

        else if (TurnManager.Inst.p1Turn && (p1PutCount < maxCount))    // 내 턴이면서 내가 낸 카드가 없으면
            eCardState = ECardState.CanMouseDrag;   // 마우스 드래그 가능

        else
            return;
    }
    #endregion

   
}