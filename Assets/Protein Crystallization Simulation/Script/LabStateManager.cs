using System.Collections.Generic;
using UnityEngine;

public class LabStateManager : MonoBehaviour
{
    public static LabStateManager Instance;

    public List<LabItemInstance> carriedItems = new List<LabItemInstance>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public LabItemInstance GetItem(string id)
    {
        return carriedItems.Find(i => i.instanceID == id);
    }

    public void AddOrUpdateItem(LabItemInstance item)
    {
        var existing = GetItem(item.instanceID);
        if (existing != null) carriedItems.Remove(existing);
        carriedItems.Add(item);
    }
}
