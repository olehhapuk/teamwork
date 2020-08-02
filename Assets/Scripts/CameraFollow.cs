using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    private float _myZCoords;
    private Vector3 _mousePos;
    private Vector3 _desiredPosition;

    private void Start()
    {
        _myZCoords = transform.position.z;
    }

    private void Update()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void LateUpdate()
    {
        _desiredPosition = (_mousePos + followTarget.position) / 2;
        _desiredPosition = (_desiredPosition + followTarget.position) / 2;
        _desiredPosition = (_desiredPosition + followTarget.position) / 2;
        _desiredPosition.z = _myZCoords;
        transform.position = _desiredPosition;
    }
}
