//this code is intentionally written to be desynced, as the purpose of it
//  is to see how desynced the remote rigidbody compared to original copy
//  in the local device.

using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NRB = NetworkedRigidbody;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager instance;
    public static AchievementManager Instance => instance;
    [SerializeField] Records records;
    [SerializeField] string antagonistTag = "antagonist", protagonistTag = "protagonist";

    bool prefabsfound = false;

    void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        records = new Records();
    }

    private void Update()
    {
        if (prefabsfound) return;
        PhotonRigidbodyView prbv = FindObjectOfType<PhotonRigidbodyView>();
        NRB.NetworkedRigidbody nrb = FindObjectOfType<NRB.NetworkedRigidbody>();
        if (!prbv || !nrb) return;
        prefabsfound = true;
        prbv.OnNetworkCall += AntaNetworkCall;
        nrb.OnNetworkCall += ProtaNetworkCall;
    }

    internal void Report(string shouldReactToTag, Vector3 pos)
    {
        if (shouldReactToTag == protagonistTag)
            records.AddProtaRecord(Time.time, pos); 
        else if (shouldReactToTag == antagonistTag)
            records.AddAntaRecord(Time.time, pos);
    }

    internal void ProtaNetworkCall()
        => records.AddProtaNetworkCall(Time.time);
    internal void AntaNetworkCall()
        => records.AddAntaNetworkCall(Time.time);

    internal Records RequestRecordsRaw()
        => records;
    internal string RequestRecords()
        => records.Format();
}

[System.Serializable]
public class Records
{
    [System.Serializable]
    public class Element
    {
        public float time;
        public Vector3 pos;
        public Element() : this(-1, -Vector3.one) { }
        public Element(float time, Vector3 pos)
        {
            this.time = time;
            this.pos = pos;
        }
    }
    [SerializeField] List<Element> protaRecords;
    [SerializeField] List<Element> antaRecords;
    [SerializeField] List<float> protaNetworkCalls;
    [SerializeField] List<float> antaNetworkCalls;

    public Records()
    {
        protaRecords = new List<Element>();
        antaRecords = new List<Element>();
        protaNetworkCalls = new List<float>();
        antaNetworkCalls = new List<float>();
    }

    public void AddProtaRecord(float record, Vector3 pos)
        => protaRecords.Add(new Element(record, pos));
    public void AddAntaRecord(float record, Vector3 pos)
        => antaRecords.Add(new Element(record, pos));
    public void AddProtaNetworkCall(float record)
        => protaNetworkCalls.Add(record);
    public void AddAntaNetworkCall(float record)
        => antaNetworkCalls.Add(record);

    public string Format()
        => JsonUtility.ToJson(this);
}