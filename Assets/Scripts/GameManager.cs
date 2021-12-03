using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 치트, UI, 랭킹, 게임오버
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;    // Inst = GameManager 스크립트

    [SerializeField] NotificationPanel notificationPanel;   // 메세지 출력용 패널

    /*
    [Multiline(10)]
    [SerializeField] string cheatInfo;
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] TitlePanel titlePanel;
    [SerializeField] CameraEffect cameraEffect;
    [SerializeField] GameObject endTurnBtn;

    WaitForSeconds delay2 = new WaitForSeconds(2);
    */
    void Start()    // 게임 시작
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
#if UNITY_EDITOR    // 유니티(에디터)에서 치트키 사용(게임 내 X)
        InputCheatKey();    // 치트키 입력 시 설정대로 실행
#endif
    }

    void InputCheatKey()    // 치트키 목록
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))  // 1번 누르면
            TurnManager.OnAddCard?.Invoke(true);  // 내 카드 추가

        if (Input.GetKeyDown(KeyCode.Keypad2))  // 2번 누르면
            TurnManager.OnAddCard?.Invoke(false); // 상대 카드 추가
        if (Input.GetKeyDown(KeyCode.Keypad3))  // 3번 누르면
            TurnManager.Inst.EndTurn(); // 턴 종료

        if (Input.GetKeyDown(KeyCode.Keypad4))  // 3번 누르면
            Debug.Log("콘솔창 확인용");

        /*
        if (Input.GetKeyDown(KeyCode.Keypad4))  // 4번 누르면
            CardManager.Inst.TryPutCard(false); // 상대가 카드를 냄

        if (Input.GetKeyDown(KeyCode.Keypad5))  // 5번 누르면
            EntityManager.Inst.DamageBoss(true, 19);    // 나한테 19뎀

        if (Input.GetKeyDown(KeyCode.Keypad6))  // 6번 누르면
            EntityManager.Inst.DamageBoss(false, 19);   // 적한테 19뎀
        */
    }

    public void StartGame() // 게임 시작 함수
    {
        StartCoroutine(TurnManager.Inst.StartGameCo()); // 게임 시작 함수 호출, 코루틴 실행
    }
    
    public void Notification(string message)    // 공지 패널, 메세지 입력 시
    {
        notificationPanel.Show(message);    // show함수 출력(notificationPanel에 메세지 출력)
    }


    /*
    public IEnumerator GameOver(bool isMyWin)
    {
        TurnManager.Inst.isLoading = true;
        endTurnBtn.SetActive(false);
        yield return delay2;

        TurnManager.Inst.isLoading = true;
        resultPanel.Show(isMyWin ? "승리" : "패배");
        cameraEffect.SetGrayScale(true);
    }
    */
}
