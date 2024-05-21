using Photon.Pun;
using System.IO;
using UnityEngine;

namespace RemoteTest
{
    public class RigidbodyTranceiver2 : MonoBehaviour, IPunObservable
    {
        [SerializeField] PhotonView pv;
        [SerializeField] Rigidbody rb;

        [SerializeField] bool doDesyncProof = false;
        [SerializeField] float desyncCheckEvery = 3;
        float accumulatedDeltaTime = 0;

        [SerializeField] float angleThreshold = 1f;
        [SerializeField] float distanceThreshold = 0.1f;
        [SerializeField] Vector3 lastHeading;

        [SerializeField] Vector3 tp, tr;

        void Awake()
        {
            if (!pv) pv = GetComponent<PhotonView>();
            if (!rb) rb = GetComponent<Rigidbody>();
            lastHeading = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        void Update()
        {
            if (pv.IsMine) return;
            if (!doDesyncProof) return;
            if (accumulatedDeltaTime > desyncCheckEvery)
            {
                accumulatedDeltaTime -= desyncCheckEvery;
                pv.RPC("AmIDesynced", RpcTarget.All, rb.position, rb.rotation.eulerAngles, rb.velocity, rb.angularVelocity);
            }
            accumulatedDeltaTime += Time.deltaTime;
        }

        public void FixedUpdate()
        {
            if (pv.IsMine) return;
            rb.position = Vector3.MoveTowards(rb.position, tp, Time.fixedDeltaTime);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.Euler(tr), Time.fixedDeltaTime * 100.0f);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (Vector3.Angle(lastHeading, rb.velocity) > angleThreshold)
                {
                    lastHeading = rb.velocity;
                    stream.SendNext(rb.position);
                    stream.SendNext(rb.rotation);
                    stream.SendNext(rb.velocity);
                    stream.SendNext(rb.angularVelocity);
                }
            }
            else
            {
                Vector3 position_r = (Vector3)stream.ReceiveNext();
                Quaternion rotation_r = (Quaternion)stream.ReceiveNext();
                Vector3 velocity_r = (Vector3)stream.ReceiveNext();
                Vector3 angularVelocity_r = (Vector3)stream.ReceiveNext();
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                position_r += velocity_r * lag;
                rotation_r = Quaternion.Euler(rotation_r.eulerAngles + (angularVelocity_r * lag));
                if(Mathf.Abs(rb.position.sqrMagnitude - position_r.sqrMagnitude) > distanceThreshold*distanceThreshold)
                {
                    rb.position = position_r;
                    rb.rotation = rotation_r;
                    rb.velocity = velocity_r;
                    rb.angularVelocity = angularVelocity_r;
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!pv.IsMine) return;
            pv.RPC("SyncRigidbody", RpcTarget.AllBuffered, rb.velocity, rb.angularVelocity);
            pv.RPC("SyncTransform", RpcTarget.AllBuffered, transform.position, transform.rotation.eulerAngles);
        }

        public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            if (!pv.IsMine) return;
            pv.RPC("SyncForce", RpcTarget.AllBuffered, force, forceMode);
        }

        public void AddTorque(Vector3 torque, ForceMode forceMode = ForceMode.Force)
        {
            if (!pv.IsMine) return;
            pv.RPC("SyncTorque", RpcTarget.AllBuffered, torque, forceMode);
        }

        [PunRPC]
        void SyncRigidbody(Vector3 velocity, Vector3 angularVelocity)
        {
            if (pv.IsMine) return;
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }

        [PunRPC]
        void SyncTransform(Vector3 position, Vector3 eulerRotation)
        {
            if (pv.IsMine) return;
            transform.position = position;
            transform.rotation = Quaternion.Euler(eulerRotation);
        }

        [PunRPC]
        void SyncForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
            => rb.AddForce(force, forceMode);

        [PunRPC]
        void SyncTorque(Vector3 torque, ForceMode forceMode = ForceMode.Force)
            => rb.AddTorque(torque, forceMode);

        [PunRPC]
        void AmIDesynced(Vector3 velocity, Vector3 angularVelocity, Vector3 position, Vector3 eulerRotation)
        {
            if (!pv.IsMine) return;
            bool desynced = false;
            if(rb.velocity != velocity)
                desynced = true;
            if(!desynced && rb.angularVelocity != angularVelocity)
                desynced = true;
            if (!desynced && rb.position != position)
                desynced = true;
            if (!desynced && rb.rotation.eulerAngles != eulerRotation)
                desynced = true;
            if (desynced)
                pv.RPC("YouAreDesynced", RpcTarget.All, rb.velocity, rb.angularVelocity, rb.position, rb.rotation.eulerAngles);
        }

        [PunRPC]
        void YouAreDesynced(Vector3 velocity, Vector3 angularVelocity, Vector3 position, Vector3 eulerRotation)
        {
            if (pv.IsMine) return;
            DebugLogger.Instance.Log(name + " is Desynced so syncing it back");
            tp = position;
            tr = eulerRotation;
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }
}