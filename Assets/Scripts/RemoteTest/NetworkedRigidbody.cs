using Photon.Pun;
using UnityEngine;

namespace RemoteTest
{
    public class NetworkedRigidbody : MonoBehaviour
    {
        [SerializeField] protected PhotonView pv;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected HandShakeTimeTracer hstt;

        [SerializeField] protected bool doDesyncProof = false;
        [SerializeField] protected float desyncCheckEvery = 3;
        protected float accumulatedDeltaTime = 0;

        [SerializeField] protected float angleThreshold = 1f;
        [SerializeField] protected Vector3 lastHeading;

        public Vector3 velocity => rb.velocity;
        public Vector3 angularVelocity => rb.angularVelocity;
        public Vector3 position => rb.position;
        public Quaternion rotation => rb.rotation;

        void Awake()
        {
            if (!pv) pv = GetComponent<PhotonView>();
            if (!rb) rb = GetComponent<Rigidbody>();
            if (!hstt) hstt = HandShakeTimeTracer.Instance;
            lastHeading = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        private void Update()
        {
            DoOnUpdate();
        }

        void OnCollisionEnter(Collision collision)
        {
            DoOnCollisionEnter(collision);
        }

        protected virtual void DoOnUpdate()
        {
            if (!pv.IsMine) return;
            if (doDesyncProof) DesyncProof();
        }

        protected virtual void DesyncProof()
        {
            if (accumulatedDeltaTime > desyncCheckEvery)
            {
                accumulatedDeltaTime -= desyncCheckEvery;
                pv.RPC(nameof(Sync), RpcTarget.AllBuffered, rb.velocity, rb.angularVelocity, transform.position, transform.rotation.eulerAngles);
            }
            accumulatedDeltaTime += Time.deltaTime;
        }

        protected virtual void CompensatedUpdate(Vector3 v, Vector3 av, Vector3 p, Vector3 r, float deltaTime)
        {
            rb.velocity = v;
            rb.angularVelocity = av;
            rb.position = p + rb.velocity * deltaTime;
            rb.rotation = Quaternion.Euler(r + (rb.angularVelocity * deltaTime));
        }

        protected virtual void DoOnCollisionEnter(Collision collision)
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(Sync), RpcTarget.AllBuffered, rb.velocity, rb.angularVelocity, transform.position, transform.rotation.eulerAngles);
        }

        public virtual void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(SyncForce), RpcTarget.AllBuffered, force, forceMode);
        }

        public virtual void AddTorque(Vector3 torque, ForceMode forceMode = ForceMode.Force)
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(SyncTorque), RpcTarget.AllBuffered, torque, forceMode);
        }

        public virtual void Teleport(Vector3 position, Quaternion rotation)
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(SyncTransform), RpcTarget.AllBuffered, position, rotation.eulerAngles);
        }

        [PunRPC]
        protected virtual void Sync(Vector3 velocity, Vector3 angularVelocity, Vector3 position, Vector3 eulerRotation)
        {
            if (pv.IsMine) return;
            CompensatedUpdate(
                velocity,
                angularVelocity,
                position,
                eulerRotation,
                hstt.HandShakeTime / 2
            );
        }

        [PunRPC]
        protected virtual void SyncForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
            => rb.AddForce(force, forceMode);

        [PunRPC]
        protected virtual void SyncTorque(Vector3 torque, ForceMode forceMode = ForceMode.Force)
            => rb.AddTorque(torque, forceMode);

        [PunRPC]
        protected virtual void SyncTransform(Vector3 position, Vector3 eulerRotation)
        {
            if (pv.IsMine) return;
            transform.position = position;
            transform.rotation = Quaternion.Euler(eulerRotation);
        }
    }
}