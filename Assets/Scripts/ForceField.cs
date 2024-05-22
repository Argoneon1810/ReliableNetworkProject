using System;
using UnityEngine;
using NRB = NetworkedRigidbody.NetworkedRigidbody;

public class ForceField : MonoBehaviour
{
    [SerializeField] Vector3 dir;
    [SerializeField] float forceMultiplier;

    [SerializeField] string antagonistTag = "antagonist", protagonistTag = "protagonist";

    Action<Vector3, ForceMode> fnAddForceProta = null, fnAddForceAnta = null;

    private void OnValidate()
    {
        if (dir.sqrMagnitude != 1)
            dir.Normalize();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(protagonistTag))
            fnAddForceProta = other.gameObject.GetComponent<NRB>().AddForce;
        else if (other.transform.CompareTag(antagonistTag))
            fnAddForceAnta = other.attachedRigidbody.AddForce;
    }
    private void OnTriggerStay(Collider other)
    {
        if (fnAddForceProta != null) fnAddForceProta(dir * forceMultiplier, ForceMode.Force);
        if (fnAddForceAnta != null) fnAddForceAnta(dir * forceMultiplier, ForceMode.Force);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag(protagonistTag))
            fnAddForceProta = null;
        else if (other.transform.CompareTag(antagonistTag))
            fnAddForceAnta = null;
    }
}
