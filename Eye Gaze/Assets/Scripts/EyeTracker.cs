using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// This script will NOT run if a component of type ARFace is not attached to the object
[RequireComponent(typeof(ARFace))]
public class EyeTracker : MonoBehaviour
{
    // We serialized these GameObjects so that we can assign them through the unity editor
    [SerializeField] private GameObject leftEyePrefab;
    [SerializeField] private GameObject rightEyePrefab;
    [SerializeField] private Text eyeTrackerSupportedText;
    
    private GameObject _leftEye;
    private GameObject _rightEye;
    private GameObject _lineLeft;
    private GameObject _lineRight;
    public ARFace arFace;
    
    private void Awake()
    {
        // When the script is loaded get the ARFace component attached to the object
        arFace = GetComponent<ARFace>();
        eyeTrackerSupportedText = GameObject.FindWithTag("DebugText").GetComponent<Text>();
    }

    private void OnEnable()
    {
        var faceManager = FindObjectOfType<ARFaceManager>();

        /* IF the faceManager is not null, ensure that its subsystem is not null,
         then make sure the device supports eye tracking - if it does set the ARFace to run
         our OnUpdated code every time ARFace updates, if it doesn't let the console know */
        if (faceManager != null && faceManager.subsystem != null && faceManager.subsystem.SubsystemDescriptor.supportsEyeTracking)
        {
            _lineLeft = new GameObject();
            _lineRight = new GameObject();
            arFace.updated += OnUpdated;
        }
        else
        {
            
        }
    }

    private void OnDisable()
    {
        // When the script is disabled remove our OnUpdated code from the ARFace's updated code
        arFace.updated -= OnUpdated;
        SetVisibility(false);
    }

    private void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
    {
        // Checking if the eyes are ready before we try to calibrate
        var isReadyL = false;
        var isReadyR = false;
        /* if the ARFace detects eyes, and the eye is null instantiate the eye prefab
         at the ARFace eye Transform location */
        if (arFace.leftEye != null && _leftEye == null)
        {
            _leftEye = Instantiate(leftEyePrefab, arFace.leftEye);
            _leftEye.SetActive(false);
            isReadyL = true;
        }
        if (arFace.rightEye != null && _rightEye == null)
        {
            _rightEye = Instantiate(rightEyePrefab, arFace.rightEye);
            _rightEye.SetActive(false);
            isReadyR = true;
        }

        // if both eyes are ready start calibration
        if (isReadyL && isReadyR)
        {
            GameObject.FindWithTag("CanvasUI").GetComponent<CalibrationSetup>().StartCalibration(arFace);
            isReadyL = false;
            isReadyR = false;
        }
        Transform transform1;
        eyeTrackerSupportedText.text = " Left: \nPosition: (" +(transform1 = arFace.leftEye.transform).position.x.ToString("F5") + ", " + transform1.position.y.ToString("F5") 
                                       + ", " + transform1.position.z.ToString("F5") + ")"
                                       + "\n Rotation: (" + transform1.rotation.x.ToString("F5") + ", " + transform1.rotation.y.ToString("F5") + ", " + 
                                       transform1.rotation.z.ToString("F5") + ")"+ "\n"
                                       + "Right: \nPosition: " +(transform1 = arFace.rightEye.transform).position.ToString()
                                       + "\n Rotation: " + transform1.rotation.ToString();
        /* if ARFace's tracking state is currently tracking, and it's state is currently
         initializing or tracking (both return greater values than ARSessionState.Ready) then
         set visibility to true, else false */
        var isVisible = (arFace.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready);
        SetVisibility(isVisible);
    }

    private void SetVisibility(bool isVisible)
    {
        // as long as neither of the eyes are null update their active values
        if (_leftEye == null || _rightEye == null) return;
        _leftEye.SetActive(isVisible);
        _rightEye.SetActive(isVisible);
    }
    
    private static IEnumerator Wait()
    {
        yield return new WaitForSeconds(8f);
    }

}
