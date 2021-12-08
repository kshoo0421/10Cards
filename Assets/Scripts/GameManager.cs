using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ġƮ, UI, ��ŷ, ���ӿ���
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;    // Inst = GameManager ��ũ��Ʈ

    [Multiline(10)]
    [SerializeField] NotificationPanel notificationPanel;   // �޼��� ��¿� �г�
    WaitForSeconds delay1 = new WaitForSeconds(1);
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] GameObject endTurnBtn;
    [SerializeField] TitlePanel titlePanel;
    [SerializeField] CameraEffect cameraEffect;

    // ���� ����

    void Start()    // ���� ����
    {
        UISetup();
    }
    
    void Update()   // ġƮŰ �Է�
    {
#if UNITY_EDITOR    // ����Ƽ(������)���� ġƮŰ ���(���� �� X)
        InputCheatKey();    // ġƮŰ �Է� �� ������� ����
#endif
    }

    // ���� �Լ�
    void InputCheatKey()    // ġƮŰ ���
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))  // 1�� ������
            TurnManager.OnAddCard?.Invoke(true);  // �� ī�� �߰�

        if (Input.GetKeyDown(KeyCode.Keypad2))  // 2�� ������
            TurnManager.OnAddCard?.Invoke(false); // ��� ī�� �߰�

        if (Input.GetKeyDown(KeyCode.Keypad3))  // 3�� ������
            TurnManager.Inst.EndTurn(); // �� ����

        if (Input.GetKeyDown(KeyCode.Keypad4))  // 4�� ������
            CardManager.Inst.TryPutCard(false); // ��밡 ī�带 ��
    }

    public void StartGame() // ���� ���� �Լ� ȣ��
    {
        StartCoroutine(TurnManager.Inst.StartGameCo()); // ���� ���� �Լ� ȣ��, �ڷ�ƾ ����
    }
    
    public void Notification(string message)    // ���� �г�, �޼��� �Է� ��
    {
        notificationPanel.Show(message);    // show�Լ� ���(notificationPanel�� �޼��� ���)
    }

    void UISetup()
    {
        notificationPanel.ScaleZero();
        resultPanel.ScaleZero();
        titlePanel.Active(true);
        cameraEffect.SetGrayScale(false);
    }

    public IEnumerator GameOver(bool isMyWin)
    {
        TurnManager.Inst.isLoading = true;
        endTurnBtn.SetActive(false);
        yield return delay1;

        TurnManager.Inst.isLoading = true;
        resultPanel.Show(isMyWin ? "�¸�" : "�й�");
        cameraEffect.SetGrayScale(true);
        
    }
}
