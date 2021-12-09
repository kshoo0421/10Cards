using System.Collections;
using System.Collections.Generic;
using System;   // Action ��� �� �ʿ�
using UnityEngine;
using Random = UnityEngine.Random;  // System�� UnityEngine�� Random�� ���ļ� UnityEngine�� Random�� ����ϰڴٴ� �ǹ�

public class TurnManager : MonoBehaviour
{
    public static TurnManager Inst { get; private set; }
    private void Awake() => Inst = this;

    [Header("Develop")]
    [SerializeField] [Tooltip("���� �� ��带 ���մϴ�")] ETurnMode eTurnMode;    // ����(�� ��, ��� ��, ����) ����
    [SerializeField] [Tooltip("ī�� ����� �ſ� �������ϴ�")] bool fastMode;    // ���� ��� ����
    [SerializeField] [Tooltip("���� ī�� ������ ���մϴ�")] int startCardCount;    // ���� �� ��� ����
    [Header("Properties")]
    public bool isLoading; // ���� ������ isLoading�� true�� �ϸ� ī��� ��ƼƼ Ŭ�� ����
    public bool p1Turn; // �� �� / ��� �� ����
    enum ETurnMode { Random, P1, P2 }    // enum�� ������ ������� ����. ����, �� ��, ��� �� �� ����
    WaitForSeconds delay05 = new WaitForSeconds(0.5f);  // delay05�� ���� �ð� 0.5��
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);  // delay07�̸� ���� �ð� 0.7��
    public static Action<bool> OnAddCard;   // OnAddCard �Լ� ����, �ܺ� ����
    public static event Action<bool> OnTurnStarted; // �� ���� �Լ� ����, �ܺ� ����

    // ���� �Լ�

    void GameSetup()    // ��Ӹ�� ����, ���� ����
    {
        if (fastMode)   // ��Ӹ���
            delay05 = new WaitForSeconds(0.05f);    // delay05�� 0.05�ʷ� �ٲ� = ���� �ӵ��� ������

        switch (eTurnMode)  // ���� ����
        {
            case ETurnMode.Random:  // case1. random�̸�
                p1Turn = Random.Range(0, 2) == 0;   // �� �߿� �ϳ�; a == 0 �� ���̸� �� ��, �ƴϸ� ��� ��
                break;
            case ETurnMode.P1:  // �� ���̸�
                p1Turn = true;  // �� �� = true
                break;
            case ETurnMode.P2:   // ��� ���̸�
                p1Turn = false; // �� �� = false
                break;
        }
    }

    public IEnumerator StartGameCo()    // ����(Coroutine) ���� �Լ�
    {
        GameSetup();    // ���� ����(�ӵ�, ����)
        isLoading = true;   // �ε� �� ǥ��(��Ÿ �ൿ ����)

        for (int i = 0; i < startCardCount; i++)    // ���� ī�� �����ŭ �ݺ�
        {
            yield return delay05;   // *yield return �� ��Ҹ� ���� ��ȯ; �ӵ� : delay05(�Ϲ� 0.5��, ��� 0.05��)
            OnAddCard?.Invoke(false);   // OnAddCard = false
            yield return delay05;   // �ӵ� : delay05
            OnAddCard?.Invoke(true);    // OnAddCard = true
        }
        StartCoroutine(StartTurnCo());  // �� ���� �Լ� ����
    }

    IEnumerator StartTurnCo()   // �� ���� �Լ�
    {
        isLoading = true;   // �ε� �� ǥ��(��Ÿ �ൿ ����)
        if (p1Turn) // �� ���̸�
            GameManager.Inst.Notification("���� ��");  // ���� �� ǥ��
      
        yield return delay07;   // �ӵ� : delay07
        OnAddCard?.Invoke(p1Turn);  // OnAddCard = true
        yield return delay07;   // �ӵ� : delay07
        isLoading = false;  // �ε� �� = �Է� ����
        
        OnTurnStarted?.Invoke(p1Turn);  // OnTurnStarted = true; �� �� ����
    }

    public void EndTurn()   // �� ����
    {
        p1Turn = !p1Turn;   // �� �Ͽ��� ��� ������ ��ȯ
        StartCoroutine(StartTurnCo());  // (���) �� ����
    }
}
