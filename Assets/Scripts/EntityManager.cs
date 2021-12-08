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
    [SerializeField] List<Entity> myEntities;   // �� ��ƼƼ ���
    [SerializeField] List<Entity> otherEntities;    // ��� ��ƼƼ ���
    [SerializeField] Transform showEntity;  // �����ֱ� ��ġ
    [SerializeField] Entity myEmptyEntity;  // �� ��ƼƼ ����
    [SerializeField] TMP_Text myEntityTMP;  // �� ���� TMP
    [SerializeField] TMP_Text otherEntityTMP;  // ��� ���� TMP

    bool ExistMyEmptyEntity => myEntities.Exists(x => x == myEmptyEntity);  // �� entity�� �������� ���� : 
    int MyEmptyEntityIndex => myEntities.FindIndex(x => x == myEmptyEntity);    // 
    bool CanMouseInput => TurnManager.Inst.myTurn && !TurnManager.Inst.isLoading;
    public int otherPutCard;    // ��� ī�� ��

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


    void CountNumbering()   // �� �� ī��Ʈ
    {
         myEntityTMP.text = this.myEntities.Count.ToString();
        otherEntityTMP.text = this.otherEntities.Count.ToString();
    }

    // ���� �Լ�

    void OnTurnStarted(bool myTurn) // ��� �Ͽ� AI ����
    {
        if (!myTurn)
        {
            otherPutCard = 0;
            StartCoroutine(AICo());
        }
    }

    IEnumerator AICo()  // ��� AI ����
    {
        CardManager.Inst.TryPutCard(false); // ī�� ��
        otherPutCard++;
        yield return delay1;    // 1�� ���
        
        CardManager.Inst.TryPutCard(false); // ī�� ��
        otherPutCard++;
        yield return delay1;    // 1�� ���

        TurnManager.Inst.EndTurn(); // �� ����
    }

    void EntityAlignment(bool isMine)   // ��ƼƼ ����
    {
        float targetX = isMine ? -21.4f : 21.7f;
        float targetY = isMine ? -37.2f : 36.7f;    // other / my�� ���� y��ǥ ����
        var targetEntities = isMine ? myEntities: otherEntities;   // target entities�� my/other ����

        for (int i = 0; i < targetEntities.Count; i++)  // entity ����ŭ �ݺ� 
        {
            var targetEntity = targetEntities[i];   // entity �ϳ��ϳ� ����
            targetEntity.originPos = new Vector3(targetX, targetY, 0);  // �� entity ��ġ ����
            targetEntity.MoveTransform(targetEntity.originPos, true, 0.5f); // DoTween ����ؼ� ��ġ �ű�
        }
    }

    public void InsertMyEmptyEntity(float xPos) // �� entity ����(��ġ ��� �뵵)
    {
        if (!ExistMyEmptyEntity)    // EmptyEntity�� ������
            myEntities.Add(myEmptyEntity);  // �� entity�� �� entity ����

        Vector3 emptyEntityPos = myEmptyEntity.transform.position;  // �� entity�� ������ ���� �� �ʱ�ȭ
        emptyEntityPos.x = xPos;    // �� entity�� x��ǥ = x ��ǥ
        myEmptyEntity.transform.position = emptyEntityPos;  // �� entity ��ġ ����

    }
    
    public void RemoveMyEmptyEntity()
    {
        if (!ExistMyEmptyEntity)    // �� entity�� �������� ������
            return; // ��ȯ

        myEntities.RemoveAt(MyEmptyEntityIndex);    // empty entity �ε������� �ٷ� ����
        EntityAlignment(true);  // ��ƼƼ ������

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

        var targetEntities = isMine ? myEntities : otherEntities;
        entity.GetComponent<Order>().SetOriginOrder(targetEntities.Count);
        EntityAlignment(isMine);

        return true;
    }
}
