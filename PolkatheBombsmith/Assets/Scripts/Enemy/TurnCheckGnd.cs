using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCheckGnd : MonoBehaviour
{
    public bool turnPointGnd;

    void Start()
    {
        turnPointGnd = false; // ‰Šú’l‚ğİ’è
        Debug.Log(turnPointGnd);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("’n–Ê’[‚Å‚·"); 
        turnPointGnd = true; // ”½“]‚³‚¹‚é
    }
}
