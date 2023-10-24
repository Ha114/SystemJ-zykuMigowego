using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentRecorder : MonoBehaviour
{
    public Segment segment;
    private float timer;
    private float timeValue;

    public void SetTime() {
        timeValue = 0;
        timer = 0;
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        timeValue += Time.unscaledDeltaTime;
        if (segment.isRecord & timer >= 1 / segment.recordFrequency)
        {
            segment.timeStamp.Add(timeValue);
            segment.position.Add(this.transform.position);
            segment.rotation.Add(this.transform.rotation);
            timer = 0;
        }
    }
}
