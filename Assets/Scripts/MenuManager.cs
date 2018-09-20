using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public GameObject[] prefabs;
    public UnityARCameraManager cameraManager;
    private GameObject currentInstance;

    // Use this for initialization
    void Start()
    {

        //if (PlayerPrefs.HasKey("Latitude") || PlayerPrefs.HasKey("Longtitude") && false)
        //{

        //}
        //else
        //{
        GameObject wlc = prefabs[0];
        currentInstance = Instantiate(wlc, wlc.transform.position, Quaternion.identity);
        currentInstance.transform.SetParent(transform, false);
        cameraManager.enabled = false;
        UpdateLocationString();
        //}
    }

    private void UpdateLocationString()
    {
        GameObject go = GameObject.Find("LocationString");
        if (go != null)
            print("object found");
        Text textUi = go.GetComponent<Text>();
        if (textUi != null)
        {
            LocationService.Instance.UpdateCoords(textUi);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
