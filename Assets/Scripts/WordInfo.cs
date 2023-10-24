using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordInfo : MonoBehaviour
{

    [SerializeField] public Text variations;

    public void SetUp(string var)
    {
        variations.text = var;
    }
}
