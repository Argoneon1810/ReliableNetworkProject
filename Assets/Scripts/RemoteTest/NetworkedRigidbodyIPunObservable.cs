using Photon.Pun;
using UnityEngine;

namespace RemoteTest
{
    public class NetworkedRigidbodyIPunObservable : NetworkedRigidbody, IPunObservable
    {
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (Vector3.Angle(lastHeading, rb.velocity) > angleThreshold)
                {
                    lastHeading = rb.velocity;
                    stream.SendNext(rb.velocity);
                    stream.SendNext(rb.angularVelocity);
                    stream.SendNext(rb.position);
                    stream.SendNext(rb.rotation.eulerAngles);
                }
            }
            else
            {
                CompensatedUpdate(
                    (Vector3)stream.ReceiveNext(),
                    (Vector3)stream.ReceiveNext(),
                    (Vector3)stream.ReceiveNext(),
                    (Vector3)stream.ReceiveNext(),
                    Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime))
                );
            }
        }
    }
}