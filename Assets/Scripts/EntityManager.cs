using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;    // TMP 사용

public class EntityManager : MonoBehaviour
{
    public static EntityManager Inst { get; private set; }
    private void Awake() => Inst = this;
    [SerializeField] GameObject entityPrefab;   // 엔티티 Prefab 연결
    [SerializeField] List<Entity> p1Entities;   // 내 엔티티 목록
    [SerializeField] List<Entity> p2Entities;    // 상대 엔티티 목록
    [SerializeField] Transform showEntity;  // 보여주기 위치
    [SerializeField] Entity p1EmptyEntity;  // 빈 엔티티 생성
    [SerializeField] TMP_Text p1EntityTMP;  // 내 묘지 TMP
    [SerializeField] TMP_Text p2EntityTMP;  // 상대 묘지 TMP
    [SerializeField] Transform p1HandSpawnPoint;
    [SerializeField] Transform p2HandSpawnPoint;

    bool ExistP1EmptyEntity => p1Entities.Exists(x => x == p1EmptyEntity);  // 빈 entity의 존재유무 조건 : 
    int P1EmptyEntityIndex => p1Entities.FindIndex(x => x == p1EmptyEntity);    // 
    bool CanMouseInput => TurnManager.Inst.p1Turn && !TurnManager.Inst.isLoading;
    public int p2PutCard;    // 상대 카드 수

    Entity selectEntity;
    Entity targetPickEntity;
    WaitForSeconds delay1 = new WaitForSeconds(1);  // delay1은 1초 대기
    WaitForSeconds delay2 = new WaitForSeconds(2);  // delay2는 2초 대기

    // 게임 진행
    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted; // OnTurnStarted 활성화 => 여기서도 활성화
    }

    void Update()
    {
        CountNumbering();
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted; // 비활성화 => 여기서도 비활성화
    }

    // 관련 함수
    void CountNumbering()   // 내 덱 카운트
    {
        p1EntityTMP.text = this.p1Entities.Count.ToString();
        p2EntityTMP.text = this.p2Entities.Count.ToString();
    }

    void OnTurnStarted(bool p1Turn) // 상대 턴에 AI 실행
    {
        if (!p1Turn)
        {
            p2PutCard = 0;
            StartCoroutine(AICo());
        }
    }

    IEnumerator AICo()  // 상대 AI 설정
    {
        if (CardManager.Inst.p2DeckCount.Count > 0)
        {
            CardManager.Inst.TryPutCard(false); // 카드 냄
            p2PutCard++;
            yield return delay1;    // 1초 대기

            CardManager.Inst.TryPutCard(false); // 카드 냄
            p2PutCard++;
            yield return delay1;    // 1초 대기

            TurnManager.Inst.EndTurn(); // 턴 종료
        }

        yield return delay2;
    }

    void EntityAlignment(bool isMine)   // 엔티티 정렬
    {
        float targetX = isMine ? -21.4f : 21.7f;
        float targetY = isMine ? -37.2f : 36.7f;    // other / my에 따라 y좌표 변경
        var targetEntities = isMine ? p1Entities: p2Entities;   // target entities도 my/other 구분

        for (int i = 0; i < targetEntities.Count; i++)  // entity 수만큼 반복 
        {
            var targetEntity = targetEntities[i];   // entity 하나하나 지정
            targetEntity.originPos = new Vector3(targetX, targetY, 0);  // 각 entity 위치 정렬
            targetEntity.MoveTransform(targetEntity.originPos, true, 0.5f); // DoTween 사용해서 위치 옮김
        }
    }

    public void InsertP1EmptyEntity(float xPos) // 빈 entity 생성(위치 잡는 용도)
    {
        if (!ExistP1EmptyEntity)    // EmptyEntity가 없으면
            p1Entities.Add(p1EmptyEntity);  // 내 entity에 빈 entity 생성

        Vector3 emptyEntityPos = p1EmptyEntity.transform.position;  // 빈 entity의 포지션 선언 및 초기화
        emptyEntityPos.x = xPos;    // 빈 entity의 x좌표 = x 좌표
        p1EmptyEntity.transform.position = emptyEntityPos;  // 빈 entity 위치 적용
    }
    
    public void RemoveP1EmptyEntity()
    {
        if (!ExistP1EmptyEntity)    // 내 entity가 존재하지 않으면
            return; // 반환

        p1Entities.RemoveAt(P1EmptyEntityIndex);    // empty entity 인덱스에서 바로 제거
        EntityAlignment(true);  // 엔티티 재정렬

    }

    public bool SpawnEntity(bool isMine, Item item, Vector3 spawnPos)
    {
        var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI);
        var entity = entityObject.GetComponent<Entity>();

        if (isMine)
            p1Entities[P1EmptyEntityIndex] = entity;
        else
            p2Entities.Insert(Random.Range(0, p2Entities.Count), entity);

        entity.isMine = isMine;
        entity.Setup(item);

        var targetEntities = isMine ? p1Entities : p2Entities;
        entity.GetComponent<Order>().SetOriginOrder(targetEntities.Count);
        EntityAlignment(isMine);

        return true;
    }
}
