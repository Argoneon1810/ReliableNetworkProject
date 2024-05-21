using System;
using Photon.Pun;
using UnityEngine;

namespace RemoteTest
{
    public class RigidbodyTranceiver : MonoBehaviour
    {
        [SerializeField] PhotonView pv;
        [SerializeField] Rigidbody rb;

        [SerializeField] bool doDesyncProof = false;
        [SerializeField] float desyncCheckEvery = 3;
        float accumulatedDeltaTime = 0;

        [SerializeField] float angleThreshold = 1f;
        [SerializeField] Vector3 lastHeading;

        void Awake()
        {
            if (!pv) pv = GetComponent<PhotonView>();
            if (!rb) rb = GetComponent<Rigidbody>();
            lastHeading = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        void Update()
        {
            if (!pv.IsMine) return;
            if (doDesyncProof) DesyncProof();
            DirectionChangeDetector();
        }

        void DesyncProof()
        {
            if (accumulatedDeltaTime > desyncCheckEvery)
            {
                accumulatedDeltaTime -= desyncCheckEvery;
                pv.RPC("SyncRigidbody", RpcTarget.AllBuffered, rb.velocity, rb.angularVelocity);
                pv.RPC("SyncTransform", RpcTarget.AllBuffered, transform.position, transform.rotation.eulerAngles);
            }
            accumulatedDeltaTime += Time.deltaTime;
        }

        void DirectionChangeDetector()
        {
            float deg = Vector3.Angle(lastHeading, rb.velocity);
            if (deg > angleThreshold)
            {
                lastHeading = rb.velocity;
                pv.RPC("SyncRigidbody", RpcTarget.AllBuffered, rb.velocity, rb.angularVelocity);
                pv.RPC("SyncTransform", RpcTarget.AllBuffered, transform.position, transform.rotation.eulerAngles);
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
    }
}