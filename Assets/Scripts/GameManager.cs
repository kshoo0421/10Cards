using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 치트, UI, 랭킹, 게임오버
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;    // Inst = GameManager 스크립트

    [SerializeField] NotificationPanel notificationPanel;   // 메세지 출력용 패널
    WaitForSeconds delay1 = new WaitForSeconds(1);
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] GameObject endTurnBtn;
    [SerializeField] TitlePanel titlePanel;
    [SerializeField] CameraEffect cameraEffect;

    public GameObject p1O;
    public GameObject p2O;
    public GameObject randomO;

    // 게임 진행
    void Start()    // 게임 시작
    {
        UISetup();
    }
    
    // 관련 함수
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


    public void StartGame() // 게임 시작 함수 호출
    {
        StartCoroutine(TurnManager.Inst.StartGameCo()); // 게임 시작 함수 호출, 코루틴 실행
    }
    
    public void Notification(string message)    // 공지 패널, 메세지 입력 시
    {
        notificationPanel.Show(message);    // show함수 출력(notificationPanel에 메세지 출력)
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
        resultPanel.Show(isP1Win ? "승리" : "패배");
        
        cameraEffect.SetGrayScale(true);
    }
}
