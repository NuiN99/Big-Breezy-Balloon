using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int Index { get; private set; }
    public Vector3 RespawnPoint => respawnPosition.position;
    public Quaternion RespawnRotation => respawnPosition.rotation;
    
    [SerializeField] Transform respawnPosition;
    
    CheckpointManager _manager;
    
    public void Init(CheckpointManager manager, int index)
    {
        _manager = manager;
        Index = index;
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Balloon"))
        {
            _manager.TrySetActiveCheckpoint(Index);
        }
    }
}