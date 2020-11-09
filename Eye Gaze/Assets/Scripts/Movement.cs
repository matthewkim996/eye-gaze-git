using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class Movement : MonoBehaviour
{
    public Text isMoving;
    [SerializeField] private GameObject cursorPrefab;
    private GameObject _cursor;
    private bool _isReady;
    
    private float _mid;
    private float _max;
    private float _min;

    private Rect _size;
    private Vector2 _screenMid;
    private float _screenMax;
    private float _screenMin;
    private float _pixelsPerEyeMovement;
    private float[] _eyeValueAtLocation;
    private ARFace _arFace;

    private void Awake()
    {
        _size = GetComponent<RectTransform>().rect;
        _screenMid = new Vector2(_size.width / 2, _size.height / 2);
        _screenMax = _size.height - 150;
        _screenMin = 50;
    }

    public void StartMoving(float[] eyeValueAtLocation, ARFace arFace)
    {
        _eyeValueAtLocation = eyeValueAtLocation;
        _arFace = arFace;
        
        var position = new Vector3(_screenMid.x, _screenMid.y, 0);
        _cursor = Instantiate(cursorPrefab, position, Quaternion.identity);
        _cursor.transform.SetParent(transform, true);
        isMoving.text = "IsMoving: True";
        FindDistances();
        _isReady = true;
    }

    private void FindDistances()
    {
        var distanceFromMidToMaxScreenSpace = (_size.height - 150) - (_size.height / 2);
        var distanceFromMidToMaxWorldSpace = _eyeValueAtLocation[1] - _eyeValueAtLocation[0];
        _pixelsPerEyeMovement =  distanceFromMidToMaxWorldSpace / distanceFromMidToMaxScreenSpace;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isReady) return;
        var rotationValue = (_arFace.leftEye.transform.rotation.x -_eyeValueAtLocation[0]) / _pixelsPerEyeMovement;
        _cursor.transform.position =
            new Vector3(_screenMid.x, _screenMid.y + rotationValue, 0);
        isMoving.text = "IsMoving: True " + _screenMid.y + rotationValue;
    }
}
