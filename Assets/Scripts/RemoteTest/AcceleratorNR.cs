using Photon.Pun;
using UnityEngine;

namespace RemoteTest
{
    public class AcceleratorNR : Accelerator<NetworkedRigidbody>, IRigidbodyAdapter
    {
        protected override void SetAdapter()
        {
            rbAdapter = this;
        }

        public override void Stop()
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(StopAcceleration), RpcTarget.AllBufferedViaServer);
        }

        [PunRPC]
        protected override void StopAcceleration()
        {
            enabled = false;
            rb.Stop();
        }

        void IRigidbodyAdapter.AddForce(Vector3 velocity)
            => rb.AddForce(velocity);

        Vector3 IRigidbodyAdapter.GetVelocity()
            => rb.Velocity;
    }
}