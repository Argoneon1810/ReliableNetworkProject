using Photon.Pun;
using UnityEngine;

namespace RemoteTest
{
    public class AcceleratorPR : Accelerator<Rigidbody>, IRigidbodyAdapter
    {
        [SerializeField] PhotonRigidbodyView prv;
        protected override void SetAdapter()
        {
            rbAdapter = this;
        }

        void IRigidbodyAdapter.AddForce(Vector3 velocity)
        {
            rb.AddForce(velocity);
            prv.OnNetworkCall?.Invoke();
        }

        Vector3 IRigidbodyAdapter.GetVelocity()
            => rb.velocity;

        public override void Stop()
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(StopAcceleration), RpcTarget.AllBufferedViaServer);
            prv.OnNetworkCall?.Invoke();
        }

        [PunRPC]
        protected override void StopAcceleration()
        {
            enabled = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}