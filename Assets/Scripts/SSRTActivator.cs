using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSRTActivator : MonoBehaviour
{
    SSRT ssrt;
    void Start()
    {
        ssrt = GetComponent<SSRT>();
        ssrt.enabled = true;
        ssrt.lightOnly = false;
        ssrt.directLightingAO = true;
        ssrt.resolutionDownscale = SSRT.ResolutionDownscale.Full;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
