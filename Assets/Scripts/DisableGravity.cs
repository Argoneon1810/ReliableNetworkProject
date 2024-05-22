using UnityEngine;
using NRB = NetworkedRigidbody;

public class DisableGravity : MonoBehaviour
{
    [SerializeField] string antagonistTag = "antagonist", protagonistTag = "protagonist";

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(protagonistTag))
            other.gameObject.GetComponent<NRB.NetworkedRigidbody>().UseGravity(false);
        else if (other.transform.CompareTag(antagonistTag))
            other.attachedRigidbody.useGravity = false;
    }
}
