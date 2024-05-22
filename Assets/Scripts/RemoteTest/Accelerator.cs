using System;
using UnityEngine;

namespace RemoteTest
{
    public class Accelerator<T> : MonoBehaviour where T : Component
    {
        [SerializeField] protected T rb;
        [SerializeField] protected float multiplier = 1;
        protected IRigidbodyAdapter rbAdapter;

        void Start()
        {
            if (rb == null) rb = GetComponent<T>();
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
    }
}