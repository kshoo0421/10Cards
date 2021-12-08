using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] Renderer[] backRenderers;  // ������ Renderer���� ������ ��
    [SerializeField] Renderer[] middleRenderers;    // �߾ӿ� �ִ� Renderer���� ������ ��
    [SerializeField] string sortingLayerName;   // SortingLayer �̸��� ������
    int originOrder;    // originOrder ����

    public void SetOriginOrder(int originOrder) // ���� order ȣ��
    {
        this.originOrder = originOrder; // ���� �״�� ��������
        SetOrder(originOrder);  // ���� ����
    }

    public void SetMostFrontOrder(bool isMostFront) // originOrder���� 0�� ���� ���
    {
        SetOrder(isMostFront ? 100 : originOrder);  // ���̸� ���� ���ʿ� ��ġ, �ƴϸ� ������� ��ġ
    }
   
    public void SetOrder(int order) // �ܺο��� �̸��� �Է��ϸ�
    {
        int mulOrder = order * 10;  // �ű⿡ 10�� ���ϰ�(������ �ΰ�)
 
        foreach (var renderer in backRenderers) // ������ Renderer����
        {
            renderer.sortingLayerName = sortingLayerName;   //  SortingLayer(�з�) ���� ���ְ�
            renderer.sortingOrder = mulOrder;   // ������ order�� ��������
        }

        foreach (var renderer in middleRenderers)    // �߰� Renderer����
        {
            renderer.sortingLayerName = sortingLayerName;   // SortingLayer(�з�) ���� ���ְ�
            renderer.sortingOrder = mulOrder + 1;   // �� ĭ �տ� ���̵��� ����
        }
    }
}
