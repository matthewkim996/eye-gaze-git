using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class EyeTrackerSupported : MonoBehaviour
{
    [SerializeField] private Text eyeTrackerSupportedText;
    private void OnEnable()
    {
        var faceManager = FindObjectOfType<ARFaceManager>();
        
        
        /* Ensuring we found the Face manager and that it's subsystem exists
         before checking that the device supports eye tracking */
        if (faceManager != null && faceManager.subsystem != null && faceManager.subsystem.SubsystemDescriptor.supportsEyeTracking)
        {
            
        }
    }

}
