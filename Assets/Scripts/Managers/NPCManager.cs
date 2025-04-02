using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    private Dictionary<string, BaseNPC> npcRegistry = new Dictionary<string, BaseNPC>();
    private Dictionary<string, float> npcTrustLevels = new Dictionary<string, float>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Registers an NPC and restores its trust level if it was recorded.
    /// </summary>
    public void RegisterNPC(BaseNPC npc)
    {
        if (!npcRegistry.ContainsKey(npc.npcName))
        {
            npcRegistry[npc.npcName] = npc;

            // Restore trust level if exists, otherwise initialize it
            if (npcTrustLevels.ContainsKey(npc.npcName))
            {
                npc.TrustLevel = npcTrustLevels[npc.npcName];
            }
            else
            {
                npcTrustLevels[npc.npcName] = npc.TrustLevel;
            }

            Debug.Log($"Registerd {npc.npcName} with trust: {npc.TrustLevel}");
        }
    }

    /// <summary>
    /// Updates the stored trust level for an NPC.
    /// </summary>
    public void UpdateTrustLevel(string npcName, float trustLevel)
    {
        if (npcTrustLevels.ContainsKey(npcName))
        {
            npcTrustLevels[npcName] = trustLevel;
            npcRegistry[npcName].TrustLevel = trustLevel;
        }
        else
        {
            Debug.LogWarning($"[NPCManager] Attempted to update trust for {npcName}, but it's not registered.");
        }
    }

    /// <summary>
    /// Gets the trust level of an NPC.
    /// </summary>
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

    /// <summary>
    /// Clears NPC data (useful when resetting the game).
    /// </summary>
    public void ClearNPCData()
    {
        npcRegistry.Clear();
        npcTrustLevels.Clear();
    }
}
