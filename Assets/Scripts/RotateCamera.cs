﻿using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float speed = 5f;
    private Transform _rotator;

    public void Start()
    {
        _rotator = GetComponent<Transform>();
    }

    public void Update()
    {
        _rotator.Rotate(0, speed * Time.deltaTime, 0);
    }
}