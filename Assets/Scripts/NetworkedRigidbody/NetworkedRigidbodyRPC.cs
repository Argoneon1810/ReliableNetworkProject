using Photon.Pun;
using UnityEngine;

namespace NetworkedRigidbody
{
    public class NetworkedRigidbodyRPC : NetworkedRigidbody
    {
        protected override void DoOnEveryTick()
        {
            base.DoOnEveryTick();
            if (!pv.IsMine) return;
            DirectionChangeDetector();
        }

        void DirectionChangeDetector()
        {
            float deg = Vector3.Angle(lastHeading, rb.velocity);
            if (deg > angleThreshold)
            {
                lastHeading = rb.velocity;
                pv.RPC(nameof(Sync), RpcTarget.AllBuffered, rb.velocity, rb.angularVelocity, transform.position, transform.rotation.eulerAngles);
                OnNetworkCall?.Invoke();
            }
        }
    }
}