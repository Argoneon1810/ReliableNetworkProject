using RemoteTest;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] string antagonistTag = "antagonist", protagonistTag = "protagonist";
    [SerializeField] GameObject goalWall;

    bool antaPass, protaPass;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.CompareTag(protagonistTag))
        {
            protaPass = true;
            collider.gameObject.GetComponent<AcceleratorNR>().enabled = false;
            collider.gameObject.GetComponent<NetworkedRigidbody>().Stop();
        }
        else if (collider.transform.CompareTag(antagonistTag))
        {
            antaPass = true;
            collider.gameObject.GetComponent<AcceleratorPR>().enabled = false;
            Rigidbody rb = collider.attachedRigidbody;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    private void Update()
    {
        if (antaPass && protaPass)
        {
            goalWall.SetActive(true);
            Destroy(this);
        }
    }
}
