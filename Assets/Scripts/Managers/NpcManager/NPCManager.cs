using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager responsible for tracking NPC trust levels.
/// Loads saved trust levels and provides methods to register NPCs,
/// update trust levels, retrieve trust info, and clear data.
/// </summary>
public class NPCManager : BaseManager<NPCManager>
{
    // Dictionary to store trust levels keyed by NPC name
    private Dictionary<string, float> npcTrustLevels = new Dictionary<string, float>();

    private void Start()
    {
        // Load saved trust levels on start
        npcTrustLevels = SaveNLoadManager.Instance.LoadTrustLevels();
    }

    /// <summary>
    /// Registers an NPC by setting its trust level from saved data if present,
    /// otherwise adds the NPC's initial trust level to the dictionary.
    /// </summary>
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

    /// <summary>
    /// Updates the trust level for a registered NPC.
    /// Logs a warning if the NPC is not registered.
    /// </summary>
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

    /// <summary>
    /// Retrieves the trust level for a given NPC name.
    /// Returns 0 if NPC is not found.
    /// </summary>
    public float GetTrustLevel(string npcName)
    {
        return npcTrustLevels.ContainsKey(npcName) ? npcTrustLevels[npcName] : 0;
    }

    /// <summary>
    /// Calculates and returns the total trust grade by summing all NPC trust levels.
    /// </summary>
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
    /// Returns the full dictionary of NPC trust levels.
    /// </summary>
    public Dictionary<string, float> GetNPCTrusts()
    {
        return npcTrustLevels;
    }

    /// <summary>
    /// Clears all NPC trust level data.
    /// </summary>
    public void ClearNPCData()
    {
        npcTrustLevels.Clear();
    }
}
