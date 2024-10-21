using NuiN.NExtensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] TMP_Text rank;
    [SerializeField] Image portrait;
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text time;

    internal void SetData(Unity.Services.Leaderboards.Models.LeaderboardEntry entryData)
    {
        Debug.Log($"Set name {entryData.PlayerName}, set score: {entryData.Score}");
        playerName.text = entryData.PlayerName;
        time.text = Timer.FormatTimestamp(entryData.Score);
    }

    


#if UNITY_EDITOR
    private void Reset()
    {
        TMP_Text[] tmpTexts = GetComponentsInChildren<TMP_Text>();

        if (tmpTexts.Length > 0)
        {
            rank = tmpTexts[0];
            int _rank = transform.GetSiblingIndex() + 1;
            rank.text = _rank.ToString();

            Image bg = GetComponent<Image>();
            if (_rank % 2 == 0)
                bg.color = new Color(0.7f, 0.7f, 0.7f);
            else
                bg.color = new Color(0.6f, 0.6f, 0.6f);
        }

        if (tmpTexts.Length > 1)
            playerName = tmpTexts[1];

        if (tmpTexts.Length > 2)
            time = tmpTexts[2];

        portrait = transform.Find("Portrait").GetComponent<Image>();
    }
#endif
}
