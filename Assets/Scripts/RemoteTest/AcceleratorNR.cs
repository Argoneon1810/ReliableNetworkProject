using UnityEngine;

namespace RemoteTest
{
    public class AcceleratorNR : Accelerator<NetworkedRigidbody>, IRigidbodyAdapter
    {
        protected override void SetAdapter()
        {
            rbAdapter = this;
        }

        void IRigidbodyAdapter.AddForce(Vector3 velocity)
            => rb.AddForce(velocity);

        Vector3 IRigidbodyAdapter.GetVelocity()
            => rb.Velocity;
    }
}