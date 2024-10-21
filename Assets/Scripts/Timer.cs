using JetBrains.Annotations;
using NuiN.NExtensions;
using NuiN.SpleenTween;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    [SerializeField, InjectComponent] private TMP_Text textTimer;
    [SerializeField] private float gellyInterval = 10f;
    [SerializeField] private Vector3 gellyScale = Vector3.one;

    int prevMinutes = 0;

    private void Update()
    {
        int minutes = Convert.ToInt32(Math.Floor(Time.timeSinceLevelLoad / gellyInterval));
        if (minutes > prevMinutes)
        {
            SpleenTween.AddScale(transform, gellyScale, 0.25f).SetEase(Ease.InOutQuad).OnComplete(() => SpleenTween.AddScale(transform, -gellyScale, 0.25f).SetEase(Ease.InOutQuad));
            prevMinutes = minutes;
        }

        textTimer.text = FormatTimestamp(Time.timeSinceLevelLoad);
    }

    public void CompleteLevel()
    {
        OnLevelComplete();
    }

    public static async void OnLevelComplete()
    {
        float score = Time.timeSinceLevelLoad;
        await LeaderboardsService.Instance.AddPlayerScoreAsync("track1", score);
        SceneManager.LoadScene("SplashScreen");
    }

    public static string FormatTimestamp(double timestamp)
    {
        int minutes = Convert.ToInt32(Math.Floor(timestamp / 60.0));
        //string minStr = minutes <= 0 ? "00" : $"{minutes:00}";
        
        int seconds = Convert.ToInt32(Math.Floor(timestamp % 60.0));
        //string secStr = seconds <= 0 ? "00" : $"{seconds:00}";

        double ms = timestamp - Math.Truncate(timestamp);
        //string msStr = ms <= 0.00 ? "00" : $"{ms:.00}";

        return $"{minutes:00}:{seconds:00}{ms:.00}";
    }
}
