using UnityEngine;

namespace RemoteTest
{
    public interface IRigidbodyAdapter
    {
        internal void AddForce(Vector3 velocity);
        internal Vector3 GetVelocity();
    }
}