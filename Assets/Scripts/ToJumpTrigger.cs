using UnityEngine;
using NRB = NetworkedRigidbody;

public class ToJumpTrigger : MonoBehaviour
{
    [SerializeField] Transform antagonistDestination, protagonistDestination;
    [SerializeField] string antagonistTag = "antagonist", protagonistTag= "protagonist";
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == antagonistTag)
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = antagonistDestination.position;
            rb.rotation = antagonistDestination.rotation;
        }
        else if(other.tag == protagonistTag)
        {
            NRB.NetworkedRigidbody nrb = other.gameObject.GetComponent<NRB.NetworkedRigidbody>();
            nrb.Stop();
            nrb.Teleport(protagonistDestination.position, protagonistDestination.rotation);
        }
    }
}
