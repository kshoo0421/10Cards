using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] Renderer[] backRenderers;  // 뒤쪽을 Renderer들을 가지고 옴
    [SerializeField] Renderer[] middleRenderers;    // 중앙에 있는 Renderer들을 가지고 옴
    [SerializeField] string sortingLayerName;   // SortingLayer 이름을 정해줌
    int originOrder;    // originOrder 선언

    public void SetOriginOrder(int originOrder) // 최초 order 호출
    {
        this.originOrder = originOrder; // 변수 그대로 가져오기
        SetOrder(originOrder);  // 순서 세팅
    }

    public void SetMostFrontOrder(bool isMostFront) // originOrder값이 0일 때를 대비
    {
        SetOrder(isMostFront ? 100 : originOrder);  // 참이면 가장 앞쪽에 배치, 아니면 기존대로 배치
    }
   
    public void SetOrder(int order) // 외부에서 이름만 입력하면
    {
        int mulOrder = order * 10;  // 거기에 10을 곱하고(간격을 두고)
 
        foreach (var renderer in backRenderers) // 뒤쪽의 Renderer들을
        {
            renderer.sortingLayerName = sortingLayerName;   //  SortingLayer(분류) 먼저 해주고
            renderer.sortingOrder = mulOrder;   // 곱해진 order를 대입해줌
        }

        foreach (var renderer in middleRenderers)    // 중간 Renderer들을
        {
            renderer.sortingLayerName = sortingLayerName;   // SortingLayer(분류) 먼저 해주고
            renderer.sortingOrder = mulOrder + 1;   // 한 칸 앞에 보이도록 해줌
        }
    }
}
