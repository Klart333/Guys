using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenande : Weapon
{
    [SerializeField]
    private float _detonationTime = 5;

    [SerializeField]
    private float _explosionRadius = 5;

    [SerializeField]
    private float _explosionTerrainForce = 10;

    [SerializeField]
    private float _yStretch = 8;

    private MarchingCubes _marchingCubes;

    private float timer = 0;

    private void Start()
    {
        _marchingCubes = FindObjectOfType<MarchingCubes>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= _detonationTime)
        {
            timer = 0;
            Explode();
        }
    }

    private void Explode()
    {
        _marchingCubes.IncreaseAtPosition(transform.position, _explosionRadius, _explosionTerrainForce, _yStretch);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().velocity *= 0.5f;
    }
}
