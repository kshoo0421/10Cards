using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Inst { get; private set; }
    private void Awake() => Inst = this;
    [SerializeField] GameObject entityPrefab;   // 엔티티 Prefab 연결
    [SerializeField] List<Entity> myEntities;   // 내 엔티티 목록
    [SerializeField] List<Entity> otherEntities;    // 상대 엔티티 목록
    [SerializeField] Transform showEntity;  // 보여주기 위치
    [SerializeField] Entity myEmptyEntity;  // 빈 엔티티 생성
    bool ExistMyEmptyEntity => myEntities.Exists(x => x == myEmptyEntity);  // 빈 entity의 존재유무 조건 : 
    int MyEmptyEntityIndex => myEntities.FindIndex(x => x == myEmptyEntity);    // 
    bool CanMouseInput => TurnManager.Inst.myTurn && !TurnManager.Inst.isLoading;

    Entity selectEntity;
    Entity targetPickEntity;
    WaitForSeconds delay1 = new WaitForSeconds(1);  // delay1은 1초 대기
    WaitForSeconds delay2 = new WaitForSeconds(2);  // delay2는 2초 대기


    // 게임 진행

    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted; // OnTurnStarted 활성화 => 여기서도 활성화
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted; // 비활성화 => 여기서도 비활성화
    }

    // 관련 함수

    void OnTurnStarted(bool myTurn) // 상대 턴에 AI 실행
    {
        if (!myTurn)
            StartCoroutine(AICo());
    }

    IEnumerator AICo()  // 상대 AI 설정
    {
        CardManager.Inst.TryPutCard(false); // 카드 냄
        yield return delay1;    // 1초 대기

        CardManager.Inst.TryPutCard(false); // 카드 냄
        yield return delay1;    // 1초 대기

        TurnManager.Inst.EndTurn(); // 턴 종료
    }

    void EntityAlignment(bool isMine)   // 엔티티 정렬
    {
        float targetX = isMine ? -21.4f : 21.7f;
        float targetY = isMine ? -37.2f : 36.7f;    // other / my에 따라 y좌표 변경
        var targetEntities = isMine ? myEntities : otherEntities;   // target entities도 my/other 구분

        for (int i = 0; i < targetEntities.Count; i++)  // entity 수만큼 반복 
        {
            var targetEntity = targetEntities[i];   // entity 하나하나 지정
            targetEntity.originPos = new Vector3(targetX, targetY, 0);  // 각 entity 위치 정렬
            targetEntity.MoveTransform(targetEntity.originPos, true, 0.5f); // DoTween 사용해서 위치 옮김
            targetEntity.GetComponent<Order>()?.SetOriginOrder(i);  // 
        }
    }

    public void InsertMyEmptyEntity(float xPos) // 빈 entity 생성(위치 잡는 용도)
    {
        if (!ExistMyEmptyEntity)    // EmptyEntity가 없으면
            myEntities.Add(myEmptyEntity);  // 내 entity에 빈 entity 생성

        Vector3 emptyEntityPos = myEmptyEntity.transform.position;  // 빈 entity의 포지션 선언 및 초기화
        emptyEntityPos.x = xPos;    // 빈 entity의 x좌표 = x 좌표
        myEmptyEntity.transform.position = emptyEntityPos;  // 빈 entity 위치 적용

    }
    
    public void RemoveMyEmptyEntity()
    {
        if (!ExistMyEmptyEntity)    // 내 entity가 존재하지 않으면
            return; // 반환

        myEntities.RemoveAt(MyEmptyEntityIndex);    // empty entity 인덱스에서 바로 제거
        EntityAlignment(true);  // 엔티티 재정렬

    }

    public bool SpawnEntity(bool isMine, Item item, Vector3 spawnPos)
    {
        var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI);
        var entity = entityObject.GetComponent<Entity>();

        if (isMine)
            myEntities[MyEmptyEntityIndex] = entity;
        else
            otherEntities.Insert(Random.Range(0, otherEntities.Count), entity);

        entity.isMine = isMine;
        entity.Setup(item);
        EntityAlignment(isMine);

        return true;
    }

}
