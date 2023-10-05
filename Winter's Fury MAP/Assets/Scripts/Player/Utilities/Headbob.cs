using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Headbob
{
    [SerializeField] private Transform playerCam;
    [Range(0.01f, 0.1f), SerializeField] private float amount;
    [Range(1f, 30f), SerializeField] private float frequency;
    [Range(10f, 100f), SerializeField] private float smooth;
    private Vector3 startPos;

    public void Setup()
    {
        startPos = playerCam.localPosition;
    }

    public void StartHeadBob()
    {
        Vector3 pos = Vector3.zero;

        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency / 2f) * amount * 1.6f, smooth * Time.deltaTime);

        playerCam.localPosition += pos;
    }

    public void ResetHeadBob()
    {
        if (playerCam.localPosition == startPos) return;

        playerCam.localPosition = Vector3.Lerp(playerCam.localPosition, startPos, 10f * Time.deltaTime);

        if (Vector3.Distance(playerCam.localPosition, startPos) <= 0.001f) playerCam.localPosition = startPos;
    }
}
