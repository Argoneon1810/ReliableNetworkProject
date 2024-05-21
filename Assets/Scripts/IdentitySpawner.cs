using UnityEngine;
using Photon.Pun;

public class IdentitySpawner : MonoBehaviour
{
    [SerializeField] GameObject ifMaster, ifClient;
    [SerializeField] Transform ifMasterT, ifClientT;
    [SerializeField] string MasterTagName = "protagonist", ClientTagName = "antagonist";
    private void Start()
    {
        ifMasterT = GameObject.FindGameObjectWithTag(MasterTagName).transform;
        ifClientT = GameObject.FindGameObjectWithTag(ClientTagName).transform;
        GameObject instantiated = null;
        if (PhotonNetwork.IsMasterClient)
        {
            DebugLogger.Instance.Log("Summoning Master Client");
            instantiated = PhotonNetwork.Instantiate(ifMaster.name, ifMasterT.position, ifMasterT.rotation);
        }
        else
        {
            DebugLogger.Instance.Log("Summoning Remote Client");
            instantiated = PhotonNetwork.Instantiate(ifClient.name, ifClientT.position, ifClientT.rotation);
        }
        NetworkSpawner spawner = FindObjectOfType<NetworkSpawner>();
        spawner.Enlist(instantiated);
        spawner.RequestDestroy(gameObject);
    }
}
