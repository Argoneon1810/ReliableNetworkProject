using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementTarget : MonoBehaviour
{
    [SerializeField] string shouldReactToTag = "protagonist";
    [SerializeField] AchievementManager manager;
    [SerializeField] int index = 0;
    private void Start()
    {
        manager = AchievementManager.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(shouldReactToTag)) return;
        manager.Report(shouldReactToTag, index);
        Destroy(this);
    }
}
