using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardPanel : MonoBehaviour
{
    private LeaderboardEntry[] entries;

    private void Awake()
    {
        entries = GetComponentsInChildren<LeaderboardEntry>();
    }

    private async void OnEnable()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        LeaderboardScoresPage res = await LeaderboardsService.Instance.GetScoresAsync("track1");
        if(res != null)
            ShowScores(res);
    }

    private void ShowScores(LeaderboardScoresPage res)
    {
        res.Results.Sort((x, y) => x.Rank.CompareTo(y.Rank));
        for (int i = 0; i < entries.Length && i < res.Results.Count; i++)
        {
            LeaderboardEntry entryUI = entries[i];
            Unity.Services.Leaderboards.Models.LeaderboardEntry entryData = res.Results[i];

            entryUI.SetData(entryData);
        }
    }
}
