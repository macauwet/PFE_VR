using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibUpdate : MonoBehaviour
{
    public bool activeCalib = false;
    public SkinnedMeshRenderer anchorRendR;
    public SkinnedMeshRenderer anchorRendL;
    public SkinnedMeshRenderer controllerRendR;
    public SkinnedMeshRenderer controllerRendL;
    // Update is called once per frame
    void Update()
    {
        if (activeCalib && (OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three)))
        {
            CalibrationTool.CalibrateFromTag();
            CalibrationTool.CalibrateFromTag();//need sometime a second pass
        }
        if (activeCalib!= controllerRendR.enabled)
        {
            anchorRendR.enabled = activeCalib;
            anchorRendL.enabled = activeCalib;
            controllerRendL.enabled = activeCalib;
            controllerRendR.enabled = activeCalib;
        }
    }
}
