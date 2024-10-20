using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<BalloonMovement> OnPlayerDied = delegate { };
    public static void InvokePlayerDied(BalloonMovement balloon) => OnPlayerDied.Invoke(balloon);
}
