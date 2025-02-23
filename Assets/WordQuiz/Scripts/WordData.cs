using System.Collections;
using System.Collections. Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordData : MonoBehaviour
{
    [SerializeField]
    private Text chartext;
[HideInInspector]
    public char charvalue;

    private Button buttonObj;
    private void Awake ()
    {
        buttonObj = GetComponent <Button>();
        if (buttonObj)
        {
            buttonObj.onClick.AddListener(()=> charSelected());
        }

    }

    public void SetChar(char vaulue)
    {
        chartext.text = vaulue + "";
        charvalue = vaulue;
    }
    private void charSelected ()
    {

    }


}