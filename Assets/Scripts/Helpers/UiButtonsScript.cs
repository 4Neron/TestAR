using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonsScript : MonoBehaviour
{

    public Text textUi;
    public Text deltaUi;
    private UnityARCameraManager cameraManager;

    // Use this for initialization
    void Start()
    {
        if (LocationService.Instance != null)
        {
            LocationService.Instance.LocationServiceText = textUi;
            LocationService.Instance.LocationServiceDeltaText = deltaUi;
            LocationService.Instance.StartUpdate();
        }

        cameraManager = GameObject.Find("ARCameraManager").GetComponent<UnityARCameraManager>();
    }

    public void Pause()
    {
        cameraManager.ToggleVideo();
    }

    public void StopDetection()
    {
        cameraManager.ToggleDetection();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
