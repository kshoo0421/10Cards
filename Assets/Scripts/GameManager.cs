using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 치트, UI, 랭킹, 게임오버
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;    // Inst = GameManager 스크립트

    [Multiline(10)]
    [SerializeField] NotificationPanel notificationPanel;   // 메세지 출력용 패널
    WaitForSeconds delay1 = new WaitForSeconds(1);
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] GameObject endTurnBtn;
    [SerializeField] TitlePanel titlePanel;
    [SerializeField] CameraEffect cameraEffect;

    // 게임 진행

    void Start()    // 게임 시작
    {
        UISetup();
    }
    
    void Update()   // 치트키 입력
    {
#if UNITY_EDITOR    // 유니티(에디터)에서 치트키 사용(게임 내 X)
        InputCheatKey();    // 치트키 입력 시 설정대로 실행
#endif
    }

    // 관련 함수
    void InputCheatKey()    // 치트키 목록
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))  // 1번 누르면
            TurnManager.OnAddCard?.Invoke(true);  // 내 카드 추가

        if (Input.GetKeyDown(KeyCode.Keypad2))  // 2번 누르면
            TurnManager.OnAddCard?.Invoke(false); // 상대 카드 추가

        if (Input.GetKeyDown(KeyCode.Keypad3))  // 3번 누르면
            TurnManager.Inst.EndTurn(); // 턴 종료

        if (Input.GetKeyDown(KeyCode.Keypad4))  // 4번 누르면
            CardManager.Inst.TryPutCard(false); // 상대가 카드를 냄
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

    public IEnumerator GameOver(bool isMyWin)
    {
        TurnManager.Inst.isLoading = true;
        endTurnBtn.SetActive(false);
        yield return delay1;

        TurnManager.Inst.isLoading = true;
        resultPanel.Show(isMyWin ? "승리" : "패배");
        cameraEffect.SetGrayScale(true);
        
    }
}
