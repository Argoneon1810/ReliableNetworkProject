using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace RemoteTest
{
    public class IdentitySpawner : MonoBehaviour
    {
        [SerializeField] List<GameObject> ifMaster, ifClient;
        [SerializeField] Transform ifMasterT, ifClientT;
        private void Start()
        {
            ifMasterT = GameObject.FindGameObjectWithTag("protagonist").transform;
            ifClientT = GameObject.FindGameObjectWithTag("antagonist").transform;
            List<GameObject> instantiated = new List<GameObject>();
            if (PhotonNetwork.IsMasterClient)
            {
                DebugLogger.Instance.Log("Summoning Master Client");
                int i = 0;
                foreach(GameObject go in ifMaster)
                    instantiated.Add(PhotonNetwork.Instantiate(go.name, ifMasterT.position + new Vector3(0,i+=2,0), ifMasterT.rotation));
            }
            else
            {
                DebugLogger.Instance.Log("Summoning Remote Client");
                foreach (GameObject go in ifClient)
                    instantiated.Add(PhotonNetwork.Instantiate(go.name, ifClientT.position, ifClientT.rotation));
            }
            NetworkSpawner spawner = FindObjectOfType<NetworkSpawner>();
            foreach(GameObject go in instantiated)
                spawner.Enlist(go);
            spawner.RequestDestroy(gameObject);
        }
    }
}