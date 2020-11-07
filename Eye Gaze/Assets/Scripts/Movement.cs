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

    private ARFace _arFace;

    private void Awake()
    {
        _size = GetComponent<RectTransform>().rect;
        _screenMid = new Vector2(_size.width / 2, _size.height / 2);
        _screenMax = _size.height - 150;
        _screenMin = 50;
    }

    public void StartMoving(float mid, float max, float min , ARFace arFace)
    {
        _mid = mid;
        _max = max;
        _min = min;
        _arFace = arFace;
        
        var position = new Vector3(_screenMid.x, _screenMid.y, 0);
        _cursor = Instantiate(cursorPrefab, position, Quaternion.identity);
        _cursor.transform.SetParent(transform, true);
        isMoving.text = "IsMoving: True";
        _isReady = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isReady) return;
        var rotationValue = _arFace.leftEye.transform.rotation.x;
        _cursor.transform.position =
            new Vector3(_screenMid.x, rotationValue > _mid ? _screenMax : _screenMin, 0);
        isMoving.text = "IsMoving: True " + rotationValue;
    }
}
