using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocker : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 300;
        QualitySettings.vSyncCount = 0;
    }
}
