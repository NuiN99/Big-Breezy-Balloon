using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnPlayerDied = delegate { };
    public static void InvokePlayerDied() => OnPlayerDied.Invoke();
}
