using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Segment : ScriptableObject
{
    public bool isRecord = false;
    public bool isReplay = false;
    public float recordFrequency = 249;

    public List<float> timeStamp;
    public List<Vector3> position;
    public List<Quaternion> rotation;
   
    public void ResetData()
    {
        timeStamp.Clear();
        position.Clear();
        rotation.Clear();
    }
}
