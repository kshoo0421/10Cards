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

    [SerializeField] ItemSO itemSO; // 외부 "ItemSO(파일형식)"와 itemSO(스크립트) 연결
    [SerializeField] GameObject cardPrefab; // 외부 "GameObject(파일형식)"와 cardPrefab(스크립트) 연결
    [SerializeField] List<Card> myCards;    // 내 카드 리스트와 myCards(스크립트) 연결
    [SerializeField] List<Card> otherCards; // 상대 카드 리스트와 otherCards(스크립트) 연결
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] Transform otherCardSpawnPoint;
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;
    [SerializeField] Transform otherCardLeft;
    [SerializeField] Transform otherCardRight;  // cardSpawnPoint, myCardLeft, myCardRight, otherCardLeft, otherCardRight의 위치와 스크립트 연결
    [SerializeField] ECardState eCardState; // 마우스 클릭/드래그 여부 확인용
    [SerializeField] TMP_Text myDeckTMP;  // 내 덱 TMP
    [SerializeField] TMP_Text otherDeckTMP;  // 상대 덱 TMP
    [SerializeField] Transform myEntitySpawnPoint;
    [SerializeField] Transform otherEntitySpawnPoint;
    [SerializeField] TMP_Text TurnCardTMP;  // 낸 카드 수 TMP
    [SerializeField] TMP_Text MaxCardTMP;  // 최대 카드 수 TMP
    [SerializeField] GameObject myBackCard;  // 낸 카드 수 TMP
    [SerializeField] GameObject otherBackCard;  // 최대 카드 수 TMP

    public List<Item> myDeckCount;  // 아이템 버퍼 리스트 선언
    public List<Item> otherDeckCount;  // 아이템 버퍼 리스트 선언
    Card selectCard;    // 선택한 카드 변수 선언
    bool isMyCardDrag;  // 내 카드 드래그 여부 변수 선언
    bool onMyCardArea;  // 내 카드 영역(카드 확대 영역)
    enum ECardState { Nothing, CanMouseOver, CanMouseDrag } // ECardState는 1. 아무 것도 안되는 경우, 2. 확대만 되는 경우, 3. 드래그까지 되는 경우로 나뉨
    int myPutCount; // 내 턴에 카드 놓기 제한
    public bool[] myPercent;    // 내 덱 구성
    public bool[] otherPercent; // 상대 덱 구성

    // 게임 진행

    void Start()    // 덱 조정 및 아이템 섞기, AddCard, OnTurnStarted 호출
    {
        SetupItemBuffer();  // 덱 조정 및 아이템 섞기
        TurnManager.OnAddCard += AddCard;   // TurnManager가 OnAddCard를 호출하면 AddCard함수 호출
        TurnManager.OnTurnStarted += OnTurnStarted; // TurnManager가 OnTurnStarted를 호출하면 OnTurnStarted함수 호출
    }

    void Update()   // 드래그, DetectCardArea, 카드 선택 가능 유무
    {
        if (isMyCardDrag)   // 내 카드 드래그 O
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
        var cardObject = Instantiate(cardPrefab, isMine ? cardSpawnPoint.position : otherCardSpawnPoint.position, Utils.QI); // cardObject변수는 (카드 세트, 카드 리스폰 위치, 각도)의 정보를 가짐 
        var card = cardObject.GetComponent<Card>(); // card는 Card 스크립트 내용의 변수
        card.Setup(PopItem(isMine), isMine);  // 나 or 상대 카드 뽑기
        (isMine ? myCards : otherCards).Add(card);  // 내꺼면 내 카드, 아니면 상대 카드 추가

        SetOriginOrder(isMine); // 카드 레이어 순서 정렬
        CardAlignment(isMine);  // 카드들 위치 정렬

    }

    void SetOriginOrder(bool isMine)    // 레이어 정렬
    {
        int Count = isMine ? myCards.Count : otherCards.Count; // 내 카드 총 수 or 상대 카드 총 수
        for (int i = 0; i < Count; i++)  // 가진 카드(패) 전체 레이어 정렬
        {
            var targetCard = isMine ? myCards[i] : otherCards[i];   // 내 카드 or 상대 카드
            targetCard?.GetComponent<Order>().SetOriginOrder(i);    // ?는 Nullable(Null값 사용 가능); 카드들 위치 정렬
        }
    }
    
    void CardAlignment(bool isMine) // 패 위치 정렬(내 카드 or 상대 위치)
    {
    
        List<PRS> originCardPRSs = new List<PRS>(); // 카드 리스트의 위치, 회전, 크기
        if (isMine) // 내 카드
            originCardPRSs = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one * 10f);  // 왼오 위치, 카드(패) 수, 상한점, 크기
        else     // 상대 카드
            originCardPRSs = RoundAlignment(otherCardLeft, otherCardRight, otherCards.Count, -0.5f, Vector3.one * 10f);    // 위와 동일
    

        var targetCards = isMine ? myCards : otherCards;    // 손패(나 or 상대)
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
            Item item = myDeckCount[0];  // 아이템이 첫 번째순서면
            myDeckCount.RemoveAt(0); // 아이템 버퍼에서 첫 번째 카드 제거
            return item;    // 함수 종료
        }
        else
        {
            Item item = otherDeckCount[0];  // 아이템이 첫 번째순서면
            otherDeckCount.RemoveAt(0); // 아이템 버퍼에서 첫 번째 카드 제거
            return item;    // 함수 종료
        }
    }

    void SetupItemBuffer() // 아이템 버퍼 세팅
    {
        myDeckCount = new List<Item>(20);   // 아이템 버퍼는 아이템 리스트(20개)의 리스트(새 리스트)
        
        for (int i = 0; i < 20; i++) // 아이템의 총 갯수(20)만큼 반복
        {
            Item item = itemSO.items[i];    // item = 아이템리스트의 i번째 아이템
            bool per = myPercent[i];
            if(per == true)
                myDeckCount.Add(item);   // 리스트에 카드 추가
        }

        for (int i = 0; i < myDeckCount.Count; i++)    // 내꺼 아이템 섞기; 아이템 버퍼의 갯수만큼
        {
            int rand = Random.Range(i, myDeckCount.Count);   // rand는 총 갯수보다 적은 수 중 랜덤값
            Item temp = myDeckCount[i];  // i번째 아이템 임시 저장
            myDeckCount[i] = myDeckCount[rand];   // 랜덤 위치의 아이템을 i번째로
            myDeckCount[rand] = temp;    // i번째 아이템은 그 랜덤위치로 바꿈
        }

        otherDeckCount = new List<Item>(20);
        for (int i = 0; i < itemSO.items.Length; i++) // 아이템의 총 갯수(100)만큼 반복
        {
            Item item = itemSO.items[i];    // item = 아이템리스트의 i번째 아이템
            bool per = myPercent[i];
            if (per == true)
                otherDeckCount.Add(item);   // 리스트에 카드 추가
        }

        for (int i = 0; i < otherDeckCount.Count; i++)    // 상대꺼
        {
            int rand = Random.Range(i, otherDeckCount.Count);   
            Item temp = otherDeckCount[i];
            otherDeckCount[i] = otherDeckCount[rand];
            otherDeckCount[rand] = temp;  
        }
        
    }

    void OnTurnStarted(bool myTurn) // 턴 시작 시
    {
        if (myTurn == true) // 내 턴이면
            myPutCount = 0; // 카드 놓을 수 있음
    }
   
    public bool TryPutCard(bool isMine) // 카드 놓기 함수
    {
        if (isMine && myPutCount >= 2)  // 내 카드인데, 이미 낸 카드가 2를 넘으면
            return false;   // false 반환
        if (!isMine && otherCards.Count <= 0)   // 상대 카드인데, 상대 카드 패가 0이라면
            return false;   // false 반환

        Card card = isMine ? selectCard : otherCards[Random.Range(0, otherCards.Count)];    // 내 카드면 선택한 카드, 상대 카드면 카드 중 아무거나
        Vector3 spawnPoint = isMine ? Utils.MousePos : otherEntitySpawnPoint.position;
        var targetCards = isMine ? myCards : otherCards;    // 타겟 카드는 내 차례면 내 카드들(패), 아니면 상대 카드들(패)

        if (EntityManager.Inst.SpawnEntity(isMine, card.item, spawnPoint))    // 턴, 아이템 내용, 스폰 위치 입력이 되면
        {
            targetCards.Remove(card);   // 타겟 카드(패)에서 카드 삭제
            card.transform.DOKill();    // 없애버림
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

    void CountNumbering()   // 내 덱 카운트
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

    // #region 마우스 설정

    #region MyCard

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
        
        isMyCardDrag = true;
    }

    public void CardMouseUp()   // 클릭 해제
    {
        isMyCardDrag = false;

        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (onMyCardArea)
            EntityManager.Inst.RemoveMyEmptyEntity();

        if (!onMyCardArea)
            TryPutCard(true);
    }

    void CardDrag() // 카드 드래그 함수
    {
        if (!onMyCardArea)  // 내 카드 영역 벗어나면
        {
            if (eCardState != ECardState.CanMouseDrag)  // 마우스 드래그 가능한 상태가 아니라면(드래그 불가)
                return; // 그대로 반환

            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false); // 카드 움직임, 두트윈 사용X
            EntityManager.Inst.InsertMyEmptyEntity(Utils.MousePos.x);   // x축에 맞춰 빈 엔티티 생성(위치 잡는 용도)
        }
    }

    void EnlargeCard(bool isEnlarge, Card card) // 카드 확대
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

    void DetectCardArea()   // 카드 범위 인식
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);    // 2D로 마우스 위치 인식
        int layer = LayerMask.NameToLayer("MyCardArea");    // 인식하는 레이어 이름 "MyCardArea"
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);   // onMyCardArea에 충돌하는 영역 설정
    }

    void SetECardState()    // 카드 선택 가능 유무
    {
        if (TurnManager.Inst.isLoading) // 로딩 중이면
            eCardState = ECardState.Nothing;    // 카드 선택 안됨

        else if (!TurnManager.Inst.myTurn || myPutCount == 2)    // 내 턴이 아니거나, 이미 1장을 냈거나, 엔티티가 다 찼으면
            eCardState = ECardState.CanMouseOver;   // 카드 확대만 가능

        else if (TurnManager.Inst.myTurn && (myPutCount == 0 || myPutCount == 1))    // 내 턴이면서 내가 낸 카드가 없으면
            eCardState = ECardState.CanMouseDrag;   // 마우스 드래그 가능
    }
    #endregion
}