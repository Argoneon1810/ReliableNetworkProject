using System;
using UnityEngine;


namespace LocalTest
{
    public class RigidbodyReceiver : MonoBehaviour
    {
        [SerializeField] Rigidbody rb;
        [SerializeField] bool isDebug;

        private void Awake()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
        }

        internal void Sync(Rigidbody rb)
        {
            this.rb.velocity = rb.velocity;
            this.rb.angularVelocity = rb.angularVelocity;
            if (isDebug)
            {
                Vector3 tp = transform.position;
                tp.y = rb.position.y;
                transform.position = tp;
            }
            else transform.position = rb.position;
            transform.rotation = rb.rotation;
        }

        bool Check(Rigidbody rb) => this.rb.velocity == rb.velocity && this.rb.angularVelocity == rb.angularVelocity
                                    && transform.position == rb.transform.position && transform.rotation == rb.transform.rotation;
        internal void SyncIfDesync(Rigidbody rb)
        {
            if (!Check(rb))
                Sync(rb);
        }

        internal void SyncForce(Vector3 force, ForceMode fm) => rb.AddForce(force, fm);
        internal void SyncTorque(Vector3 torque, ForceMode fm) => rb.AddTorque(torque, fm);
    }
}