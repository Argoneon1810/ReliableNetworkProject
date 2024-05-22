using UnityEngine;

namespace RemoteTest
{
    public class AcceleratorPR : Accelerator<Rigidbody>, IRigidbodyAdapter
    {
        [SerializeField] Vector3 dir;
        [SerializeField] float forceMultiplier;

        [SerializeField] string antagonistTag = "antagonist", protagonistTag = "protagonist";
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