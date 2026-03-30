using UnityEngine;
using TMPro;

public class Stopwatch : MonoBehaviour
{
    public TMP_Text timerText;       // Drag your UI Text here in Inspector

    public float ElapsedTime => elapsedTime;
    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Start()
    {
        isRunning = true;
        UpdateDisplay(); // shows 00:00.00 on game start
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateDisplay();
        }
    }

    public void StartStop()
    {
        isRunning = !isRunning;
    }

    public void Reset()
    {
        isRunning = false;
        elapsedTime = 0f;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 100) % 100);

        timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
}