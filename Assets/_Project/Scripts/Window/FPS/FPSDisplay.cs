using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _updateInterval = 0.5f; 
    [SerializeField] private bool _showMinFPS = true;     
    [SerializeField] private bool _showMaxFPS = true;    

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _fpsText;

    private float _fps;
    private float _minFps = float.MaxValue;
    private float _maxFps = float.MinValue;
    private float _accumulatedFps;
    private int _frameCount;
    private float _timeLeft;

    private void Start()
    {
        _timeLeft = _updateInterval;
    }

    private void Update()
    {
        float currentFps = 1.0f / Time.unscaledDeltaTime;
        _accumulatedFps += currentFps;
        _frameCount++;

        if (currentFps < _minFps) _minFps = currentFps;
        if (currentFps > _maxFps) _maxFps = currentFps;

        _timeLeft -= Time.unscaledDeltaTime;
        if (_timeLeft <= 0.0f)
        {
            _fps = _accumulatedFps / _frameCount;
            UpdateFPSText();
            ResetCounters();
            _timeLeft = _updateInterval;
        }
    }

    private void UpdateFPSText()
    {
        if (_fpsText == null) return;

        string fpsString = $"FPS: {Mathf.Round(_fps)}";
        if (_showMinFPS) fpsString += $"\nMin: {Mathf.Round(_minFps)}";
        if (_showMaxFPS) fpsString += $"\nMax: {Mathf.Round(_maxFps)}";

        _fpsText.text = fpsString;
    }

    private void ResetCounters()
    {
        _accumulatedFps = 0;
        _frameCount = 0;
        _minFps = float.MaxValue;
        _maxFps = float.MinValue;
    }
}
