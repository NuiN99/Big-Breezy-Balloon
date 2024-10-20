using NuiN.NExtensions;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] Checkpoint[] checkpoints;

    Checkpoint _activeCheckpoint;

    void Awake()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            Checkpoint checkpoint = checkpoints[i];
            checkpoint.Init(this, i);
        }
    }

    void OnEnable()
    {
        GameEvents.OnPlayerDied += RespawnPlayer;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerDied -= RespawnPlayer;
    }

    void RespawnPlayer(BalloonMovement player)
    {
        this.DoAfter(1f, () =>
        {
            player.EnableVisuals();
            player.ResetPhysics();
            player.RB.position = _activeCheckpoint.RespawnPoint;
            player.RB.rotation = _activeCheckpoint.RespawnRotation;
        });
    }

    public void TrySetActiveCheckpoint(int index)
    {
        // only progress checkpoints forward
        if (_activeCheckpoint == null || index > _activeCheckpoint.Index)
        {
            Debug.Log($"Set Checkpoint: {index}");
            _activeCheckpoint = checkpoints[index];
        }
    }
}