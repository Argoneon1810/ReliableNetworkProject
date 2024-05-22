using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

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

        [SerializeField] protected bool bCompensate;
        public UnityAction OnNetworkCall;

        public Vector3 Velocity { get => rb.velocity; private set => pv.RPC(nameof(SetVelocity), RpcTarget.AllBufferedViaServer, value); }
        [PunRPC] protected void SetVelocity(Vector3 velocity) => rb.velocity = velocity;
        public Vector3 AngularVelocity { get => rb.angularVelocity; private set => pv.RPC(nameof(SetAngularVelocity), RpcTarget.AllBufferedViaServer, value); }
        [PunRPC] protected void SetAngularVelocity(Vector3 angularVelocity) => rb.angularVelocity = angularVelocity;
        public Vector3 Position { get => rb.position; private set => pv.RPC(nameof(SetPosition), RpcTarget.AllBufferedViaServer, value); }
        [PunRPC] protected void SetPosition(Vector3 position) => rb.position = position;
        public Quaternion Rotation { get => rb.rotation; private set => pv.RPC(nameof(SetRotation), RpcTarget.AllBufferedViaServer, value); }
        [PunRPC] protected void SetRotation(Quaternion rotation) => rb.rotation = rotation;

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
                OnNetworkCall?.Invoke();
                accumulatedDeltaTime -= desyncCheckEvery;
                pv.RPC(nameof(Sync), RpcTarget.AllBuffered, rb.velocity, rb.angularVelocity, transform.position, transform.rotation.eulerAngles);
            }
            accumulatedDeltaTime += Time.deltaTime;
        }

        protected virtual void CompensatedUpdate(Vector3 v, Vector3 av, Vector3 p, Vector3 r, float deltaTime)
        {
            rb.velocity = v;
            rb.angularVelocity = av;
            rb.position = bCompensate ? p + rb.velocity * deltaTime : p;
            rb.rotation = bCompensate ? Quaternion.Euler(r) : Quaternion.Euler(r + (rb.angularVelocity * deltaTime));
        }

        protected virtual void DoOnCollisionEnter(Collision collision)
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(Sync), RpcTarget.AllBuffered, rb.velocity, rb.angularVelocity, transform.position, transform.rotation.eulerAngles);
            OnNetworkCall?.Invoke();
        }

        public virtual void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(SyncForce), RpcTarget.AllBuffered, force, forceMode);
            OnNetworkCall?.Invoke();
        }

        public virtual void AddTorque(Vector3 torque, ForceMode forceMode = ForceMode.Force)
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(SyncTorque), RpcTarget.AllBuffered, torque, forceMode);
            OnNetworkCall?.Invoke();
        }

        public virtual void Teleport(Vector3 position, Quaternion rotation)
        {
            if (!pv.IsMine) return;
            Position = position;
            Rotation = rotation;
            OnNetworkCall?.Invoke();
        }

        public virtual void Stop()
        {
            if (!pv.IsMine) return;
            Velocity = Vector3.zero;
            AngularVelocity = Vector3.zero;
            OnNetworkCall?.Invoke();
        }

        public virtual void UseGravity(bool targetState)
        {
            if (!pv.IsMine) return;
            pv.RPC(nameof(SyncGravity), RpcTarget.AllBufferedViaServer, targetState);
            OnNetworkCall?.Invoke();
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
        protected virtual void SyncGravity(bool targetState)
            => rb.useGravity = targetState;
    }
}