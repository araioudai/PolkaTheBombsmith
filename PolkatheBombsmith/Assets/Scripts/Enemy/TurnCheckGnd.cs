using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCheckGnd : MonoBehaviour
{
    public bool turnPointGnd;

    void Start()
    {
        turnPointGnd = false; // �����l��ݒ�
        Debug.Log(turnPointGnd);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("�n�ʒ[�ł�"); 
        turnPointGnd = true; // ���]������
    }
}
