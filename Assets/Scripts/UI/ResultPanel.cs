using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] TMP_Text resultTMP;

    public void Show(string message)
    {
        resultTMP.text = message;
        transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutQuad);
    }

    public void Restart()
    {
        SceneManager.LoadScene(2);  // ÇöÀç ¾À ·Îµù
    }

    public void MenuBack()
    {
        SceneManager.LoadScene(1);  // ¸Þ´º ¾À ·Îµù
    }

    private void Start() => ScaleZero();

    [ContextMenu("ScaleOne")]
    void scaleOne() => transform.localScale = Vector3.one;

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero;
}
