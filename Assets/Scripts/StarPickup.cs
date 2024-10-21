using System;
using NuiN.NExtensions;
using UnityEngine;

public class StarPickup : MonoBehaviour
{
    private SpinningAnimation spin;
    public float y_rise = 20.0f;
    public float y_rise_speed = 0.1f;
    private bool pickedUp = false;

    [SerializeField] Timer riseTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(y_rise < 0) y_rise = 0f;
        spin = GetComponent<SpinningAnimation>();
    }

    void Update()
    {
        if(pickedUp) pickupStarAnimation();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
        }
    }

    public void Collect()
    {
        if (pickedUp) return;
        riseTimer.Restart();
        spin.rotationDirection = new Vector3(0f,5f,0f);
        pickedUp = true;
    }

    void pickupStarAnimation()
    {
        spin.speed = 2f;
        if(!riseTimer.IsComplete)
        {
            transform.position += new Vector3(0f,y_rise_speed,0f);
        } else {
            Destroy(this.gameObject);
        }
    }
}
