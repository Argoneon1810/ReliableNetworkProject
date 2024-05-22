using UnityEngine;
using Photon.Pun;

public class HandShakeTimeTracer : MonoBehaviour
{
    public static HandShakeTimeTracer Instance { get; private set; }
    [SerializeField] PhotonView pv;
    bool received = true;
    float pre;
    int t = 1;

    [SerializeField] float handShakeTime = 0;
    public float HandShakeTime { get => handShakeTime; set => pv.RPC(nameof(SetHandShakeTime), RpcTarget.AllBufferedViaServer, value); }
    [PunRPC] void SetHandShakeTime(float handShakeTime) => this.handShakeTime = handShakeTime;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else
        {
            FindObjectOfType<NetworkSpawner>().RequestDestroy(gameObject);
            return;
        }
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (received.AsTrigger())
        {
            pre = Time.time;
            pv.RPC(nameof(Tic), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void Tic()
    {
        if (pv.IsMine) return;
        pv.RPC(nameof(Toc), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void Toc()
    {
        if (!pv.IsMine) return;
        received = true;
        float delta = Time.time - pre;
        HandShakeTime = (HandShakeTime * 0.9f + delta * 0.1f) / (1 - Mathf.Pow(0.9f, t = ++t > 100 ? 100 : t));
        DebugLogger.Instance.Log("Network Delay: " + delta + "s");
    }
}