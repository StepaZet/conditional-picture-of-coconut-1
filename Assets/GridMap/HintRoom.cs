using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintRoom : MonoBehaviour
{
    public GameObject help;
    public GameObject hint;
    public GameObject hintBack;

    private void OnTriggerStay2D(Collider2D obj)
    {
        if (obj.gameObject.layer != 9)
            return;
        if (help != null)
            help.gameObject.SetActive(true);
        if (!hint.gameObject.activeSelf)
        {
            hint.gameObject.SetActive(true);
            hintBack.gameObject.SetActive(true);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer != 9)
            return;
        if (help != null)
            help.gameObject.SetActive(true);
        hint.gameObject.SetActive(true);
        hintBack.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.gameObject.layer != 9)
            return;
        hint.gameObject.SetActive(false);
        hintBack.gameObject.SetActive(false);
    }
}
