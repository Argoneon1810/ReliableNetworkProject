using UnityEngine;

namespace RemoteTest
{
    public class AcceleratorPR : MonoBehaviour
    {
        [SerializeField] Rigidbody rb;
        [SerializeField] float multiplier = 1;

        void Start()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            rb.AddForce(rb.velocity * multiplier);
        }
    }
}