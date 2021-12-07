using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item   // item에 들어갈 항목들
{
    public string name; // 카드 이름
    public string effect; // 카드 효과(설명)
    public Sprite sprite;   // 스프라이트
    public float percent; // 확률
    public int effectNumber;    // 효과 넘버
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]   // 파일 이름 "ItemSO", 프로젝트에서 우클릭으로 파일을 생성할 때 "Scriptable Object" - "ItemSO" 경로로 생성
public class ItemSO : ScriptableObject  // ScriptableObject를 상속받음
{
    public Item[] items;
}