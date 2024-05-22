//this code is intentionally written to be desynced, as the purpose of it
//  is to see how desynced the remote rigidbody compared to original copy
//  in the local device.

using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager instance;
    public static AchievementManager Instance => instance;
    [SerializeField] Records records;
    [SerializeField] string antagonistTag = "antagonist", protagonistTag = "protagonist";

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

    internal void Report(string shouldReactToTag, int index)
    {
        if(shouldReactToTag == antagonistTag)
            records.AddAntaRecord(Time.time);
        else if (shouldReactToTag == protagonistTag)
            records.AddProtaRecord(Time.time);
    }

    internal Records RequestRecordsRaw()
        => records;
    internal string RequestRecords()
        => records.Format();
}

[System.Serializable]
public class Records
{
    [SerializeField] List<float> protaRecords;
    [SerializeField] List<float> antaRecords;

    public Records()
    {
        protaRecords = new List<float>();
        antaRecords = new List<float>();
    }
    public Records(List<float> protaRecords, List<float> antaRecords)
    {
        this.protaRecords = protaRecords;
        this.antaRecords = antaRecords;
    }

    public void AddProtaRecord(float record)
        => protaRecords.Add(record);
    public void AddAntaRecord(float record)
        => antaRecords.Add(record);

    public string Format()
        => JsonUtility.ToJson(this);
}