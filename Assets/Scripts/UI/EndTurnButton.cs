using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] Sprite active; // ���� ���� ��ư
    [SerializeField] Sprite inactive;   // ���� �Ұ��� ��ư
    [SerializeField] Text btnText;
    
    private void Start()    // �����ϸ�
    {
        Setup(false);   // false�� ����
        TurnManager.OnTurnStarted += Setup; // ���� �����ϸ� Setup�� ����
        
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnStarted -= Setup; // ���� ������ Setup�� false
    }

    public void Setup(bool isActive)    // Ȱ��ȭ/��Ȱ��ȭ ���� ����
    {
        GetComponent<Image>().sprite = isActive ? active : inactive;    // ��Ȳ�� �´� �׸� ����
        GetComponent<Button>().interactable = isActive;
        btnText.color = isActive ? new Color32(255, 195, 90, 255) : new Color32(55, 55, 55, 255);   // ����� : ȸ��; Color32 0~255�� ���� ǥ��; color : 0�� 1 ���̷� ǥ��
    }
}
