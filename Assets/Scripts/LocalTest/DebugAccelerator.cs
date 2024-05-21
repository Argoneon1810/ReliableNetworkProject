using UnityEngine;

namespace RemoteTest
{
    public class DebugAccelerator : MonoBehaviour
    {
        [SerializeField] NetworkedRigidbody rb;
        [SerializeField] float multiplier = 1;

        void Start()
        {
            if (!rb) rb = GetComponent<NetworkedRigidbody>();
        }

        void Update()
        {
            rb.AddForce(rb.velocity * multiplier);
        }
    }
}