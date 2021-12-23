using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] Sprite active; // 실행 가능 버튼
    [SerializeField] Sprite inactive;   // 실행 불가능 버튼
    [SerializeField] Text btnText;
    
    private void Start()    // 시작하면
    {
        Setup(false);   // false로 셋팅
        TurnManager.OnTurnStarted += Setup; // 턴이 시작하면 Setup도 시작
        
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnStarted -= Setup; // 턴이 끝나면 Setup값 false
    }

    public void Setup(bool isActive)    // 활성화/비활성화 유무 받음
    {
        GetComponent<Image>().sprite = isActive ? active : inactive;    // 상황에 맞는 그림 변경
        GetComponent<Button>().interactable = isActive;
        btnText.color = isActive ? new Color32(255, 195, 90, 255) : new Color32(55, 55, 55, 255);   // 노란색 : 회색; Color32 0~255로 색감 표현; color : 0과 1 사이로 표현
    }
}
