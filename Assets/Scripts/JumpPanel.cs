using System;
using UnityEngine;
using NRB = NetworkedRigidbody.NetworkedRigidbody;

public class JumpPanel : MonoBehaviour
{
    [SerializeField] Vector3 dir;
    [SerializeField] float forceMultiplier;

    [SerializeField] string antagonistTag = "antagonist", protagonistTag = "protagonist";

    private void OnValidate()
    {
        if (dir.sqrMagnitude != 1)
            dir.Normalize();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Action<Vector3, ForceMode> fnAddForce = null;
        if (collision.transform.CompareTag(protagonistTag))
            fnAddForce = collision.gameObject.GetComponent<NRB>().AddForce;
        else if (collision.transform.CompareTag(antagonistTag))
            fnAddForce = fnAddForce = collision.rigidbody.AddForce;
        if(fnAddForce != null) fnAddForce(dir * forceMultiplier, ForceMode.Impulse);
    }
}
