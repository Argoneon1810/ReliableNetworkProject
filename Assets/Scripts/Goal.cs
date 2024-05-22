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
            if(collider.gameObject.TryGetComponent(out AcceleratorNR anr))
                anr.Stop();
        }
        else if (collider.transform.CompareTag(antagonistTag))
        {
            antaPass = true;
            if(collider.gameObject.TryGetComponent(out AcceleratorPR apr))
                apr.Stop();
        }
    }
    private void Update()
    {
        if (antaPass && protaPass)
        {
            goalWall.SetActive(true);
            TestGameManager.Instance.ReportGameFinished();
            Destroy(this);
        }
    }
}
