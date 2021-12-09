using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // TMP 사용
using DG.Tweening;  // DOTween 사용

public class NotificationPanel : MonoBehaviour
{
    [SerializeField] TMP_Text notificationTMP;  // TMP 파일 연결

    public void Show(string message)    // 메세지 출력
    {
        notificationTMP.text = message; // TMP 텍스트 = 메세지
        Sequence sequence = DOTween.Sequence()  // 두트윈용 시퀀스 함수
            .Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad))   // 초기값 = 0(Unity에서 설정); scale 1까지 변화, 시간 0.3초
            .AppendInterval(0.9f)   // 0.9초동안 변화 없이 보여줌
            .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad)); // scale 0까지 변화
    }

    void Start() => ScaleZero();    // 시작하면 크기 0으로

    [ContextMenu("ScaleOne")]   // 우클릭 시 선택지 생성; 이름 "ScaleOne"
    void ScaleOne() => transform.localScale = Vector3.one;  // 크기 1(최대)로

    [ContextMenu("ScaleZero")]  // 우클릭 시 선택지 생성; 이름 "ScaleZero"
    public void ScaleZero() => transform.localScale = Vector3.zero; // 크기 0(최소, 없음)으로
}
