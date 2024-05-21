using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace RemoteTest
{
    public class TestGameManager : MonoBehaviour
    {
        NetworkManager nm;
        [SerializeField] Button BtnSimulate;
        [SerializeField] PhotonView pv;
        bool bCanSimulate = false;
        bool bClicked = false;

        private void Start()
        {
            nm = NetworkManager.Instance;
            nm.OnJoinedRoomEvents += StopTime;
            if (!pv) pv = GetComponent<PhotonView>();
        }

        private void Update()
        {
            if(bClicked.AsTrigger())
            {
                DebugLogger.Instance.Log("Dispatched");
                Time.timeScale = 0.01f;
                pv.RPC(nameof(StartSimulation), RpcTarget.AllBufferedViaServer);
            }
            if (!(nm.MyState == NetworkManager.CurrentNetworkState.InRoom)) return;
            if (!bCanSimulate) return;
            BtnSimulate.interactable = true;
        }

        public void OnClick()
        {
            DebugLogger.Instance.Log("Clicked");
            bClicked = true;
        }

        void StopTime()
        {
            pv.RPC(nameof(StopSimulation), RpcTarget.AllBufferedViaServer);
        }

        [PunRPC]
        private void StartSimulation()
        {
            DebugLogger.Instance.Log("Starting Simulation");
            BtnSimulate.gameObject.SetActive(false);
            Time.timeScale = 1;
        }

        [PunRPC]
        private void StopSimulation()
        {
            Time.timeScale = 0;
            bCanSimulate = true;
        }
    }
}