using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PRS    // Position Rotation Scale
{
    public Vector3 pos; // position은 Vector3 형식의 변수
    public Quaternion rot;  // rotation은 Quaternion(회전 데이터) 형식의 변수
    public Vector3 scale;   // scale은 Vector3 형식의 변수

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)  // PRS함수(위치, 회전, 크기) 선언
    {
        this.pos = pos; // pos는 public Vector3 값
        this.rot = rot; // rot는 public Quaternion 값
        this.scale = scale; // scale은 public Vector3 값
    }
}

public class Utils
{
    public static Quaternion QI => Quaternion.identity; // 회전

    public static Vector3 MousePos  // 마우스 위치
    {
        get // 입력
        {
            Vector3 result = Camera.main.ScreenToWorldPoint(Input.mousePosition);   // 결과값 = 스크린 기준 마우스의 위치
            result.z = -10; // z값 : -10(범위 인식용)
            return result;  // 결과값 반환
        }
    }
}
