using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item   // item�� �� �׸��
{
    public string name; // ī�� �̸�
    public string effect; // ī�� ȿ��(����)
    public Sprite sprite;   // ��������Ʈ
    public float percent; // Ȯ��
    public int effectNumber;    // ȿ�� �ѹ�
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]   // ���� �̸� "ItemSO", ������Ʈ���� ��Ŭ������ ������ ������ �� "Scriptable Object" - "ItemSO" ��η� ����
public class ItemSO : ScriptableObject  // ScriptableObject�� ��ӹ���
{
    public Item[] items;
}