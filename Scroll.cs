using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Scroll : MonoBehaviour
{
    private GameObject _leftEye;
    private GameObject _rightEye;
    private GameObject _lineLeft;
    private GameObject _lineRight;

    private float _mid;
    private float _max;
    private float _min;

    private int _location;
    private float _screenTop;
    private float _screenBottom;
    private float _screenLeft;
    private float _screenRight;

    private ARFace _arFace;

//------------------------------------------------------------------------------------------
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
//---------------------------------------------------------------------------------------------
    public Navigate(rect size, int location)
	{ 
        // trying to implement scrolling by clicking on the far left/middle of screen for backwards, right/middle
        // for forward, Top/middle for scroll up, bottom/middle for scroll down.
        _location = location;

        /*
         if(location == left){
            "go back"
        }
         if(location == right){
            "go forward"
        }
        if(location == top){
            "scrollUp.size[3]
        }
        if(location == bottom){
            "scrollDown.size[3]"
        }
         */
    }
}


/*
 import numpy as np

def scroll_pdf(y_avg, y_window):
    """
    Function that decides whether to scroll or not
    Input :
        y_avg : int
            Average ordinate of the centre of the eyes over time
        y_window : list
            List of ordinates of the centre of the eyes over a window of 10 frames
    Return : int
        Returns either 1,-1 or 0 representing a directive to scroll up, down or not scroll at all respectively
    """
    mean_y = np.mean(y_window)
    y_area = sum(y_window) - (len(y_window)) * y_window[0]
    threshold_y = 200
    if y_area > threshold_y:
        if mean_y >= y_avg:
            return 1
        else:
            return 0
    elif y_area < -threshold_y:
        if mean_y > y_avg:
            return 0
        else:
            return -1
    else:
        return 0


def next_page(x_avg, x_window):
    """
    Function that decides whether to turn to a new page or not
    Input :
        x_avg : int
            Average abcissa of the centre of the face over time
        x_window : list
            List of abcissae of the centre of the face over a window of 10 frames
    Return : int
        Returns either 1,-1 or 0 representing a directive to turn to the next page, previous page or not turn at all respectively
    """
    mean_x = np.mean(x_window)
    x_area = sum(x_window) - (len(x_window)) * x_window[0]
    threshold_x = 200
    if x_area > threshold_x:
        if mean_x >= x_avg:
            return 1
        else:
            return 0
    elif x_area < -threshold_x:
        if mean_x > x_avg:
            return 0
        else:
            return -1
    else:
        return 0

if __name__ == '__main__':
    pass
 
 */