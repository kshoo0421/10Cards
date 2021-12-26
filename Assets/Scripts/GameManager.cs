using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ġƮ, UI, ��ŷ, ���ӿ���
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;    // Inst = GameManager ��ũ��Ʈ

    [SerializeField] NotificationPanel notificationPanel;   // �޼��� ��¿� �г�
    WaitForSeconds delay1 = new WaitForSeconds(1);
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] GameObject endTurnBtn;
    [SerializeField] TitlePanel titlePanel;
    [SerializeField] CameraEffect cameraEffect;

    public GameObject p1O;
    public GameObject p2O;
    public GameObject randomO;

    // ���� ����
    void Start()    // ���� ����
    {
        UISetup();
    }
    
    // ���� �Լ�
    public void RandomTurn()
    {
        TurnManager.Inst.eTurnMode = 0;
        randomO.SetActive(true);
        p1O.SetActive(false);
        p2O.SetActive(false);
    }

    public void P1Turn()
    {
        TurnManager.Inst.eTurnMode = 1;
        randomO.SetActive(false);
        p1O.SetActive(true);
        p2O.SetActive(false);
    }

    public void P2Turn()
    {
        TurnManager.Inst.eTurnMode = 2;
        randomO.SetActive(false);
        p1O.SetActive(false);
        p2O.SetActive(true);
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

    public IEnumerator GameOver(bool isP1Win)
    {
        TurnManager.Inst.isLoading = true;
        endTurnBtn.SetActive(false);
        yield return delay1;

        TurnManager.Inst.isLoading = true;
        resultPanel.Show(isP1Win ? "�¸�" : "�й�");
        
        cameraEffect.SetGrayScale(true);
    }
}
