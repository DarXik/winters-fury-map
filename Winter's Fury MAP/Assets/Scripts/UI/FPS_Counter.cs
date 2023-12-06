using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS_Counter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;

    private Dictionary<int, string> CachedNumberStrings = new();

    private int[] _frameRateSamples;
    private int _cacheNumbersAmount = 300;
    private int _averageFromAmount = 30;
    private int _averageCounter;
    private int _currentAveraged;

    void Awake()
    {
        // Cache strings and create array
        {
            for (int i = 0; i < _cacheNumbersAmount; i++) {
                CachedNumberStrings[i] = i.ToString();
            }

            _frameRateSamples = new int[_averageFromAmount];
        }
    }

    void Update()
    {
        // Sample
        {
            var currentFrame = (int)Math.Round(1f / Time.smoothDeltaTime); // Use unscaledDeltaTime for more accurate, or if your game modifies Time.timeScale.
            _frameRateSamples[_averageCounter] = currentFrame;
        }

        // Average
        {
            var average = 0f;

            foreach (var frameRate in _frameRateSamples) {
                average += frameRate;
            }

            _currentAveraged = (int)Math.Round(average / _averageFromAmount);
            _averageCounter = (_averageCounter + 1) % _averageFromAmount;
        }

        // Assign to UI
        {
            switch (_currentAveraged)
            {
                case var x when x >= 0 && x < _cacheNumbersAmount:
                    fpsText.text = $"FPS: {CachedNumberStrings[x]}";
                    break;
                case var x when x >= _cacheNumbersAmount:
                    fpsText.text = $"> {_cacheNumbersAmount} FPS";
                    break;
                case var x when x < 0:
                    fpsText.text = "";
                    break;
                default:
                    fpsText.text = "?";
                    break;
            }
        }
    }
}
