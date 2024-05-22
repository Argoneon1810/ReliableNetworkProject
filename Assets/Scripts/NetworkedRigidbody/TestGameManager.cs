using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
using System.Text;
using System;

namespace NetworkedRigidbody
{
    public class TestGameManager : MonoBehaviour
    {
        private static TestGameManager instance;
        public static TestGameManager Instance => instance;
        [SerializeField] NetworkSpawner spawner;
        [SerializeField] Button BtnSimulate;
        [SerializeField] PhotonView pv;
        bool bCanSimulate = false;

        private void Awake()
        {
            if(instance)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Start()
        {
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
            if (!spawner) spawner = NetworkSpawner.Instance;
            if (!pv) pv = GetComponent<PhotonView>();
            spawner.OnPostSpawn += StopTime;
        }

        private void Update()
        {
            if (!bCanSimulate) return;
            BtnSimulate.interactable = true;
        }

        public void OnClick()
        {
            DebugLogger.Instance.Log("Clicked");
            pv.RPC(nameof(StartSimulation), RpcTarget.AllBufferedViaServer);
        }

        public void OnClickParenting()
        {
            NetworkedRigidbody nrb = FindObjectOfType<NetworkedRigidbody>();
            //nrb.SetSyncInterval(1f/PhotonNetwork.SerializationRate);
            nrb.SetKinematics(true);
            nrb.ParentToPhotonView(pv);
        }

        public void OnClickUnparenting()
        {
            NetworkedRigidbody nrb = FindObjectOfType<NetworkedRigidbody>();
            //nrb.SetSyncInterval(1, true);
            nrb.SetKinematics(false);
            nrb.ParentToPhotonView(-1);
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

        internal void ReportGameFinished()
        {
            string extension = ".json";
            string path = Application.dataPath + "/Record_" + DateTime.Now.ToString().Replace('/', '_').Replace(':', '_');
            bool useNumbering = false;
            int i = 1;
            if(File.Exists(path + extension))
            {
                useNumbering = true;
                while (File.Exists(useNumbering ? path + string.Format(" ({0})", i) + extension : path + extension))
                    i++;
            }
            using (StreamWriter o = new StreamWriter(useNumbering ? path + string.Format(" ({0})", i) + extension : path + extension, false, Encoding.UTF8))
                o.WriteLine(AchievementManager.Instance.RequestRecords());
        }
    }
}