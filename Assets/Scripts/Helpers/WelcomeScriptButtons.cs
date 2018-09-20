using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeScriptButtons : MonoBehaviour
{
    public void NopeButton()
    {
        Text textUi = GameObject.Find("LocationString").GetComponent<Text>();
        if (textUi != null)
        {
            if (LocationService.Instance != null)
                print("Location Service is not null");
            LocationService.Instance.UpdateCoords(textUi);
        }
    }

    public void YesButton()
    {
        GameObject wlc = GameObject.FindGameObjectWithTag("Welcome");
        Destroy(wlc);
        LocationService.Instance.SetHomeCoords();
        GameObject ui = Resources.Load<GameObject>("Prefabs/UI");
        GameObject currentInstance = Instantiate(ui, ui.transform.position, Quaternion.identity);
        GameObject canvas = GameObject.Find("Canvas");
        currentInstance.transform.SetParent(canvas.transform, false);
        UnityARCameraManager cameraManager = GameObject.Find("ARCameraManager").GetComponent<UnityARCameraManager>();
        cameraManager.enabled = true;
    }
}
