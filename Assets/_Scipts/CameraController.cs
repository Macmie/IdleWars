using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _movementSpeed;
    private float _xMovement, _zMovement;

    // Update is called once per frame
    void Update()
    {
        SetCameraSpeed();
    }

    private void SetCameraSpeed()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _xMovement = horizontal * _movementSpeed * Time.deltaTime;
        _zMovement = vertical * _movementSpeed * Time.deltaTime;

        Vector3 moveVector = new Vector3(_xMovement, 0, _zMovement);
        transform.Translate(moveVector, Space.World);
    }
}
