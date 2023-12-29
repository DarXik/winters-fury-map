using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseEffect : MonoBehaviour
{


    public Animator animator;
    public static PulseEffect Instance { get; set; }

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartPulsation()
    {
        animator.SetBool("Pulsating", true);
    }

    public void StopPulsation()
    {
        animator.SetBool("Pulsating", false);
    }
}