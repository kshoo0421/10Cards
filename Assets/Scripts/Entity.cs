using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // TMP ���
using DG.Tweening;  // ��Ʈ�� ���

public class Entity : MonoBehaviour
{
    public Item item; // ������
    [SerializeField] SpriteRenderer entity; // ��ƼƼ �׸�
    [SerializeField] SpriteRenderer character;  // ĳ���� �׸�
    [SerializeField] TMP_Text nameTMP;  // �̸� TMP
    [SerializeField] TMP_Text effectTMP;    // ȿ�� TMP
    
    public string effect;
    public bool isMine; // �� ������
    public Vector3 originPos;  // ���Ŀ� originPos

    // ���� �Լ�
 
    public void Setup(Item item)    // Entity ������ �¾�
    {
        this.item = item;   // ������ ���� �״�� Ȱ��
        character.sprite = this.item.sprite;    // ī�� �׸� ��������
        nameTMP.text = this.item.name;  // ī�� �̸� �����ͼ� ǥ���ϱ�
        effectTMP.text = this.item.effect; // ȿ�� TMP�� ǥ��
    }

    public void MoveTransform(Vector3 pos, bool useDotween, float dotweenTime = 0)  // ������ �ֱ�
    {
        if (useDotween) // ��Ʈ�� ��� ��
            transform.DOMove(pos, dotweenTime); // ��Ʈ�� ����ؼ� �����̱�
        else    // �ƴϸ�
            transform.position = pos;   // ��ü �����̵�
    }
}