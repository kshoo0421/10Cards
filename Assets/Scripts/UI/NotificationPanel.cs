using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // TMP ���
using DG.Tweening;  // DOTween ���

public class NotificationPanel : MonoBehaviour
{
    [SerializeField] TMP_Text notificationTMP;  // TMP ���� ����

    public void Show(string message)    // �޼��� ���
    {
        notificationTMP.text = message; // TMP �ؽ�Ʈ = �޼���
        Sequence sequence = DOTween.Sequence()  // ��Ʈ���� ������ �Լ�
            .Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad))   // �ʱⰪ = 0(Unity���� ����); scale 1���� ��ȭ, �ð� 0.3��
            .AppendInterval(0.9f)   // 0.9�ʵ��� ��ȭ ���� ������
            .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad)); // scale 0���� ��ȭ
    }

    void Start() => ScaleZero();    // �����ϸ� ũ�� 0����

    [ContextMenu("ScaleOne")]   // ��Ŭ�� �� ������ ����; �̸� "ScaleOne"
    void ScaleOne() => transform.localScale = Vector3.one;  // ũ�� 1(�ִ�)��

    [ContextMenu("ScaleZero")]  // ��Ŭ�� �� ������ ����; �̸� "ScaleZero"
    public void ScaleZero() => transform.localScale = Vector3.zero; // ũ�� 0(�ּ�, ����)����
}
