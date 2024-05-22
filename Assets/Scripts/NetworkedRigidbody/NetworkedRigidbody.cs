using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace NetworkedRigidbody
{
    public class NetworkedRigidbody : MonoBehaviour, IStateMachine, IPunInstantiateMagicCallback, IInvokeToSync
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

        #region Initialization State
        void Awake() => Init();
        public void Init()
        {
            if (!pv) pv = GetComponent<PhotonView>();
            if (!rb) rb = GetComponent<Rigidbody>();
            if (!hstt) hstt = HandShakeTimeTracer.Instance;
            lastHeading = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
            => info.Sender.TagObject = gameObject;
        public void InvokeProperties()
        {
            Velocity = Velocity;
            AngularVelocity = AngularVelocity;
            Position = Position;
            Rotation = Rotation;
        }
        #endregion

        #region Runtime State
        void Update() => Tick();
        public void Tick() => DoOnEveryTick();
        protected virtual void DoOnEveryTick()
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
        #endregion

        #region Interruption No Param
        public virtual void Stop() =>
            TryInterrupt(new Action(delegate () {
                Velocity = Vector3.zero;
                AngularVelocity = Vector3.zero;
            }));
        public void TryInterrupt(Action doOnInterruptable)
        {
            if (!pv.IsMine) return;
            doOnInterruptable();
            OnNetworkCall?.Invoke();
        }
        #endregion

        #region Interruption Single Param
        void OnCollisionEnter(Collision collision)
            => DoOnCollisionEnter(collision);
        public virtual void DoOnCollisionEnter(Collision collision) =>
            TryInterrupt(new Action<Collision>(delegate(Collision collision) {
                pv.RPC(nameof(Sync), RpcTarget.AllBuffered, rb.velocity, rb.angularVelocity, transform.position, transform.rotation.eulerAngles);
            }), collision);
        public virtual void UseGravity(bool targetState) =>
            TryInterrupt(new Action<bool>(delegate (bool targetState) {
                pv.RPC(nameof(SyncGravity), RpcTarget.AllBufferedViaServer, targetState);
            }), targetState);
        public void TryInterrupt<T>(Action<T> doOnInterruptable, T param)
        {
            if (!pv.IsMine) return;
            doOnInterruptable(param);
            OnNetworkCall?.Invoke();
        }
        #endregion

        #region Interruption Dual Param
        public virtual void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force) =>
            TryInterrupt(new Action<Vector3, ForceMode>(delegate (Vector3 force, ForceMode forceMode) {
                pv.RPC(nameof(SyncForce), RpcTarget.AllBuffered, force, forceMode);
            }), force, forceMode);
        public virtual void AddTorque(Vector3 torque, ForceMode forceMode = ForceMode.Force) =>
            TryInterrupt(new Action<Vector3, ForceMode>(delegate (Vector3 torque, ForceMode forceMode) {
                pv.RPC(nameof(SyncTorque), RpcTarget.AllBuffered, torque, forceMode);
            }), torque, forceMode);
        public virtual void Teleport(Vector3 position, Quaternion rotation) =>
            TryInterrupt(new Action<Vector3, Quaternion>(delegate (Vector3 position, Quaternion rotation) {
                Position = position;
                Rotation = rotation;
            }), position, rotation);
        public void TryInterrupt<T1, T2>(Action<T1, T2> doOnInterruptable, T1 param1, T2 param2)
        {
            if (!pv.IsMine) return;
            doOnInterruptable(param1, param2);
            OnNetworkCall?.Invoke();
        }
        #endregion

        #region Lag Compensation
        protected virtual void CompensatedUpdate(Vector3 v, Vector3 av, Vector3 p, Vector3 r, float deltaTime)
        {
            rb.velocity = v;
            rb.angularVelocity = av;
            rb.position = bCompensate ? p + rb.velocity * deltaTime : p;
            rb.rotation = bCompensate ? Quaternion.Euler(r) : Quaternion.Euler(r + (rb.angularVelocity * deltaTime));
        }
        #endregion

        #region RPC Methods
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
        #endregion

    }
}