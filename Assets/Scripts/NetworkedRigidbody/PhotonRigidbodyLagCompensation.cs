using UnityEngine;
using Photon.Pun;

namespace NetworkedRigidbody
{
    public class PhotonRigidbodyLagCompensation : MonoBehaviour, IPunObservable
    {
        [SerializeField] Rigidbody rb;
        [SerializeField] bool bCompensate;
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(rb.position);
                stream.SendNext(rb.rotation);
                stream.SendNext(rb.velocity);
            }
            else
            {
                rb.position = (Vector3)stream.ReceiveNext();
                rb.rotation = (Quaternion)stream.ReceiveNext();
                rb.velocity = (Vector3)stream.ReceiveNext();

                if (!bCompensate) return;
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                rb.position += rb.velocity * lag;
            }
        }
    }
}