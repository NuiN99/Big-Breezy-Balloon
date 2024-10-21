using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningAnimation : MonoBehaviour
{
    public Vector3 rotationDirection = new Vector3(0f,0f,0f);
    public GameObject thingToSpin;

    public float speed = 1f;
    // Start is called before the first frame update

    void FixedUpdate()
    {
        if(thingToSpin == null) return;
        thingToSpin.transform.Rotate(rotationDirection * speed);
    }
}
