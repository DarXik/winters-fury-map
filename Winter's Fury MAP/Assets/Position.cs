using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    public static Position Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }
}
