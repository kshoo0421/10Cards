using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PRS    // Position Rotation Scale
{
    public Vector3 pos; // position�� Vector3 ������ ����
    public Quaternion rot;  // rotation�� Quaternion(ȸ�� ������) ������ ����
    public Vector3 scale;   // scale�� Vector3 ������ ����

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)  // PRS�Լ�(��ġ, ȸ��, ũ��) ����
    {
        this.pos = pos; // pos�� public Vector3 ��
        this.rot = rot; // rot�� public Quaternion ��
        this.scale = scale; // scale�� public Vector3 ��
    }
}

public class Utils
{
    public static Quaternion QI => Quaternion.identity; // ȸ��

    public static Vector3 MousePos  // ���콺 ��ġ
    {
        get // �Է�
        {
            Vector3 result = Camera.main.ScreenToWorldPoint(Input.mousePosition);   // ����� = ��ũ�� ���� ���콺�� ��ġ
            result.z = -10; // z�� : -10(���� �νĿ�)
            return result;  // ����� ��ȯ
        }
    }
}
