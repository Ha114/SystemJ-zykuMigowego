using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPlayer : MonoBehaviour
{
    #region singleton
    public static SegmentPlayer instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion
    public Segment segment;
    private float timeValue;
    private int index;
    private int index2;

    public void SetData()
    {
        timeValue = 0;
        index = 0;
        index2 = 0;
    }

    private void Update()
    {
        timeValue += Time.unscaledDeltaTime;
        if (segment.isReplay)
        {
            GetIndex();
            SetTransform();
        }
    }

    private void GetIndex()
    {
       // Debug.Log("index = "+ index + ", index2 = "+ index2);
        for (int i = 0; i < segment.timeStamp.Count - 2; i++)
        {
            if (segment.timeStamp[i] == timeValue)
            {
                index = i;
                index2 = i;
                return;
            }
            else if (segment.timeStamp[i] < timeValue & timeValue < segment.timeStamp[i + 1])
            {
                index = i;
                index2 = i + 1;
                return;
            }
        }
        index = segment.timeStamp.Count - 1;
        index2 = segment.timeStamp.Count - 1;
    }

    private void SetTransform()
    {
        if (index == index2)    //index -> timeValue from Ghost
        {
            this.transform.position = segment.position[index];
            this.transform.rotation = segment.rotation[index];
        }
        else
        {
            float interpolationFactor = (timeValue - segment.timeStamp[index]) / (segment.timeStamp[index2] - segment.timeStamp[index]);
            this.transform.position = Vector3.Lerp(segment.position[index], segment.position[index2], interpolationFactor);
            this.transform.rotation = Quaternion.Lerp(segment.rotation[index], segment.rotation[index2], interpolationFactor);
        }
    }
}
