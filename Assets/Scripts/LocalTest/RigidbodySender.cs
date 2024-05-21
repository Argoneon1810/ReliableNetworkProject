using UnityEngine;

namespace LocalTest
{
    public class RigidbodySender : MonoBehaviour
    {
        [SerializeField] RigidbodyReceiver receiver;
        [SerializeField] Rigidbody rb;

        [SerializeField] bool doDesyncProof = false;
        [SerializeField] float desyncCheckEvery = 3;
        float accumulatedDeltaTime = 0;

        [SerializeField] float angleThreshold = 1f;
        [SerializeField] Vector3 lastHeading;

        void Awake()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            lastHeading = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        void Update()
        {
            if (doDesyncProof) DesyncProof();
            DirectionChangeDetector();
        }

        void DesyncProof()
        {
            if (accumulatedDeltaTime > desyncCheckEvery)
            {
                accumulatedDeltaTime -= desyncCheckEvery;
                receiver.SyncIfDesync(rb);
            }
            accumulatedDeltaTime += Time.deltaTime;
        }

        void DirectionChangeDetector()
        {
            float deg = Vector3.Angle(lastHeading, rb.velocity);
            if (deg > angleThreshold)
            {
                lastHeading = rb.velocity;
                receiver.Sync(rb);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            receiver.Sync(rb);
        }

        public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            rb.AddForce(force, forceMode);
            receiver.SyncForce(force, forceMode);
        }

        public void AddTorque(Vector3 torque, ForceMode forceMode = ForceMode.Force)
        {
            rb.AddTorque(torque, forceMode);
            receiver.SyncTorque(torque, forceMode);
        }
    }

}