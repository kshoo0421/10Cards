using System.Collections;
using System.Collections.Generic;
using System;   // Action 사용 시 필요
using UnityEngine;
using Random = UnityEngine.Random;  // System과 UnityEngine의 Random이 겹쳐서 UnityEngine의 Random을 사용하겠다는 의미

public class TurnManager : MonoBehaviour
{
    public static TurnManager Inst { get; private set; }
    private void Awake() => Inst = this;

    [Header("Develop")]
    [SerializeField] [Tooltip("시작 턴 모드를 정합니다")] ETurnMode eTurnMode;    // 선공(내 턴, 상대 턴, 랜덤) 결정
    [SerializeField] [Tooltip("카드 배분이 매우 빨라집니다")] bool fastMode;    // 빠른 모드 유무
    [SerializeField] [Tooltip("시작 카드 개수를 정합니다")] int startCardCount;    // 시작 패 장수 결정
    [Header("Properties")]
    public bool isLoading; // 게임 끝나면 isLoading을 true로 하면 카드와 엔티티 클릭 방지
    public bool p1Turn; // 내 턴 / 상대 턴 결정
    enum ETurnMode { Random, P1, P2 }    // enum은 연관된 상수들의 집합. 랜덤, 내 턴, 상대 턴 중 결정
    WaitForSeconds delay05 = new WaitForSeconds(0.5f);  // delay05면 진행 시간 0.5초
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);  // delay07이면 진행 시간 0.7초
    public static Action<bool> OnAddCard;   // OnAddCard 함수 설정, 외부 연결
    public static event Action<bool> OnTurnStarted; // 턴 시작 함수 설정, 외부 연결

    // 관련 함수

    void GameSetup()    // 고속모드 유무, 선공 결정
    {
        if (fastMode)   // 고속모드면
            delay05 = new WaitForSeconds(0.05f);    // delay05가 0.05초로 바뀜 = 진행 속도가 빨라짐

        switch (eTurnMode)  // 선공 결정
        {
            case ETurnMode.Random:  // case1. random이면
                p1Turn = Random.Range(0, 2) == 0;   // 둘 중에 하나; a == 0 이 참이면 내 턴, 아니면 상대 턴
                break;
            case ETurnMode.P1:  // 내 턴이면
                p1Turn = true;  // 내 턴 = true
                break;
            case ETurnMode.P2:   // 상대 턴이면
                p1Turn = false; // 내 턴 = false
                break;
        }
    }

    public IEnumerator StartGameCo()    // 게임(Coroutine) 실행 함수
    {
        GameSetup();    // 게임 셋팅(속도, 선공)
        isLoading = true;   // 로딩 중 표시(기타 행동 방지)

        for (int i = 0; i < startCardCount; i++)    // 시작 카드 장수만큼 반복
        {
            yield return delay05;   // *yield return 각 요소를 따로 반환; 속도 : delay05(일반 0.5초, 고속 0.05초)
            OnAddCard?.Invoke(false);   // OnAddCard = false
            yield return delay05;   // 속도 : delay05
            OnAddCard?.Invoke(true);    // OnAddCard = true
        }
        StartCoroutine(StartTurnCo());  // 턴 시작 함수 실행
    }

    IEnumerator StartTurnCo()   // 턴 시작 함수
    {
        isLoading = true;   // 로딩 중 표시(기타 행동 방지)
        if (p1Turn) // 내 턴이면
            GameManager.Inst.Notification("나의 턴");  // 나의 턴 표시
      
        yield return delay07;   // 속도 : delay07
        OnAddCard?.Invoke(p1Turn);  // OnAddCard = true
        yield return delay07;   // 속도 : delay07
        isLoading = false;  // 로딩 끝 = 입력 가능
        
        OnTurnStarted?.Invoke(p1Turn);  // OnTurnStarted = true; 내 턴 시작
    }

    public void EndTurn()   // 턴 종료
    {
        p1Turn = !p1Turn;   // 내 턴에서 상대 턴으로 전환
        StartCoroutine(StartTurnCo());  // (상대) 턴 시작
    }
}
