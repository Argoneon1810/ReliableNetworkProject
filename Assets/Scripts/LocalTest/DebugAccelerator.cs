using UnityEngine;

namespace LocalTest
{
    public class DebugAccelerator : MonoBehaviour
    {
        [SerializeField] RigidbodySender rbs;
        [SerializeField] Vector3 force;

        void Start()
        {
            if (!rbs) rbs = GetComponent<RigidbodySender>();
        }

        void Update()
        {
            rbs.AddForce(force);
        }
    }
}