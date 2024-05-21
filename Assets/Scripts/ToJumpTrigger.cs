using UnityEngine;
using RemoteTest;

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
        else
        {
            NetworkedRigidbody nrb = other.gameObject.GetComponent<NetworkedRigidbody>();
            nrb.Stop();
            nrb.Teleport(protagonistDestination.position, protagonistDestination.rotation);
        }
    }
}
