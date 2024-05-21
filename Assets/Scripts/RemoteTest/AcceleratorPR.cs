using UnityEngine;

namespace RemoteTest
{
    public class AcceleratorPR : Accelerator<Rigidbody>, IRigidbodyAdapter
    {
        protected override void SetAdapter()
        {
            rbAdapter = this;
        }

        void IRigidbodyAdapter.AddForce(Vector3 velocity)
            => rb.AddForce(velocity);

        Vector3 IRigidbodyAdapter.GetVelocity()
            => rb.velocity;
    }
}