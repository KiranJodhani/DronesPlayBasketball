﻿using UnityEngine;

public class RotatorCustom : MonoBehaviour
{
    [Range(-1.0f, 1.0f)]
    public float xForceDirection = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float yForceDirection = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float zForceDirection = 0.0f;

    public float speedMultiplier = 1;

    public Space spacePivot = Space.Self;

    void FixedUpdate()
    {
        transform.Rotate(xForceDirection * speedMultiplier, yForceDirection * speedMultiplier, zForceDirection * speedMultiplier, spacePivot);
    }
}