using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARFace))]
public class EyeTracker : MonoBehaviour
{
    [SerializeField] private GameObject leftEyePrefab;
    [SerializeField] private GameObject rightEyePrefab;

    private GameObject _leftEye, _rightEye;

    private ARFace _arFace;

    private void Awake()
    {
        _arFace = GetComponent<ARFace>();
    }

    private void OnEnable()
    {
        var faceManager = FindObjectOfType<ARFaceManager>();
        
        /* Ensuring we found the Face manager and that it's subsystem exists
         before checking that the device supports eye tracking */
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
        _arFace.updated -= OnUpdated;
        SetVisibility(false);
    }

    void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
    {
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

        var isVisible = (_arFace.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready);
        SetVisibility(isVisible);
    }

    void SetVisibility(bool isVisible)
    {
        if (_leftEye != null && _rightEye != null)
        {
            _leftEye.SetActive(isVisible);
            _rightEye.SetActive(isVisible);
        }
    }

}
