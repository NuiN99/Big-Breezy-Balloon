using NuiN.NExtensions;
using System;
using System.Collections;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;
    [SerializeField] private SoftBody softBody;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float blastForce = 3f;

    [SerializeField] private SoftBodyParams deflatedBodyParams;
    [SerializeField] private SoftBodyParams inflatedBodyParams;

    private SoftBodyParams curBodyParams;
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

            if (t > float.Epsilon && Input.GetKey(KeyCode.Space))
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

                } while (t > float.Epsilon && Input.GetKey(KeyCode.Space));

                t = Math.Clamp(t, 0, 1);
                UpdateSoftBody();
            }

            if (Input.GetKey(KeyCode.LeftControl) && t < 1)
            {
                do
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, maxScale, t);
                    yield return new WaitForFixedUpdate();
                    t += Time.fixedDeltaTime;

                } while (Input.GetKey(KeyCode.LeftControl) && t < 1);

                t = Math.Clamp(t, 0, 1);
                UpdateSoftBody();
            }

            if (Input.GetKey(KeyCode.LeftAlt) && t > float.Epsilon)
            {
                do
                {
                    transform.localScale = Vector3.Lerp(minScale, transform.localScale, t);
                    yield return new WaitForFixedUpdate();
                    t -= Time.fixedDeltaTime;

                } while (Input.GetKey(KeyCode.LeftAlt) && t > float.Epsilon);
                
                t = Math.Clamp(t, 0, 1);
                UpdateSoftBody();
            }

            yield return null;
        }
    }

    private void UpdateSoftBody()
    {
        //if (t >= 1f - float.Epsilon)
        //{
        //    softBody.SetRigid();
        //    return;
        //}
        //else
        //    softBody.SetSoft();

        curBodyParams.Lerp(deflatedBodyParams, inflatedBodyParams, t);
        softBody.UpdateSprings(curBodyParams);
    }
}
