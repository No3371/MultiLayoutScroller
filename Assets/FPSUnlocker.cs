using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSUnlocker : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 1000;
        QualitySettings.vSyncCount = 0;
    }
}
