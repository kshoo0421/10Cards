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
    [SerializeField] List<Entity> p1Entities;   // �� ��ƼƼ ���
    [SerializeField] List<Entity> p2Entities;    // ��� ��ƼƼ ���
    [SerializeField] Transform showEntity;  // �����ֱ� ��ġ
    [SerializeField] Entity p1EmptyEntity;  // �� ��ƼƼ ����
    [SerializeField] TMP_Text p1EntityTMP;  // �� ���� TMP
    [SerializeField] TMP_Text p2EntityTMP;  // ��� ���� TMP
    [SerializeField] Transform p1HandSpawnPoint;
    [SerializeField] Transform p2HandSpawnPoint;

    bool ExistP1EmptyEntity => p1Entities.Exists(x => x == p1EmptyEntity);  // �� entity�� �������� ���� : 
    int P1EmptyEntityIndex => p1Entities.FindIndex(x => x == p1EmptyEntity);    // 
    bool CanMouseInput => TurnManager.Inst.p1Turn && !TurnManager.Inst.isLoading;
    public int p2PutCard;    // ��� ī�� ��

    Entity selectEntity;
    Entity targetPickEntity;
    WaitForSeconds delay1 = new WaitForSeconds(1);  // delay1�� 1�� ���
    WaitForSeconds delay2 = new WaitForSeconds(2);  // delay2�� 2�� ���

    // ���� ����
    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted; // OnTurnStarted Ȱ��ȭ => ���⼭�� Ȱ��ȭ
    }

    void Update()
    {
        CountNumbering();
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted; // ��Ȱ��ȭ => ���⼭�� ��Ȱ��ȭ
    }

    // ���� �Լ�
    void CountNumbering()   // �� �� ī��Ʈ
    {
        p1EntityTMP.text = this.p1Entities.Count.ToString();
        p2EntityTMP.text = this.p2Entities.Count.ToString();
    }

    void OnTurnStarted(bool p1Turn) // ��� �Ͽ� AI ����
    {
        if (!p1Turn)
        {
            p2PutCard = 0;
            StartCoroutine(AICo());
        }
    }

    IEnumerator AICo()  // ��� AI ����
    {
        if (CardManager.Inst.p2DeckCount.Count > 0)
        {
            CardManager.Inst.TryPutCard(false); // ī�� ��
            p2PutCard++;
            yield return delay1;    // 1�� ���

            CardManager.Inst.TryPutCard(false); // ī�� ��
            p2PutCard++;
            yield return delay1;    // 1�� ���

            TurnManager.Inst.EndTurn(); // �� ����
        }

        yield return delay2;
    }

    void EntityAlignment(bool isMine)   // ��ƼƼ ����
    {
        float targetX = isMine ? -21.4f : 21.7f;
        float targetY = isMine ? -37.2f : 36.7f;    // other / my�� ���� y��ǥ ����
        var targetEntities = isMine ? p1Entities: p2Entities;   // target entities�� my/other ����

        for (int i = 0; i < targetEntities.Count; i++)  // entity ����ŭ �ݺ� 
        {
            var targetEntity = targetEntities[i];   // entity �ϳ��ϳ� ����
            targetEntity.originPos = new Vector3(targetX, targetY, 0);  // �� entity ��ġ ����
            targetEntity.MoveTransform(targetEntity.originPos, true, 0.5f); // DoTween ����ؼ� ��ġ �ű�
        }
    }

    public void InsertP1EmptyEntity(float xPos) // �� entity ����(��ġ ��� �뵵)
    {
        if (!ExistP1EmptyEntity)    // EmptyEntity�� ������
            p1Entities.Add(p1EmptyEntity);  // �� entity�� �� entity ����

        Vector3 emptyEntityPos = p1EmptyEntity.transform.position;  // �� entity�� ������ ���� �� �ʱ�ȭ
        emptyEntityPos.x = xPos;    // �� entity�� x��ǥ = x ��ǥ
        p1EmptyEntity.transform.position = emptyEntityPos;  // �� entity ��ġ ����
    }
    
    public void RemoveP1EmptyEntity()
    {
        if (!ExistP1EmptyEntity)    // �� entity�� �������� ������
            return; // ��ȯ

        p1Entities.RemoveAt(P1EmptyEntityIndex);    // empty entity �ε������� �ٷ� ����
        EntityAlignment(true);  // ��ƼƼ ������

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
