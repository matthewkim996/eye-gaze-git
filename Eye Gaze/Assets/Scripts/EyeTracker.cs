using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// This script will NOT run if a component of type ARFace is not attached to the object
[RequireComponent(typeof(ARFace))]
public class EyeTracker : MonoBehaviour
{
    // We serialized these GameObjects so that we can assign them through the unity editor
    [SerializeField] private GameObject leftEyePrefab;
    [SerializeField] private GameObject rightEyePrefab;
    
    private GameObject _leftEye;
    private GameObject _rightEye;

    private ARFace _arFace;
    
    private void Awake()
    {
        // When the script is loaded get the ARFace component attached to the object
        _arFace = GetComponent<ARFace>();
    }

    private void OnEnable()
    {
        var faceManager = FindObjectOfType<ARFaceManager>();
        
        /* IF the faceManager is not null, ensure that its subsystem is not null,
         then make sure the device supports eye tracking - if it does set the ARFace to run
         our OnUpdated code every time ARFace updates, if it doesn't let the console know */
        if (faceManager != null && faceManager.subsystem != null && faceManager.subsystem.SubsystemDescriptor.supportsEyeTracking)
        {
            _arFace.updated += OnUpdated;
        }
        else
        {
            Debug.LogError("This device does not support eye tracking");
        }
    }

    private void OnDisable()
    {
        // When the script is disabled remove our OnUpdated code from the ARFace's updated code
        _arFace.updated -= OnUpdated;
        SetVisibility(false);
    }

    private void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
    {
        /* if the ARFace detects eyes, and the eye is null instantiate the eye prefab
         at the ARFace eye Transform location */
        if (_arFace.leftEye != null && _leftEye == null)
        {
            _leftEye = Instantiate(leftEyePrefab, _arFace.leftEye);
            _leftEye.SetActive(false);
        }
        if (_arFace.rightEye != null && _rightEye == null)
        {
            _rightEye = Instantiate(rightEyePrefab, _arFace.rightEye);
            _rightEye.SetActive(false);
        }
        
        /* if ARFace's tracking state is currently tracking, and it's state is currently
         initializing or tracking (both return greater values than ARSessionState.Ready) then
         set visibility to true, else false */
        var isVisible = (_arFace.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready);
        SetVisibility(isVisible);
    }

    private void SetVisibility(bool isVisible)
    {
        // as long as neither of the eyes are null update their active values
        if (_leftEye == null || _rightEye == null) return;
        _leftEye.SetActive(isVisible);
        _rightEye.SetActive(isVisible);
    }

}
