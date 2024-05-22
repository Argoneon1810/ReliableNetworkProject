using System;
using UnityEngine;

using Photon.Pun;

namespace NetworkedRigidbody
{
    public class Accelerator<T> : MonoBehaviour where T : Component
    {
        [SerializeField] protected T rb;
        [SerializeField] protected float multiplier = 1;
        [SerializeField] protected PhotonView pv;
        protected IRigidbodyAdapter rbAdapter;

        void Start()
        {
            if (!rb) rb = GetComponent<T>();
            if (!pv) pv = GetComponent<PhotonView>();
            SetAdapter();
        }

        void Update()
        {
            rbAdapter.AddForce(rbAdapter.GetVelocity() * multiplier);
        }

        protected virtual void SetAdapter()
        {
            throw new NotImplementedException();
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }

        [PunRPC]
        protected virtual void StopAcceleration()
        {
            throw new NotImplementedException();
        }
    }
}