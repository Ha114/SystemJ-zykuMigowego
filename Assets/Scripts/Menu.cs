using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class Menu : MonoBehaviour
{
    #region singleton
    public static Menu instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion
    private String _editWord;
    public GameObject ShureDeletePanel;
    public GameObject RecordNewPanel;
    private GameObject _slot;

    public InputField input;
    public GameObject AlphCont;
    public GameObject DictCont;

    public void Start()
    {
        input.onValueChanged.AddListener(delegate { ValueChangeCheck(input.text); });
    }
    public void ValueChangeCheck(string s)
    {
        SQLiteDB.instance.UpdateDictionarySearchLetter(s, AlphCont, DictCont);
    }

    public void CleatSearchText(InputField input)
    {
        input.text = "";
        SQLiteDB.instance.FindByWorld("", DictCont);

    }

    public void FindByWordInSqlDB(string category)
    {
        SQLiteDB.instance.FindByWorld(category, DictCont);
    }

    public void EditButton(bool b, string name, GameObject go)
    {
        _slot = go;
        if (b) { DeleteWord(name); }
        else { UpdateWord(name); }
    }

    //Update Word
    private void UpdateWord(string nameWord)
    {
        _editWord = nameWord;
        RecordNewPanel.SetActive(true);
        Text massegeText = RecordNewPanel.transform.GetChild(2).GetComponent<Text>();
        InputField inputWordName = RecordNewPanel.transform.GetChild(0).GetComponent<InputField>();
        InputField inputVarName = RecordNewPanel.transform.GetChild(1).GetComponent<InputField>();

        inputWordName.text = nameWord;
        inputVarName.text = SQLiteDB.instance.SelectVariations(nameWord);

        massegeText.text = "Przepisz słowo  <b>" + nameWord + "</b>";
        GameManager.instance.ClearDate();
        SQLiteDB.instance.SelectDataHand(nameWord);
    }

    public void UpdateButton()
    {
        Destroy(_slot);
        SQLiteDB.instance.DeleteQuery(_editWord);
        RecordWord(RecordNewPanel);
        _editWord = null;
        _slot = null;
        RecordNewPanel.SetActive(false);
    }

    //Delete Word
    private void DeleteWord(string nameWord)
    {
        _editWord = nameWord;
        ShureDeletePanel.SetActive(true);
        Text warning = ShureDeletePanel.transform.GetChild(0).GetComponent<Text>();
        warning.text = "Czy na pewno chcesz usunąć słowo <b>" + nameWord + "</b>?";
    }

    public void DeleteWordButtonShure()
    {
        GameManager.instance.Message("Słowo o nazwie <b>" + _editWord + "</b> zostało usunięte");
        Destroy(_slot);
        ShureDeletePanel.SetActive(false);
        SQLiteDB.instance.DeleteQuery(_editWord);
        _editWord = null;
        _slot = null;
    }


    public void ClearText(InputField inputField)
    {
        inputField.text = "";
    }

    public void AcceptText(InputField inputField)
    {
        StartCoroutine(inputTextWord(inputField.text));
    }

    IEnumerator inputTextWord(string str)
    {
        //Divide into words
        string[] newStr = str.Split(new char[] { ' ', ',', ';', ':', '-', '!', '?', '#', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string x in newStr)
        {
            string result = SQLiteDB.instance.QueryNameVar(x);
            if (result != "")
            {
                GameManager.instance.isShow = false;
                if (SQLiteDB.instance.SelectVRData(x) == 0)
                    GameManager.instance.ChangeCameraPosition(1);
                else
                    GameManager.instance.ChangeCameraPosition(2);
                SQLiteDB.instance.SelectDataHand(result);

                int lh = SQLiteDB.instance.GetDataLeft(result);
                int rh = SQLiteDB.instance.GetDataRight(result);
                float lastItemTime;
                if (lh > rh) { lastItemTime = GameManager.instance.L_segmentList[0].timeStamp.LastOrDefault(); }
                else { lastItemTime = GameManager.instance.segmentList[0].timeStamp.LastOrDefault(); }
                yield return new WaitForSeconds(lastItemTime);
            }
            else
            {
                char[] b = x.ToCharArray();
                foreach (char ch in b)
                {
                    GameManager.instance.ChangeCameraPosition(2);
                    if (ch == '.')
                        StopCoroutine(inputTextWord(str));
                    else if (ch != ' ')
                    {
                       // Debug.Log("Ch = " + ch);
                        SQLiteDB.instance.SelectData(ch.ToString().ToUpper());
                        float lastItemTime = GameManager.instance.segmentList[0].timeStamp.LastOrDefault();
                        yield return new WaitForSeconds(lastItemTime);
                    }
                }
            }
        }
        StopCoroutine(inputTextWord(str));
    }

    public void StateRecord(bool b)
    {
        if (b) { GameManager.instance.ClearDate(); }
        string name = EventSystem.current.currentSelectedGameObject.name;
        Button btn = GameObject.Find(name).GetComponent<Button>();
        var t = btn.transform.parent.GetComponent<Transform>();
        Toggle tgRightHand = t.gameObject.transform.GetChild(3).GetComponent<Toggle>();
        Toggle tgLeftHand = t.gameObject.transform.GetChild(4).GetComponent<Toggle>();
        if (tgRightHand.isOn && !tgLeftHand.isOn)
        {
            GameManager.instance.ChangeStateRecordRight(b);
           // Debug.Log("Prawa ręka");
        }
        else if (!tgRightHand.isOn && tgLeftHand.isOn)
        {
            GameManager.instance.ChangeStateRecordLeft(b);
          //  Debug.Log("Lewa ręka");
        }
        else if (tgRightHand.isOn && tgLeftHand.isOn)
        {
            GameManager.instance.ChangeStateRecordTwoHandl(b);
          //  Debug.Log("Dwie ręka");
        }
        else if (!tgRightHand.isOn && !tgLeftHand.isOn)
        {
            GameManager.instance.Message("Gest nie może składać się z <b>0</b> rąk");
        }
    }

    public void ShowAlphabet(string name)
    {
        GameManager.instance.ChangeStateReplay(true, 0);
        Debug.Log("Prawa ręka");
    }


    public void Show()
    {
        GameManager.instance.isShow = true;
        string name = EventSystem.current.currentSelectedGameObject.name;
        Button btn = GameObject.Find(name).GetComponent<Button>();
        var t = btn.transform.parent.GetComponent<Transform>();
        Toggle tgRightHand = t.gameObject.transform.GetChild(3).GetComponent<Toggle>();
        Toggle tgLeftHand = t.gameObject.transform.GetChild(4).GetComponent<Toggle>();

        if (tgRightHand.isOn && !tgLeftHand.isOn)
        {
            GameManager.instance.ChangeStateReplay(true, 0);
            Debug.Log("Prawa ręka");
        }
        else if (!tgRightHand.isOn && tgLeftHand.isOn)
        {
            GameManager.instance.ChangeStateReplay(true, 1);
            Debug.Log("Lewa ręka");
        }
        else if (tgRightHand.isOn && tgLeftHand.isOn)
        {
            GameManager.instance.ChangeStateReplay(true, 2);
            Debug.Log("Dwie ręka");
        }
        else if (!tgRightHand.isOn && !tgLeftHand.isOn)
        {
            GameManager.instance.Message("Gest nie może składać się z <b>0</b> rąk");
        }
    }


    public void ShowPreviousSighn(GameObject Panel)
    {
        GameManager.instance.ChangeStateReplay(true, 0);
    }

    public void ClearDataGhost()
    {
        GameManager.instance.ClearDate();
    }

    public void RecordWord(GameObject Panel)
    {
        InputField nameWord = Panel.transform.GetChild(0).GetComponent<InputField>();
        InputField varWord = Panel.transform.GetChild(1).GetComponent<InputField>();

        Toggle tgRightHand = Panel.transform.GetChild(3).GetComponent<Toggle>();
        Toggle tgLeftHand = Panel.transform.GetChild(4).GetComponent<Toggle>();

        InputField categories = Panel.transform.GetChild(1).GetComponent<InputField>();

        //check what hand user used for sigh
        int LH = 0, RH = 0;
        if(tgLeftHand.isOn) { LH = 1; }
        if (tgRightHand.isOn) { RH = 1; }


        bool b = SQLiteDB.instance.check(nameWord.text);
        bool checkInfo = CheckFields(nameWord, varWord, RH, LH);
        if (!b)
        {
            if (checkInfo)
            {
                if (!tgRightHand.isOn && !tgLeftHand.isOn)
                {
                    GameManager.instance.Message("Gest nie może składać się z <b>0</b> rąk");
                }
                else
                {
                    Record(nameWord.text, varWord.text, RH, LH, categories.text);
                }
            }
        }
        else
        {
            GameManager.instance.Message("Słowo o nazwie <b>" + nameWord.text + "</b> już istnieje");
        }
    }

    void Record(string name, string var, int rh, int lh, string cat)
    {
        SQLiteDB.instance.InsertDictionary(name, var, rh, lh, cat);
        SQLiteDB.instance.CreateTableByName(name);
        GameManager.instance.AddSegmentData(name, rh, lh);
        GameManager.instance.InstantiateDictionaryWordSlot(name);
        GameManager.instance.ClearDate();
    }

    bool CheckFields(InputField name, InputField var, int rh, int lh)
    {
        if (name.text == "")
        {
            GameManager.instance.Message("Nie wprowadzono <b>nazwy</b> słowa");
            return false;
        }
        else if (var.text == "")
        {
            GameManager.instance.Message("Nie wprowadzono <b>odmiany</b> słowa");
            return false;
        }
        if (rh == 1)
        {
            if (GameManager.instance.segmentList[0].timeStamp.Count == 0)
            {
                GameManager.instance.Message("Słowo nie jest nagrane");
                return false;
            }
            else if (name.text != "" && var.text != "" && var.text != "" && GameManager.instance.segmentList[0].timeStamp.Count != 0)
            {
                GameManager.instance.Message("Słowo o nazwie <b>" + name.text + "</b> zostało pomyślnie zapisane");
                return true;
            }
        }
        else if (lh == 1)
        {
            if (GameManager.instance.L_segmentList[0].timeStamp.Count == 0)
            {
                GameManager.instance.Message("Słowo nie jest nagrane");
                return false;
            }
            else if (name.text != "" && var.text != "" && var.text != "" && GameManager.instance.L_segmentList[0].timeStamp.Count != 0)
            {
                GameManager.instance.Message("Słowo o nazwie <b>" + name.text + "</b> zostało pomyślnie zapisane");
                return true;
            }
        }
        else if(rh == 1 && lh == 1)
        {
            if (GameManager.instance.segmentList[0].timeStamp.Count == 0 && GameManager.instance.L_segmentList[0].timeStamp.Count == 0)
            {
                GameManager.instance.Message("Słowo nie jest nagrane");
                return false;
            }
            else if (name.text != "" && var.text != "" && var.text != "" && GameManager.instance.L_segmentList[0].timeStamp.Count != 0 && GameManager.instance.segmentList[0].timeStamp.Count != 0)
            {
                GameManager.instance.Message("Słowo o nazwie <b>" + name.text + "</b> zostało pomyślnie zapisane");
                return true;
            }
        }        
        return false;
    }

    public void ShowDictionary()
    {
        SQLiteDB.instance.UpdateDictionary();
    }

    public void ShowPanel(GameObject Panel)
    {
        Panel.SetActive(true);
    }
    public void ClosePanel(GameObject Panel)
    {
        Panel.SetActive(false);
    }
    public void ExitApp()
    {
        Application.Quit();
    }
}
