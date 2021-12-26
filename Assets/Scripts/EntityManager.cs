using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;    // TMP ���

public class EntityManager : MonoBehaviour
{
    public static EntityManager Inst { get; private set; }
    private void Awake() => Inst = this;
    [SerializeField] GameObject entityPrefab;   // ��ƼƼ Prefab ����
    public List<Entity> p1Entities;   // �� ��ƼƼ ���
    public List<Entity> p2Entities;    // ��� ��ƼƼ ���
    [SerializeField] Transform showEntity;  // �����ֱ� ��ġ
    [SerializeField] TMP_Text p1EntityTMP;  // �� ���� TMP
    [SerializeField] TMP_Text p2EntityTMP;  // ��� ���� TMP

    bool CanMouseInput => TurnManager.Inst.p1Turn && !TurnManager.Inst.isLoading;
 
    Entity selectEntity;
    Entity targetPickEntity;
    WaitForSeconds delay1 = new WaitForSeconds(1);  // delay1�� 1�� ���
    WaitForSeconds delay2 = new WaitForSeconds(2);  
    WaitForSeconds delay3 = new WaitForSeconds(3);
    WaitForSeconds delay8 = new WaitForSeconds(8);

    // ���� ����
    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted; // OnTurnStarted Ȱ��ȭ => ���⼭�� Ȱ��ȭ
    }
    
    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted; // ��Ȱ��ȭ => ���⼭�� ��Ȱ��ȭ
    }

    // ���� �Լ�
    void EntityCountNumbering(bool p1Turn)   // �� �� ī��Ʈ
    {
        if(p1Turn == true)
            p1EntityTMP.text = p1Entities.Count.ToString();
        else
            p2EntityTMP.text = p2Entities.Count.ToString();
    }

    void OnTurnStarted(bool p1Turn) // ��� �Ͽ� AI ����
    {
        if (!p1Turn)
        {
            StartCoroutine(AICo());
        }
    }

    IEnumerator AICo()  // ��� AI ����
    {
        yield return delay2;
        if (CardManager.Inst.p2DeckCount.Count > 0)
        {
            CardManager.Inst.TryPutCard(false); // ī�� ��
            yield return delay8; 
        }
        
        if (CardManager.Inst.p2DeckCount.Count > 0 && CardManager.Inst.maxCount == 2 && CardManager.Inst.p2Hands.Count > 0)
        {
            CardManager.Inst.TryPutCard(false); // ī�� ��
            yield return delay8; 
        }

        if (CardManager.Inst.p2DeckCount.Count > 0)
        {   
            TurnManager.Inst.EndTurn(); // �� ����
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
