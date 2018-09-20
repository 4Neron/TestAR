using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LocationService : MonoBehaviour
{
    public static LocationService Instance { get; set; }

    public Vector2 coords;
    private Vector2 homeCoords;
    public Text LocationServiceText;
    public Text LocationServiceDeltaText;
    public bool isLoopEnabled;
    private bool isBadAccuracy;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetHomeCoords()
    {
        homeCoords.x = coords.x;
        homeCoords.y = coords.y;
        PlayerPrefs.SetFloat("Latitude", coords.x);
        PlayerPrefs.SetFloat("Longtitude", coords.y);
    }

    public void UpdateCoords(Text text)
    {
        print("Starded func");
        StartCoroutine(GetLocation(text));
    }

    public void StartUpdate()
    {
        isLoopEnabled = true;
        StartCoroutine(LoopUpdateCoroutine());
    }

    public void StopUpdate()
    {
        isLoopEnabled = false;
    }

    public void LeftLocation()
    {
        StopUpdate();
        GameObject bye = Resources.Load<GameObject>("Prefabs/ByeScreen");
        GameObject currentInstanceBye = Instantiate(bye, bye.transform.position, Quaternion.identity);
        GameObject canvas = GameObject.Find("Canvas");
        currentInstanceBye.transform.SetParent(canvas.transform, false);
        UnityARCameraManager cameraManager = GameObject.Find("ARCameraManager").GetComponent<UnityARCameraManager>();
        cameraManager.ToggleVideo();
    }

    private IEnumerator LoopUpdateCoroutine()
    {
        while (isLoopEnabled)
        {
            if (LocationServiceText != null)
            {
                if (!Input.location.isEnabledByUser)
                {
                    print("Disabled by user");
                }

                // Start service before querying location
                Input.location.Start(10f);

                // Wait until service initializes
                int maxWait = 20;
                while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
                {
                    yield return new WaitForSeconds(1);
                    maxWait--;
                }

                // Service didn't initialize in 20 seconds
                if (maxWait < 1)
                {
                    print("Timed out");
                    yield break;
                }

                // Connection has failed
                if (Input.location.status == LocationServiceStatus.Failed)
                {
                    print("Unable to determine device location");
                    yield break;
                }
                else
                {
                    coords.x = Input.location.lastData.latitude;
                    coords.y = Input.location.lastData.longitude;
                    if (LocationServiceText != null)
                        LocationServiceText.text = coords.x + " , " + coords.y;
                    else
                        print("Text is not initialized");
                    float distance = Vector2.Distance(coords, homeCoords);
                    if (LocationServiceDeltaText != null && Input.location.lastData.horizontalAccuracy < 80f)
                    {
                        LocationServiceDeltaText.text = "Delta : " + distance.ToString("0.#####") + "  " + Input.location.lastData.horizontalAccuracy;
                        if (distance > 0.001f)
                        {
                            LeftLocation();
                        }
                    }
                    else
                    {
                        LocationServiceDeltaText.text = "Bad Accuracy";
                        if (!isBadAccuracy)
                            StartCoroutine(BadAccuracyCoroutine(distance));
                    }
                }

                // Stop service if there is no need to query location updates continuously
                Input.location.Stop();
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator BadAccuracyCoroutine(float delta)
    {
        isBadAccuracy = true;
        yield return new WaitForSeconds(10f);
        if (delta > 0.0007f)
        {
            LeftLocation();
            isBadAccuracy = false;
        }
        else
        {
            isBadAccuracy = false;
            yield break;
        }
    }

    private IEnumerator GetLocation(Text text)
    {
        if (text != null)
            text.text = "Locating your coordinates...";
        yield return new WaitForSeconds(4f);
        // First, check if user has location service enabled     
        if (!Input.location.isEnabledByUser)
        {
            print("Disabled by user");
        }


        // Start service before querying location
        Input.location.Start(10f);

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            coords.x = Input.location.lastData.latitude;
            coords.y = Input.location.lastData.longitude;
            if (text != null)
                text.text = "Lat : " + coords.x + " Lon : " + coords.y;
            else
                print("Text is not initialized");
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
}
