using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : BaseManager<NPCManager>
{
    private Dictionary<string, float> npcTrustLevels = new Dictionary<string, float>();

    private void Start()
    {
        npcTrustLevels = SaveNLoadManager.Instance.LoadTrustLevels();
    }

    public void RegisterNPC(BaseNPC npc)
    {
        if (npcTrustLevels.ContainsKey(npc.npcName))
        {
            npc.TrustLevel = npcTrustLevels[npc.npcName];
        }
        else
        {
            npcTrustLevels[npc.npcName] = npc.TrustLevel;
            UpdateTrustLevel(npc.npcName, npc.initialTrustLevel);
        }
    }

    public void UpdateTrustLevel(string npcName, float trustLevel)
    {
        if (npcTrustLevels.ContainsKey(npcName))
        {
            npcTrustLevels[npcName] = trustLevel;
        }
        else
        {
            Debug.LogWarning($"[NPCManager] Attempted to update trust for {npcName}, but it's not registered.");
        }
    }

    public float GetTrustLevel(string npcName)
    {
        return npcTrustLevels.ContainsKey(npcName) ? npcTrustLevels[npcName] : 0;
    }

    public float GetTrustGrade()
    {
        float grade = 0;

        foreach (KeyValuePair<string, float> npc in npcTrustLevels)
        {
            grade += npc.Value;
        }

        return grade;
    }

    public Dictionary<string, float> GetNPCTrusts()
    {
        return npcTrustLevels;
    }

    public void ClearNPCData()
    {
        npcTrustLevels.Clear();
    }
}
