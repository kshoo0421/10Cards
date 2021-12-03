using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ġƮ, UI, ��ŷ, ���ӿ���
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;    // Inst = GameManager ��ũ��Ʈ

    [SerializeField] NotificationPanel notificationPanel;   // �޼��� ��¿� �г�

    /*
    [Multiline(10)]
    [SerializeField] string cheatInfo;
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] TitlePanel titlePanel;
    [SerializeField] CameraEffect cameraEffect;
    [SerializeField] GameObject endTurnBtn;

    WaitForSeconds delay2 = new WaitForSeconds(2);
    */
    void Start()    // ���� ����
    {
        StartGame();
        // UISetup();
    }
    /*
    void UISetup()
    {
        notificationPanel.ScaleZero();
        resultPanel.ScaleZero();
        titlePanel.Active(true);
        cameraEffect.SetGrayScale(false);
    }
    */

    void Update()
    {
#if UNITY_EDITOR    // ����Ƽ(������)���� ġƮŰ ���(���� �� X)
        InputCheatKey();    // ġƮŰ �Է� �� ������� ����
#endif
    }

    void InputCheatKey()    // ġƮŰ ���
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))  // 1�� ������
            TurnManager.OnAddCard?.Invoke(true);  // �� ī�� �߰�

        if (Input.GetKeyDown(KeyCode.Keypad2))  // 2�� ������
            TurnManager.OnAddCard?.Invoke(false); // ��� ī�� �߰�
        if (Input.GetKeyDown(KeyCode.Keypad3))  // 3�� ������
            TurnManager.Inst.EndTurn(); // �� ����

        if (Input.GetKeyDown(KeyCode.Keypad4))  // 3�� ������
            Debug.Log("�ܼ�â Ȯ�ο�");

        /*
        if (Input.GetKeyDown(KeyCode.Keypad4))  // 4�� ������
            CardManager.Inst.TryPutCard(false); // ��밡 ī�带 ��

        if (Input.GetKeyDown(KeyCode.Keypad5))  // 5�� ������
            EntityManager.Inst.DamageBoss(true, 19);    // ������ 19��

        if (Input.GetKeyDown(KeyCode.Keypad6))  // 6�� ������
            EntityManager.Inst.DamageBoss(false, 19);   // ������ 19��
        */
    }

    public void StartGame() // ���� ���� �Լ�
    {
        StartCoroutine(TurnManager.Inst.StartGameCo()); // ���� ���� �Լ� ȣ��, �ڷ�ƾ ����
    }
    
    public void Notification(string message)    // ���� �г�, �޼��� �Է� ��
    {
        notificationPanel.Show(message);    // show�Լ� ���(notificationPanel�� �޼��� ���)
    }


    /*
    public IEnumerator GameOver(bool isMyWin)
    {
        TurnManager.Inst.isLoading = true;
        endTurnBtn.SetActive(false);
        yield return delay2;

        TurnManager.Inst.isLoading = true;
        resultPanel.Show(isMyWin ? "�¸�" : "�й�");
        cameraEffect.SetGrayScale(true);
    }
    */
}
