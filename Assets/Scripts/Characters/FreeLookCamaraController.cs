using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookCamaraController : MonoBehaviour
{
    CinemachineFreeLook vcam;
    // Start is called before the first frame update
    public float zoomSpeed = 20;
    public float minField = 10;
    public float maxField = 60;
    void Start()
    {
        vcam = GetComponent<CinemachineFreeLook>();
        vcam.m_Lens.FieldOfView = 40;
    }

    // Update is called once per frame
    void Update()
    {
        
        float FieldOfView = -Input.GetAxis("Mouse ScrollWheel");
        ZoomInOut(FieldOfView);
    }
    void ZoomInOut(float FieldOfView)
    {
        float currentView = vcam.m_Lens.FieldOfView + FieldOfView * zoomSpeed;
        if ( (currentView > minField) && (currentView < maxField))
        {
            vcam.m_Lens.FieldOfView = currentView; 
        }

    }
}
