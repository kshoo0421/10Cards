using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // TMP 사용
using DG.Tweening;  // 두트윈 사용

public class Entity : MonoBehaviour
{
    public Item item; // 아이템
    [SerializeField] SpriteRenderer entity; // 엔티티 그림
    [SerializeField] SpriteRenderer character;  // 캐릭터 그림
    [SerializeField] TMP_Text nameTMP;  // 이름 TMP
    [SerializeField] TMP_Text effectTMP;    // 효과 TMP
    
    public string effect;
    public bool isMine; // 내 것인지
    public Vector3 originPos;  // 정렬용 originPos

    // 관련 함수
 
    public void Setup(Item item)    // Entity 아이템 셋업
    {
        this.item = item;   // 아이템 변수 그대로 활용
        character.sprite = this.item.sprite;    // 카드 그림 가져오기
        nameTMP.text = this.item.name;  // 카드 이름 가져와서 표시하기
        effectTMP.text = this.item.effect; // 효과 TMP로 표시
    }

    public void MoveTransform(Vector3 pos, bool useDotween, float dotweenTime = 0)  // 움직임 주기
    {
        if (useDotween) // 두트윈 사용 시
            transform.DOMove(pos, dotweenTime); // 두트윈 사용해서 움직이기
        else    // 아니면
            transform.position = pos;   // 물체 순간이동
    }
}