using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class S_DebugStatsController : MonoBehaviour
{
    public TextMeshProUGUI versionText;
    public TextMeshProUGUI fpsText; // UI Text for displaying the FPS

    private float deltaTime = 0.0f;

    void Start()
    {
        DontDestroyOnLoad( gameObject);
        string version = Resources.Load<TextAsset>("version").text.Trim();
        versionText.text = "Version " + version;
    }

    void Update()
    {
        // Calculate deltaTime
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Calculate FPS
        float fps = 1.0f / deltaTime;

        // Update FPS Text
        fpsText.text = string.Format("FPS: {0:0.}", fps);
    }
}