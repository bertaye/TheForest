using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPainter : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private float cumulativeDeltaTime = 0.0f;
    Rect fpsRect;
    GUIStyle fpsStyle;
    float averagePeriod = 0.5f; //ms
    int frameCount = 0;
    private void Start()
    {
        int width = Screen.width, height = Screen.height;
        fpsStyle = new GUIStyle();

        fpsRect = new Rect(0, 0, width, height * 2 / 100);
        fpsStyle.alignment = TextAnchor.UpperLeft;
        fpsStyle.fontSize = height * 2 / 100;
        fpsStyle.normal.textColor = Color.white;
    }
    void Update()
    {
        cumulativeDeltaTime += Time.unscaledDeltaTime;
        frameCount++;

        if (cumulativeDeltaTime >= averagePeriod)
        {
            deltaTime = cumulativeDeltaTime / frameCount;
            cumulativeDeltaTime = 0.0f;
            frameCount = 0;
        }
    }

    void OnGUI()
    {
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", deltaTime * 1000.0f, 1.0f / deltaTime);

        GUI.Label(fpsRect, text, fpsStyle);
    }
}
