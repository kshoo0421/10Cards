using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // TMP 사용
using DG.Tweening;  // 두트윈 사용

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;  // 카드 데이터
    [SerializeField] SpriteRenderer character;  // 캐릭터 데이터
    [SerializeField] TMP_Text nameTMP;  // 이름 글자
    [SerializeField] TMP_Text effectTMP;    // 효과 글자
    [SerializeField] Sprite cardFront;  // 카드 앞면 그림
    [SerializeField] Sprite cardBack;   // 카드 뒷면 그림
    [SerializeField] Sprite noImage;    // 임시 없는 이미지

    public Item item;   // 아이템 선언
    bool isFront;   // 앞면 or 뒷면
    public PRS originPRS;   // PRS형식의 originPRS 선언

    public void Setup(Item item, bool isFront)  // 셋업(아이템, 앞면 여부) 함수
    {
        this.item = item;   // 아이템 변수 선언
        this.isFront = isFront; // 앞면, 뒷면 결정

        if (this.isFront)    // 만약 앞면이라면
        {
            card.sprite = cardFront;
            character.sprite = this.item.sprite;    // 캐릭터 스프라이트(그림) 표시
            nameTMP.text = this.item.name;  // 이름 표시
            effectTMP.text = this.item.effect.ToString();   // 효과 표시
        }
        else      // 만약 뒷면이라면
        {
            card.sprite = cardBack; // 카드 스프라이트 뒷면 표시
            character.sprite = noImage;    // 캐릭터 스프라이트(그림) 표시
            nameTMP.text = "";  // 이름 표시 X
            effectTMP.text = "";    // 효과 표시 X
        }
    }

    
    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)  // 카드 뽑기
    {
        if (useDotween) // DOTween 사용하면
        {
            transform.DOMove(prs.pos, dotweenTime); // 움직임
            transform.DORotateQuaternion(prs.rot, dotweenTime); // 회전(각도)
            transform.DOScale(prs.scale, dotweenTime);  // 크기
        }
        else       // 사용하지 않으면
        {
            transform.position = prs.pos;   // 그냥 그 위치에
            transform.rotation = prs.rot;   // 그냥 그 회전(각도)에
            transform.localScale = prs.scale;   // 그냥 그 크기로
        }
    }

    /*
    void OnMouseOver()
    {
        if (isFront)
            CardManager.Inst.CardMouseOver(this);
    }

    void OnMouseExit()
    {
        if (isFront)
            CardManager.Inst.CardMouseExit(this);
    }

    void OnMouseDown()
    {
        if (isFront)
            CardManager.Inst.CardMouseDown();
    }

    void OnMouseUp()
    {
        if (isFront)
            CardManager.Inst.CardMouseUp();
    }
    */
}
