using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGameObjectOnAwake : MonoBehaviour
{
    [SerializeField] private GameObject spawnable;
    private void Awake()
    {
        Instantiate(spawnable);
    }
}
