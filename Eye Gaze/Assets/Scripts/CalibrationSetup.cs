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
    private GameObject _child;
    [SerializeField] private GameObject calibrationIcon;
    
    private bool _isCalibrated; 
    
    // mid, max, min hold the values of eye rotation at the middle, top, and bottom of the screen
    // NOTE: at the moment we are only detecting x rotational values, these are converted to 2D y movement values
    private Rect _size;
    private float _calibrationIconSize;
    private float _mid, _max, _min;
    private int _location;
    private float[] _eyeValueAtLocation;
    private Vector2[] _calibrationLocation; // size 17 
    private ARFace _arFace;

    private void Awake()
    {
        _location = 0;
        _eyeValueAtLocation = new float[13];
        _calibrationLocation = new Vector2[13];
        _size = gameObject.GetComponent<RectTransform>().rect;
        _calibrationIconSize = calibrationIcon.GetComponent<RectTransform>().rect.width;
    }

    private void Start()
    {
        _calibrationLocation[0] = new Vector2(_size.width / 2, _size.height / 2);
        _calibrationLocation[1] = new Vector2(_size.width / 2, _size.height - 150);
        _calibrationLocation[2] = new Vector2(_size.width / 2, _calibrationIconSize);
        _calibrationLocation[3] = new Vector2(_calibrationIconSize, _size.height / 2);
        _calibrationLocation[4] = new Vector2(_size.width - _calibrationIconSize, _size.height / 2);
        _calibrationLocation[5] = new Vector2(_calibrationIconSize, _size.height - 150);
        _calibrationLocation[6] = new Vector2(_size.width - _calibrationIconSize, _size.height - 150);
        _calibrationLocation[7] = new Vector2(_calibrationIconSize, _calibrationIconSize);
        _calibrationLocation[8] = new Vector2(_size.width - _calibrationIconSize, _calibrationIconSize);
        _calibrationLocation[9] = new Vector2(_size.width / 4, _size.height - (_size.height / 4));
        _calibrationLocation[10] = new Vector2(_size.width - (_size.width / 4), _size.height - (_size.height / 4));
        _calibrationLocation[11] = new Vector2(_size.width / 4, _size.height / 4);
        _calibrationLocation[12] = new Vector2(_size.width - (_size.width / 4), _size.height / 4);
    }

    public void StartCalibration(ARFace arFace)
    {
        // if we have already calibrated before just exit the method
        if (_isCalibrated) return;
        
        /* currently the size of the device is hardcoded using the Canvas gameobject
         it is set to pixel size of Iphone 12, which will also work on Iphone X and up 
         we will need to change this eventually to something dynamic */
        _arFace = arFace;
        
        /* we call the setup method through a coroutine so that we can pause for a few
         seconds before calibration */
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        /* we run a while loop to go through the current 3 calibration locations
         location = 0 is middle of screen, 1 is top, and 2 is bottom */
        while (_location <= 2)
        {
            var position = new Vector3(_calibrationLocation[_location].x, _calibrationLocation[_location].y, 0);
            
            // spawn the calibration icon and set the parent so it is visible
            _child = Instantiate(calibrationIcon, position, Quaternion.identity);
            _child.transform.SetParent(transform, true);

            isCalibrating.text = " Calibration: TRUE " + _location;
            
            // take the sum of 6 rotation values so that we can get an avg value
            for (var i = 0; i < 6; i++)
            {
                yield return new WaitForSeconds(1);
                _eyeValueAtLocation[_location] += _arFace.leftEye.rotation.x;
            }

            _eyeValueAtLocation[_location] /= 6;

            // update location value and destroy the current calibration prefab
            _location++;
            Destroy(_child);
        }

        // call startmoving so we can setup the cursor prefab
        _isCalibrated = true;
        gameObject.GetComponent<Movement>().StartMoving(_eyeValueAtLocation, _arFace);
    }

    private void Update()
    {
        /*foreach (var i in _calibrationLocation)
        {
            var position = new Vector3(i.x, i.y, 0);
            _child = Instantiate(calibrationIcon, calibrationIcon.transform.position, Quaternion.identity);
            _child.transform.SetParent(transform, true);
        }*/
    }
}
