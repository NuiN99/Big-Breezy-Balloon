using NuiN.NExtensions;
using System;
using System.Collections;
using UnityEngine;

public class SoftBalloon : MonoBehaviour
{
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Cloth cloth;
    [SerializeField] private float blastForce = 3f;

    private float t = 0;

    private void Start()
    {
        t = 0;
        transform.localScale = minScale;
        StartCoroutine(InputHandler());
    }

    private IEnumerator InputHandler()
    {

        for (; ; )
        {
            Vector3 dir = Vector3.up;
            dir.x = Input.GetAxis("Horizontal");
            dir.z = Input.GetAxis("Vertical");

            Debug.DrawRay(transform.position, dir, Color.magenta);

            if (t > float.Epsilon && Input.GetAxis("Jump") > float.Epsilon)
            {
                do
                {
                    dir = Vector3.up;
                    dir.x = Input.GetAxis("Horizontal");
                    dir.z = Input.GetAxis("Vertical");

                    Debug.DrawRay(transform.position, dir, Color.magenta);

                    rb.AddForce(dir * blastForce, ForceMode.Force);

                    transform.localScale = Vector3.Lerp(minScale, transform.localScale, t);
                    yield return new WaitForFixedUpdate();
                    t -= Time.fixedDeltaTime;

                } while (t > float.Epsilon && Input.GetAxis("Jump") > float.Epsilon);

                t = Math.Clamp(t, 0, 1);
            }

            if (Input.GetAxis("Fire1") > float.Epsilon && t < 1)
            {
                do
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, maxScale, t);
                    yield return new WaitForFixedUpdate();
                    t += Time.fixedDeltaTime;

                } while (Input.GetAxis("Fire1") > float.Epsilon && t < 1);

                t = Math.Clamp(t, 0, 1);
            }

            if (Input.GetAxis("Fire2") > float.Epsilon && t > float.Epsilon)
            {
                do
                {
                    transform.localScale = Vector3.Lerp(minScale, transform.localScale, t);
                    yield return new WaitForFixedUpdate();
                    t -= Time.fixedDeltaTime;

                } while (Input.GetAxis("Fire2") > float.Epsilon && t > float.Epsilon);
                
                t = Math.Clamp(t, 0, 1);
            }

            yield return null;
        }
    }
}
