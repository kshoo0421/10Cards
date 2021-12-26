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
    public List<Entity> p1Entities;   // 내 엔티티 목록
    public List<Entity> p2Entities;    // 상대 엔티티 목록
    [SerializeField] Transform showEntity;  // 보여주기 위치
    [SerializeField] TMP_Text p1EntityTMP;  // 내 묘지 TMP
    [SerializeField] TMP_Text p2EntityTMP;  // 상대 묘지 TMP

    bool CanMouseInput => TurnManager.Inst.p1Turn && !TurnManager.Inst.isLoading;
 
    Entity selectEntity;
    Entity targetPickEntity;
    WaitForSeconds delay1 = new WaitForSeconds(1);  // delay1은 1초 대기
    WaitForSeconds delay2 = new WaitForSeconds(2);  
    WaitForSeconds delay3 = new WaitForSeconds(3);
    WaitForSeconds delay8 = new WaitForSeconds(8);

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
    void EntityCountNumbering(bool p1Turn)   // 내 덱 카운트
    {
        if(p1Turn == true)
            p1EntityTMP.text = p1Entities.Count.ToString();
        else
            p2EntityTMP.text = p2Entities.Count.ToString();
    }

    void OnTurnStarted(bool p1Turn) // 상대 턴에 AI 실행
    {
        if (!p1Turn)
        {
            StartCoroutine(AICo());
        }
    }

    IEnumerator AICo()  // 상대 AI 설정
    {
        yield return delay2;
        if (CardManager.Inst.p2DeckCount.Count > 0)
        {
            CardManager.Inst.TryPutCard(false); // 카드 냄
            yield return delay8; 
        }
        
        if (CardManager.Inst.p2DeckCount.Count > 0 && CardManager.Inst.maxCount == 2 && CardManager.Inst.p2Hands.Count > 0)
        {
            CardManager.Inst.TryPutCard(false); // 카드 냄
            yield return delay8; 
        }

        if (CardManager.Inst.p2DeckCount.Count > 0)
        {   
            TurnManager.Inst.EndTurn(); // 턴 종료
        }
    }

    public bool SpawnEntity(bool isMine, Item item, Vector3 spawnPos)
    {
        var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI);
        var entity = entityObject.GetComponent<Entity>();

        entity.isMine = isMine;
        entity.Setup(item);
        
        (isMine ? p1Entities : p2Entities).Add(entity);

        var targetEntities = isMine ? p1Entities : p2Entities;
        entity.GetComponent<Order>().SetOriginOrder(targetEntities.Count);

        float targetX = isMine ? -21.4f : 21.7f;
        float targetY = isMine ? -37.2f : 36.7f;
        entity.originPos = new Vector3(targetX, targetY, 0);
        entity.MoveTransform(entity.originPos, true, 0.5f);
        
        EntityCountNumbering(isMine);

        return true;
    }
}
