using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    [SerializeField] Material effectMat;


    void OnRenderImage(RenderTexture _src, RenderTexture _dest) // 모든 랜더링에서 렌더링을 완료하고 호출되는 함수, src = source(render texture)
    {
        if (effectMat == null)
            return;

        Graphics.Blit(_src, _dest, effectMat);
    }

    void OnDestroy()
    {
        SetGrayScale(false);
    }

    public void SetGrayScale(bool isGrayscale)
    {
        effectMat.SetFloat("_GrayscaleAmount", isGrayscale ? 1 : 0);
        effectMat.SetFloat("_DarkAmount", isGrayscale ? 0.12f : 0);
    }
}
