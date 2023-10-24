using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class WordSlot : MonoBehaviour
{
    public Text textWord;
    

    public void UseItem()
    {
        GameManager.instance.ClearDate();

        string nameButton = EventSystem.current.currentSelectedGameObject.name;
        if(nameButton == "WordSlot(Clone)") //Alphabet word
        {
            GameManager.instance.ChangeCameraPosition(2);
            SQLiteDB.instance.SelectData(textWord.text);
        }
        else if(textWord.text == "Witam")
        {
            GameManager.instance.ChangeCameraPosition(3);
            SQLiteDB.instance.SelectDataHand(textWord.text);
        }
        else
        {
            if(SQLiteDB.instance.SelectVRData(textWord.text) == 0)
                GameManager.instance.ChangeCameraPosition(1);
            else
                GameManager.instance.ChangeCameraPosition(2);

            SQLiteDB.instance.SelectDataHand(textWord.text);
        }
    }

    public void OnCursorEnter()
    {
        if (SQLiteDB.instance.GetVariations(textWord.text) != "null")
            GameManager.instance.DisplayWordInfo(SQLiteDB.instance.GetVariations(textWord.text), transform.position);
    }
    public void OnCursorExit()
    {
        GameManager.instance.DestroyWordInfo();
    }

    public void DestroySlot()
    {
        Menu.instance.EditButton(true, textWord.text, gameObject);
    }

    public void UpdateSlot()
    {
        Menu.instance.EditButton(false, textWord.text, gameObject);
    }
}
