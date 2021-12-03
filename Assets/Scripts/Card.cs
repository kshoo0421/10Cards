using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // TMP ���
using DG.Tweening;  // ��Ʈ�� ���

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;  // ī�� ������
    [SerializeField] SpriteRenderer character;  // ĳ���� ������
    [SerializeField] TMP_Text nameTMP;  // �̸� ����
    [SerializeField] TMP_Text effectTMP;    // ȿ�� ����
    [SerializeField] Sprite cardFront;  // ī�� �ո� �׸�
    [SerializeField] Sprite cardBack;   // ī�� �޸� �׸�
    [SerializeField] Sprite noImage;    // �ӽ� ���� �̹���

    public Item item;   // ������ ����
    bool isFront;   // �ո� or �޸�
    public PRS originPRS;   // PRS������ originPRS ����

    public void Setup(Item item, bool isFront)  // �¾�(������, �ո� ����) �Լ�
    {
        this.item = item;   // ������ ���� ����
        this.isFront = isFront; // �ո�, �޸� ����

        if (this.isFront)    // ���� �ո��̶��
        {
            card.sprite = cardFront;
            character.sprite = this.item.sprite;    // ĳ���� ��������Ʈ(�׸�) ǥ��
            nameTMP.text = this.item.name;  // �̸� ǥ��
            effectTMP.text = this.item.effect.ToString();   // ȿ�� ǥ��
        }
        else      // ���� �޸��̶��
        {
            card.sprite = cardBack; // ī�� ��������Ʈ �޸� ǥ��
            character.sprite = noImage;    // ĳ���� ��������Ʈ(�׸�) ǥ��
            nameTMP.text = "";  // �̸� ǥ�� X
            effectTMP.text = "";    // ȿ�� ǥ�� X
        }
    }

    
    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)  // ī�� �̱�
    {
        if (useDotween) // DOTween ����ϸ�
        {
            transform.DOMove(prs.pos, dotweenTime); // ������
            transform.DORotateQuaternion(prs.rot, dotweenTime); // ȸ��(����)
            transform.DOScale(prs.scale, dotweenTime);  // ũ��
        }
        else       // ������� ������
        {
            transform.position = prs.pos;   // �׳� �� ��ġ��
            transform.rotation = prs.rot;   // �׳� �� ȸ��(����)��
            transform.localScale = prs.scale;   // �׳� �� ũ���
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
