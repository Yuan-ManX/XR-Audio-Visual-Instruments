﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granulator_TestInput : MonoBehaviour
{
    public Granulator _Granulator;

    public Vector2 _PosXRange = new Vector2(-3, 3);
    public Vector2 _PosYRange = new Vector2(0, 3);

    public Vector2 _EmitCadence = new Vector2(500, 5);
    public Vector2 _SpeedRange = new Vector2(0f, 2f);


    Vector3 _PrevPos;
    Vector3 _Velocity;
    public float _Smoothing = 8;

    public float _PitchScalar = 1.5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float pos = Mathf.InverseLerp(_PosXRange.x, _PosXRange.y, transform.position.x);
        float pitch = Mathf.InverseLerp(_PosYRange.x, _PosYRange.y, transform.position.y);
        _Granulator._EmitGrainProps.Position = pos;
        _Granulator._EmitGrainProps.Pitch = pitch * _PitchScalar;



        Vector3 newVel = (transform.position - _PrevPos) / Time.deltaTime;
        _Velocity = Vector3.Lerp(_Velocity, newVel, Time.deltaTime * _Smoothing);
        float speedNorm = Mathf.InverseLerp(_SpeedRange.x, _SpeedRange.y, _Velocity.magnitude);
        float emitCadence = Mathf.Lerp(_EmitCadence.x, _EmitCadence.y, speedNorm);
        //_Granulator._TimeBetweenGrains = (int)emitCadence;


        //print("Pos: " + pos + " Pitch: " + pitch * 2);

        _PrevPos = transform.position;
    }
}
