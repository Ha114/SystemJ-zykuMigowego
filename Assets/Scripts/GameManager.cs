//using Leap.Unity.Attachments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region singleton
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion
    //RightHand
    [Header("Values for Right Hand")]
    public List<Segment> segmentList = new List<Segment>();
    [SerializeField] List<GameObject> handSegments = new List<GameObject>();
    [SerializeField] List<GameObject> handSegmentsRepeat = new List<GameObject>();
    //baze object for list creation
    public GameObject RWirstHand;
    public GameObject RWirstHandRepeat;
    //for use hand
    public GameObject HandModelRight;
    public GameObject SecondHandModel;

    //LeftHand
    [Header("Values for Left Hand")]
    public List<Segment> L_segmentList = new List<Segment>();
    public List<GameObject> L_handSegments = new List<GameObject>();
    public List<GameObject> L_handSegmentsRepeat = new List<GameObject>();
    //baze object for list creation
    public GameObject LWirstHand;
    public GameObject LWirstHandRepeat;
    //for use hand
    public GameObject HandModelLeft;
    public GameObject SecondHandModelLeft;


    //For two hands
    [Header("Two Hand")]
    public List<Segment> twoSegmentHandList = new List<Segment>();
    public List<GameObject> Two_handSegments = new List<GameObject>();
    public List<GameObject> Two_handSegmentsRepeat = new List<GameObject>();


    //for letter in ALphabet
    [Header("ALPHABET VALUES")]
    public List<GameObject> LetterSegmentsRepeat_LEFT = new List<GameObject>();
    public List<GameObject> LetterSegmentsRepeat_RIGHT = new List<GameObject>();
    [Header("ALPHABET VALUES TWO HANDS")]
    public List<Segment> twoSegmentHandList_LETTER = new List<Segment>();
    public List<GameObject> Two_handSegmentsRepeat_LETTER = new List<GameObject>();

    //baze object for list creation
    public GameObject WirstHandRepeat_LEFT;
    public GameObject WirstHandRepeat_RIGHT;
    //for use hand
    public GameObject LetterHandModelRepeat_LEFT;
    public GameObject LetterHandModelRepeat_RIGHT;



    //Alphabet
    public Transform AlphabetTransform;
    public GameObject WordSlotPrefab;
    //Dictionary
    public Transform DictionaryTransform;
    public GameObject DictionarySlotPrefab;

    public GameObject CameraObjcet;

    //MassageManager
    public GameObject prefMessageManager;
    public GameObject prefMesaage;

    //WordInfo
    public Transform canvas;
    private GameObject currentWordInfo = null;
    public GameObject wordInfoPref;

    //If user what to show or convert text
    public bool isShow = false;

    private void Start()
    {
        ClearDate();
        ChangeCameraPosition(1);
        SetChildHand();
        DoubleList();
    }


    void DoubleList()
    {
        twoSegmentHandList = segmentList.Concat(L_segmentList).ToList();
        twoSegmentHandList_LETTER = segmentList.Concat(L_segmentList).ToList();
        Two_handSegments = handSegments.Concat(L_handSegments).ToList();
        Two_handSegmentsRepeat = handSegmentsRepeat.Concat(L_handSegmentsRepeat).ToList();
        Two_handSegmentsRepeat_LETTER = LetterSegmentsRepeat_RIGHT.Concat(LetterSegmentsRepeat_LEFT).ToList();
    }


    void SetChildHand()
    {
        GetChildRecursive(RWirstHand, handSegments);
        GetChildRecursive(RWirstHandRepeat, handSegmentsRepeat);
        GetChildRecursive(LWirstHand, L_handSegments);
        GetChildRecursive(LWirstHandRepeat, L_handSegmentsRepeat);
        GetChildRecursive(WirstHandRepeat_LEFT, LetterSegmentsRepeat_LEFT);
        GetChildRecursive(WirstHandRepeat_RIGHT, LetterSegmentsRepeat_RIGHT);
    }

    private void GetChildRecursive(GameObject obj, List<GameObject> list)
    {
        if (null == obj)
            return;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;
            list.Add(child.gameObject);
            GetChildRecursive(child.gameObject, list);
        }
    }

    public void ChangeCameraPosition(int b)
    {
        switch(b)
        {
            case 1: CameraObjcet.transform.position = new Vector3(0.01919f, 0.252f, 0.645f); break;
            case 2: CameraObjcet.transform.position = new Vector3(-0.019f, 2.667f, 0.92f); break;
            case 3: CameraObjcet.transform.position = new Vector3(0.146f, 2.55f, 0.92f); break;
            default: Debug.LogError("Błąd"); break;
        }
    }

    public void DisplayWordInfo(string Name, Vector2 buttonPos)
    {
        if (currentWordInfo != null)
        {
            Destroy(currentWordInfo.gameObject);
        }

        buttonPos.x -= 180f; //180f
        buttonPos.y += 70f; //70f

        currentWordInfo = Instantiate(wordInfoPref, buttonPos, Quaternion.identity, canvas);
        currentWordInfo.GetComponent<WordInfo>().SetUp(Name);

    }
    public void DestroyWordInfo()
    {
        if (currentWordInfo != null)
        {
            Destroy(currentWordInfo.gameObject);
        }
    }


    public void SendingDataToSQL(string NameTable)
    {
        SQLiteDB.instance.SelectData(NameTable);
    }

    public void SetLetterSegmentFromSQL(string id, string timeStep, string position, string rotation)
    {
        int count = Int16.Parse(id) - 1;
        Segment segment = segmentList[count];
        //timeStep
        string[] wordsTime = timeStep.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for(int i = 0; i < wordsTime.Length; i++)
        {
            segment.timeStamp.Add(float.Parse(wordsTime[i], CultureInfo.InvariantCulture));
        }
        //position
        string[] wordsPos = position.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < wordsPos.Length; i++)
        {
            string[] xyzPos = wordsPos[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Vector3 pos = ParseToVector3(xyzPos[0], xyzPos[1], xyzPos[2]);
            segment.position.Add(pos);
        }
        //rotation
        string[] wordsRot = rotation.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < wordsRot.Length; i++)
        {
            string[] xyzwRot = wordsRot[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Quaternion rot = ParseToQuaternion(xyzwRot[3], xyzwRot[0], xyzwRot[1], xyzwRot[2]);
            segment.rotation.Add(rot);
        }
        
         ChangeStateReplay(true, 3);
    }

    public void SetWordSegmentFromSQL(string id, string timeStep, string position, string rotation, int state)
    {
        int count = Int16.Parse(id) - 1;
        Segment segment = null;
        if(state == 1)
        {
            segment = L_segmentList[count];
        }
        else if(state == 0)
        {
            segment = segmentList[count];
        }
        else if (state == 2)
        {
            segment = twoSegmentHandList[count];
        }

        //timeStep
        string[] wordsTime = timeStep.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < wordsTime.Length; i++)
        {
            segment.timeStamp.Add(float.Parse(wordsTime[i], CultureInfo.InvariantCulture));
        }
        //position
        string[] wordsPos = position.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < wordsPos.Length; i++)
        {
            string[] xyzPos = wordsPos[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Vector3 pos = ParseToVector3(xyzPos[0], xyzPos[1], xyzPos[2]);
            segment.position.Add(pos);
        }
        //rotation
        string[] wordsRot = rotation.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < wordsRot.Length; i++)
        {
            string[] xyzwRot = wordsRot[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Quaternion rot = ParseToQuaternion(xyzwRot[3], xyzwRot[0], xyzwRot[1], xyzwRot[2]);
            segment.rotation.Add(rot);
        }
        ChangeStateReplay(true, state);
    }


    public Quaternion ParseToQuaternion(string wS, string xS, string yS, string zS)
    {
        float x = float.Parse(xS, CultureInfo.InvariantCulture);
        float y = float.Parse(yS, CultureInfo.InvariantCulture);
        float z = float.Parse(zS, CultureInfo.InvariantCulture);
        float w = float.Parse(wS, CultureInfo.InvariantCulture);

        Quaternion quaternion = new Quaternion(x, y,z,w);
        return quaternion;
    }


    public Vector3 ParseToVector3(string xS, string yS, string zS)
    {
        float x = float.Parse(xS, CultureInfo.InvariantCulture);
        float y = float.Parse(yS, CultureInfo.InvariantCulture);
        float z = float.Parse(zS, CultureInfo.InvariantCulture);
        Vector3 resultVector = new Vector3(x, y, z);
        return resultVector;
    }




    //PRAWA reka start przycisk
    public void ChangeStateRecordRight(bool b)
    {
        foreach(Segment segment in segmentList)
        {
            if (b)
            {
                HandModelRight.SetActive(true);
                foreach(GameObject h in handSegments)
                {
                    SegmentRecorder segmentRecorder = h.GetComponent<SegmentRecorder>();
                    segmentRecorder.enabled = true;
                    segmentRecorder.SetTime();
                }
            }
            else
            {
                HandModelRight.SetActive(false);
                foreach (GameObject h in handSegments)
                {
                    SegmentRecorder segmentRecorder = h.GetComponent<SegmentRecorder>();
                    segmentRecorder.enabled = b;
                }
            }
            segment.isRecord = b;
        }
    }

    //LEWA reka start przycisk
    public void ChangeStateRecordLeft(bool b)
    {
        foreach (Segment segment in L_segmentList)
        {
            if (b)
            {
                HandModelLeft.SetActive(true);
                foreach (GameObject h in L_handSegments)
                {
                    SegmentRecorder segmentRecorder = h.GetComponent<SegmentRecorder>();
                    segmentRecorder.enabled = true;
                    segmentRecorder.SetTime();
                }
            }
            else
            {
                HandModelLeft.SetActive(false);
                foreach (GameObject h in L_handSegments)
                {
                    SegmentRecorder segmentRecorder = h.GetComponent<SegmentRecorder>();
                    segmentRecorder.enabled = b;
                }
            }
            segment.isRecord = b;
        }
    }
    //Two reka start przycisk
    public void ChangeStateRecordTwoHandl(bool b)
    {
        foreach (Segment segment in twoSegmentHandList)
        {
            if (b)
            {
                HandModelLeft.SetActive(true);
                HandModelRight.SetActive(true);
                foreach (GameObject h in Two_handSegments)
                {
                    SegmentRecorder segmentRecorder = h.GetComponent<SegmentRecorder>();
                    segmentRecorder.enabled = true;
                    segmentRecorder.SetTime();
                }
            }
            else
            {
                HandModelLeft.SetActive(false);
                HandModelRight.SetActive(false);
                foreach (GameObject h in Two_handSegments)
                {
                    SegmentRecorder segmentRecorder = h.GetComponent<SegmentRecorder>();
                    segmentRecorder.enabled = b;
                }
            }
            segment.isRecord = b;
        }
    }

    public void ChangeStateReplay(bool b, int handCount)
    {
        switch (handCount)
        {
            case 0: StartCoroutine(ShowSighn(b)); break; //RightHand
            case 1: StartCoroutine(ShowSighnLeft(b)); break; //LeftHand
            case 2: StartCoroutine(ShowSighnTwo(b)); break; //TwoHands
            case 3: StartCoroutine(ShowSighnLetter(b)); break; //Alphabet letter
            default: Debug.Log("błąd"); break; //zero hand
        }
    }

    //for two letters hand (VR)
    IEnumerator ShowSighnTwoLetter(bool b)
    {
        Debug.LogError("ShowSighnTwoLetter");

        foreach (Segment segment in twoSegmentHandList)
        {
            if (b)
            {
                foreach (GameObject h in Two_handSegmentsRepeat_LETTER)
                {
                    LetterHandModelRepeat_RIGHT.SetActive(true);
                    LetterHandModelRepeat_LEFT.SetActive(true);
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = true;
                    segmentPlayer.SetData();
                }
            }
            else
            {
                foreach (GameObject h in Two_handSegmentsRepeat_LETTER)
                {
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = false;
                }
            }
            segment.isReplay = b;
        }
        float lastItemTime = twoSegmentHandList[0].timeStamp.LastOrDefault();
        yield return new WaitForSeconds(lastItemTime);

        foreach (GameObject h in Two_handSegmentsRepeat_LETTER)
        {
            LetterHandModelRepeat_RIGHT.SetActive(false);
            LetterHandModelRepeat_LEFT.SetActive(false);
            SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
            segmentPlayer.enabled = false;
            segmentPlayer.SetData();
        }
        ChangeCameraPosition(1);
        if (!isShow) { ClearDate(); }
    }


    //alphabet reka
    IEnumerator ShowSighnLetter(bool b)
    {
        foreach (Segment segment in segmentList)
        {
            if (b)
            {
                foreach (GameObject h in LetterSegmentsRepeat_RIGHT)
                {
                    LetterHandModelRepeat_RIGHT.SetActive(true);
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = true;
                    segmentPlayer.SetData();
                }
            }
            else
            {
                foreach (GameObject h in LetterSegmentsRepeat_RIGHT)
                {
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = false;
                }
            }
            segment.isReplay = b;
        }

        float lastItemTime = segmentList[0].timeStamp.LastOrDefault();
        yield return new WaitForSeconds(lastItemTime);

        foreach (GameObject h in LetterSegmentsRepeat_RIGHT)
        {
            LetterHandModelRepeat_RIGHT.SetActive(false);
            SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
            segmentPlayer.enabled = false;
            segmentPlayer.SetData();
        }
        ChangeCameraPosition(1);
        if (!isShow) { ClearDate(); }
    }



    //prawa reka
    IEnumerator ShowSighn(bool b)
    {
        foreach (Segment segment in segmentList)
        {
            if (b)
            {
                foreach (GameObject h in handSegmentsRepeat)
                {
                    SecondHandModel.SetActive(true);
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = true;
                    segmentPlayer.SetData();
                }
            }
            else
            {
                foreach (GameObject h in handSegmentsRepeat)
                {
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = false;
                }
            }
            segment.isReplay = b;
        }
        
        float lastItemTime = segmentList[0].timeStamp.LastOrDefault();
        yield return new WaitForSeconds(lastItemTime);

        foreach (GameObject h in handSegmentsRepeat)
        {
            SecondHandModel.SetActive(false);
            SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
            segmentPlayer.enabled = false;
            segmentPlayer.SetData();
        }
        ChangeCameraPosition(1);
        if (!isShow) { ClearDate();}
    }

    IEnumerator ShowSighnLeft(bool b)
    {
        foreach (Segment segment in L_segmentList)
        {
            if (b)
            {
                foreach (GameObject h in L_handSegmentsRepeat)
                {
                    SecondHandModelLeft.SetActive(true);
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = true;
                    segmentPlayer.SetData();
                }
            }
            else
            {
                foreach (GameObject h in L_handSegmentsRepeat)
                {
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = false;
                }
            }
            segment.isReplay = b;
        }
        float lastItemTime = L_segmentList[0].timeStamp.LastOrDefault();
        yield return new WaitForSeconds(lastItemTime);

        foreach (GameObject h in L_handSegmentsRepeat)
        {
            SecondHandModelLeft.SetActive(false);
            SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
            segmentPlayer.enabled = false;
            segmentPlayer.SetData();
        }
        ChangeCameraPosition(1);
        if (!isShow) { ClearDate(); }

    }
    //for two hands
    IEnumerator ShowSighnTwo(bool b)
    {
        //Debug.LogError("ShowSighnTwo");
        foreach (Segment segment in twoSegmentHandList)
        {
            if (b)
            {
                foreach (GameObject h in Two_handSegmentsRepeat)
                {
                    SecondHandModel.SetActive(true);
                    SecondHandModelLeft.SetActive(true);
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = true;
                    segmentPlayer.SetData();
                }
            }
            else
            {
                foreach (GameObject h in Two_handSegmentsRepeat)
                {
                    SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
                    segmentPlayer.enabled = false;
                }
            }
            segment.isReplay = b;
        }
        float lastItemTime = twoSegmentHandList[0].timeStamp.LastOrDefault();
        yield return new WaitForSeconds(lastItemTime);

        foreach (GameObject h in Two_handSegmentsRepeat)
        {
            SecondHandModel.SetActive(false);
            SecondHandModelLeft.SetActive(false);
            SegmentPlayer segmentPlayer = h.GetComponent<SegmentPlayer>();
            segmentPlayer.enabled = false;
            segmentPlayer.SetData();
        }
        ChangeCameraPosition(1);
        if (!isShow) { ClearDate(); }
    }


    //for one hand
    public void AddSegmentData(string TableName, int _RH, int _LH)
    {
        if (_RH == 1 && _LH == 0)
        {
            RecordDataSegment(segmentList, TableName); //right hand
        }
        if (_LH == 1 && _RH == 0)
        {
            RecordDataSegment(L_segmentList, TableName); //left hand
        }
        if (_RH == 1 && _LH == 1)
        {
            RecordDataSegment(segmentList, TableName); //right hand
            RecordDataSegment(L_segmentList, TableName); //left hand
        }
    }

    void RecordDataSegment(List<Segment> sList, string TableName)
    {
        foreach (Segment segment in sList)
        {
            Debug.Log("Start LH");

            string timeString = "";
            string pos = "";
            string rot = "";
            for (int i = 0; i < segment.timeStamp.Count; i++)
            {
                timeString += ConvertToString(segment.timeStamp[i]) + " ";
                pos += ConvertToString(segment.position[i].x) + " " + ConvertToString(segment.position[i].y) + " " + ConvertToString(segment.position[i].z) + ";";
                rot += ConvertToString(segment.rotation[i].x) + " " + ConvertToString(segment.rotation[i].y) + " " + ConvertToString(segment.rotation[i].z) + " " + ConvertToString(segment.rotation[i].w) + ";";
            }
            SQLiteDB.instance.InsertNewSegment(timeString, pos, rot, TableName);
        }
    }


    string ConvertToString(float a)
    {
        NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        return a.ToString(nfi);
    }

    public void Message(string text)
    {
        GameObject msgObj = Instantiate(prefMesaage);
        msgObj.transform.SetParent(prefMessageManager.transform);
        Text msg = msgObj.transform.GetChild(0).GetComponent<Text>();
        msg.text = text;
        DestroyMessage(msgObj);
    }
    void DestroyMessage(GameObject g)
    {
        Destroy(g, 2);
    }

    public void InstantiateWordSlot(string name)
    {
        GameObject GO = Instantiate(WordSlotPrefab, AlphabetTransform);
        WordSlot wordSlot = GO.GetComponent<WordSlot>();
        wordSlot.textWord.text = name;
    }

    public void InstantiateDictionaryWordSlot(string name)
    {
        GameObject GO = Instantiate(DictionarySlotPrefab, DictionaryTransform);
        WordSlot wordSlot = GO.GetComponent<WordSlot>();
        wordSlot.textWord.text = name;
    }

    public void ClearDate()
    {
        foreach (var segment in segmentList)
        {
            segment.isRecord = false;
            segment.isReplay = false;
            segment.ResetData();
        }
        foreach (var segment in L_segmentList)
        {
            segment.isRecord = false;
            segment.isReplay = false;
            segment.ResetData();
        }
       
    }

}
