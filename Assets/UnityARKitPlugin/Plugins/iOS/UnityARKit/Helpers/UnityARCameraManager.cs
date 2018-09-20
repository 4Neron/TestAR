using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class UnityARCameraManager : MonoBehaviour
{

    public Camera m_camera;
    private UnityARSessionNativeInterface m_session;
    private Material savedClearMaterial;

    [Header("AR Config Options")]
    public UnityARAlignment startAlignment = UnityARAlignment.UnityARAlignmentGravity;
    public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.Horizontal;
    public ARReferenceImagesSet detectionImages = null;
    public bool getPointCloud = true;
    public bool enableLightEstimation = true;
    public bool enableAutoFocus = true;
    private bool sessionStarted = false;
    private ARKitWorldTrackingSessionConfiguration config;
    private bool isDetecting = true;

    // Use this for initialization
    void Start()
    {

        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

        Application.targetFrameRate = 60;
        config = new ARKitWorldTrackingSessionConfiguration();
        config.planeDetection = planeDetection;
        config.alignment = startAlignment;
        config.getPointCloudData = getPointCloud;
        config.enableLightEstimation = enableLightEstimation;
        config.enableAutoFocus = enableAutoFocus;
        if (detectionImages != null)
        {
            config.arResourceGroupName = detectionImages.resourceGroupName;
        }

        if (config.IsSupported)
        {
            m_session.RunWithConfig(config);
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
        }

        if (m_camera == null)
        {
            m_camera = Camera.main;
        }
    }

    public void ToggleVideo()
    {
        UnityARVideo arVideo = m_camera.gameObject.GetComponent<UnityARVideo>();
        ARKitWorldTrackingSessionConfiguration _config = new ARKitWorldTrackingSessionConfiguration();
        if (arVideo.enabled && arVideo != null)
        {
            _config.enableAutoFocus = false;
            _config.planeDetection = UnityARPlaneDetection.None;
            _config.enableLightEstimation = false;
            _config.getPointCloudData = false;
            m_session.Pause();
            arVideo.enabled = false;
        }
        else
        {
            _config.planeDetection = planeDetection;
            _config.alignment = startAlignment;
            _config.getPointCloudData = getPointCloud;
            _config.enableLightEstimation = enableLightEstimation;
            _config.enableAutoFocus = enableAutoFocus;
            m_camera.clearFlags = CameraClearFlags.SolidColor;
            m_session.RunWithConfig(_config);
            arVideo.enabled = true;
        }

    }

    public void ToggleDetection()
    {
        ARKitWorldTrackingSessionConfiguration _config = config;
        if (isDetecting)
        {
            _config.planeDetection = UnityARPlaneDetection.None;
            _config.alignment = startAlignment;
            _config.getPointCloudData = false;
            isDetecting = false;
        }
        else
        {
            _config = new ARKitWorldTrackingSessionConfiguration();
            _config.planeDetection = planeDetection;
            _config.alignment = startAlignment;
            _config.getPointCloudData = getPointCloud;
            _config.enableLightEstimation = enableLightEstimation;
            _config.enableAutoFocus = enableAutoFocus;
            isDetecting = true;
        }
        m_session.RunWithConfig(_config);
    }

    void FirstFrameUpdate(UnityARCamera cam)
    {
        sessionStarted = true;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
    }

    public void SetCamera(Camera newCamera)
    {
        if (m_camera != null)
        {
            UnityARVideo oldARVideo = m_camera.gameObject.GetComponent<UnityARVideo>();
            if (oldARVideo != null)
            {
                savedClearMaterial = oldARVideo.m_ClearMaterial;
                Destroy(oldARVideo);
            }
        }
        SetupNewCamera(newCamera);
    }

    private void SetupNewCamera(Camera newCamera)
    {
        m_camera = newCamera;

        if (m_camera != null)
        {
            UnityARVideo unityARVideo = m_camera.gameObject.GetComponent<UnityARVideo>();
            if (unityARVideo != null)
            {
                savedClearMaterial = unityARVideo.m_ClearMaterial;
                Destroy(unityARVideo);
            }
            unityARVideo = m_camera.gameObject.AddComponent<UnityARVideo>();
            unityARVideo.m_ClearMaterial = savedClearMaterial;
        }
    }

    // Update is called once per frame

    void Update()
    {

        if (m_camera != null && sessionStarted)
        {
            // JUST WORKS!
            Matrix4x4 matrix = m_session.GetCameraPose();
            m_camera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix);
            m_camera.transform.localRotation = UnityARMatrixOps.GetRotation(matrix);

            m_camera.projectionMatrix = m_session.GetCameraProjection();
        }

    }

}
