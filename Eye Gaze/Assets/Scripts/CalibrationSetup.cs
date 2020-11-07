using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CalibrationSetup : MonoBehaviour
{
    /* IsCalibrating is used for debugging purposes
     child is used to spawn calibration prefabs
     calibrationIcon is set within the unity editor with the calibration prefab*/
    public Text isCalibrating;
    private GameObject child;
    [SerializeField] private GameObject calibrationIcon;
    
    private bool _isCalibrated; 
    // mid, max, min hold the values of eye rotation at the middle, top, and bottom of the screen
    // NOTE: at the moment we are only detecting x rotational values, these are converted to 2D y movement values
    private float _mid, _max, _min;
    private int _location;
    
    private ARFace _arFace;
    public void StartCalibration(ARFace arFace)
    {
        // if we have already calibrated before just exit the method
        if (_isCalibrated) return;
        
        /* currently the size of the device is hardcoded using the Canvas gameobject
         it is set to pixel size of Iphone 12, which will also work on Iphone X and up 
         we will need to change this eventually to something dynamic */
        var size = gameObject.GetComponent<RectTransform>().rect;
        _arFace = arFace;
        
        /* we call the setup method through a coroutine so that we can pause for a few
         seconds before calibration */
        StartCoroutine(Setup(size, 0));
    }

    private IEnumerator Setup(Rect size, int location)
    {
        /* we run a while loop to go through the current 3 calibration locations
         location = 0 is middle of screen, 1 is top, and 2 is bottom */
        _location = location;
        while (_location <= 2)
        {
            float height = 0;
            var width = size.width / 2;
            switch (_location)
            {
                case 0:
                    height = size.height / 2;
                    break;
                case 1:
                    height = size.height - 150;
                    break;
                case 2:
                    height = 50;
                    break;
            }

            var position = new Vector3(width, height, 0);
            
            // spawn the calibration icon and set the parent so it is visible
            child = Instantiate(calibrationIcon, position, Quaternion.identity);
            child.transform.SetParent(transform, true);

            isCalibrating.text = " Calibration: TRUE " + _location;
            
            // take the sum of 6 rotation values so that we can get an avg value
            for (var i = 0; i < 6; i++)
            {
                yield return new WaitForSeconds(1);
                switch (_location)
                {
                    case 0:
                        _mid += _arFace.leftEye.transform.rotation.x;
                        break;
                    case 1:
                        _max += _arFace.leftEye.transform.rotation.x;
                        break;
                    case 2:
                        _min += _arFace.leftEye.transform.rotation.x;
                        break;
                }
            }

            switch (_location)
            {
                case 0:
                    _mid /= 6;
                    break;
                case 1:
                    _max /= 6;
                    break;
                case 2:
                    _min /= 6;
                    break;
            }

            // update location value and destroy the current calibration prefab
            _location++;
            Destroy(child);
        }

        // call startmoving so we can setup the cursor prefab
        _isCalibrated = true;
        gameObject.GetComponent<Movement>().StartMoving(_mid, _max, _min, _arFace);
    }

}
