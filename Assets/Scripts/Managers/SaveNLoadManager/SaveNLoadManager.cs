using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents an image with its filename, description, and texture data.
/// </summary>
public class ImageData
{
    public string fileName;
    public string description;
    public Texture2D texture;

    public ImageData(string fileName, string description, Texture2D texture)
    {
        this.fileName = fileName;
        this.description = description;
        this.texture = texture;
    }
}

/// <summary>
/// Manager responsible for saving and loading player data, inventory, quests, NPC trust levels, and photos.
/// </summary>
public class SaveNLoadManager : BaseManager<SaveNLoadManager>
{
    private string photoSavePath;
    private string photoDataPath;

    /// <summary>
    /// Initializes paths and creates directories if they don't exist.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        photoSavePath = Path.Combine(Application.persistentDataPath, "Photos");
        if (!Directory.Exists(photoSavePath))
        {
            Directory.CreateDirectory(photoSavePath);
        }

        photoDataPath = Path.Combine(photoSavePath, "photos.json");
    }

    #region Wrappers

    /// <summary>
    /// Serializable class for storing player-related data for saving/loading.
    /// </summary>
    [Serializable]
    private class PlayerData
    {
        public Vector3 position;

        public PlayerData(Vector3 position)
        {
            this.position = position;
        }
    }

    /// <summary>
    /// Container for a list of photo metadata, used for JSON serialization.
    /// </summary>
    [Serializable]
    private class ImageMetaDataList
    {
        public List<ImageMetaData> photos = new List<ImageMetaData>();
    }

    /// <summary>
    /// Serializable metadata for a single photo.
    /// </summary>
    [Serializable]
    public class ImageMetaData
    {
        public string fileName;
        public string description;

        public ImageMetaData(string fileName, string description)
        {
            this.fileName = fileName;
            this.description = description;
        }
    }

    /// <summary>
    /// Represents an entry of NPC trust data with NPC name and trust level.
    /// </summary>
    [Serializable]
    public class NPCTrustEntry
    {
        public string npcName;
        public float trustLevel;
    }

    /// <summary>
    /// Serializable container for a list of NPC trust entries.
    /// </summary>
    [Serializable]
    public class NPCTrustData
    {
        public List<NPCTrustEntry> entries = new List<NPCTrustEntry>();
    }

    /// <summary>
    /// Generic wrapper to serialize a list of items into JSON.
    /// </summary>
    [Serializable]
    public class ListWrapper<T>
    {
        public List<T> items;

        public ListWrapper(List<T> items)
        {
            this.items = items;
        }
    }
    #endregion

    /// <summary>
    /// Saves the current game checkpoint including player, inventory, quests, and NPC trust levels.
    /// </summary>
    public void MarkCheckpoint()
    {
        SavePlayer();
        SaveInventory();
        SaveQuest();
        SaveTrustLevels();
    }

    #region Saving

    /// <summary>
    /// Saves a photo's texture as PNG and updates photo metadata JSON file.
    /// </summary>
    /// <param name="data">ImageData object containing texture and description.</param>
    public void SavePhoto(ImageData data)
    {
        // Save PNG texture
        byte[] bytes = data.texture.EncodeToPNG();
        string pngPath = Path.Combine(photoSavePath, data.fileName + ".png");
        File.WriteAllBytes(pngPath, bytes);

        // Load existing metadata or create new
        ImageMetaDataList metaDataList = LoadMetaDataList();

        // Check if this photo already exists in metadata, update or add
        var existing = metaDataList.photos.Find(p => p.fileName == data.fileName);
        if (existing != null)
        {
            existing.description = data.description;
        }
        else
        {
            metaDataList.photos.Add(new ImageMetaData(data.fileName, data.description));
        }

        // Save updated metadata JSON
        string json = JsonUtility.ToJson(metaDataList, true);
        File.WriteAllText(photoDataPath, json);
    }

    /// <summary>
    /// Saves the player's current inventory as JSON.
    /// </summary>
    public void SaveInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, "inventory.json");
        List<InventoryItem> items = InventoryManager.Instance.GetInventoryList().ToList<InventoryItem>();

        ListWrapper<InventoryItem> wrapper = new ListWrapper<InventoryItem>(items);

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
        Debug.Log("Inventory Saved to: " + path);
    }

    /// <summary>
    /// Saves the currently active quests as JSON.
    /// </summary>
    public void SaveQuest()
    {
        string path = Path.Combine(Application.persistentDataPath, "quest.json");
        List<Quest> items = QuestManager.Instance.GetActiveQuest();

        ListWrapper<Quest> wrapper = new ListWrapper<Quest>(items);

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
        Debug.Log("Quest Saved to: " + path);
    }

    /// <summary>
    /// Saves NPC trust levels as JSON data.
    /// </summary>
    public void SaveTrustLevels()
    {
        NPCTrustData data = new NPCTrustData();

        Dictionary<string, float> npcTrustLevels = NPCManager.Instance.GetNPCTrusts();

        foreach (var kvp in npcTrustLevels)
        {
            data.entries.Add(new NPCTrustEntry
            {
                npcName = kvp.Key,
                trustLevel = kvp.Value
            });
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "npc_trust.json");
        File.WriteAllText(path, json);
        Debug.Log("NPC trust levels saved to " + path);
    }

    /// <summary>
    /// Saves the player's current position.
    /// </summary>
    public void SavePlayer()
    {
        Player player = GameManager.Instance.GetPlayer();

        PlayerData data = new PlayerData(player.transform.position + (Vector3.up * 0.5f));

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "player.json");
        File.WriteAllText(path, json);
        Debug.Log("Player location saved to " + path);
    }
    #endregion

    #region Loading

    /// <summary>
    /// Loads photo textures and their metadata from disk.
    /// </summary>
    /// <returns>List of loaded ImageData objects.</returns>
    public List<ImageData> LoadPhotoData()
    {
        List<ImageData> photos = new List<ImageData>();

        ImageMetaDataList metaDataList = LoadMetaDataList();

        foreach (var meta in metaDataList.photos)
        {
            string pngPath = Path.Combine(photoSavePath, meta.fileName + ".png");
            if (File.Exists(pngPath))
            {
                byte[] bytes = File.ReadAllBytes(pngPath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                texture.name = meta.fileName;

                photos.Add(new ImageData(meta.fileName, meta.description, texture));
            }
            else
            {
                Debug.LogWarning($"Missing texture file for {meta.fileName}");
            }
        }

        return photos;
    }

    /// <summary>
    /// Loads the player's saved inventory from JSON file.
    /// </summary>
    /// <returns>List of InventoryItem loaded from disk.</returns>
    public List<InventoryItem> LoadInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, "inventory.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ListWrapper<InventoryItem> wrapper = JsonUtility.FromJson<ListWrapper<InventoryItem>>(json);
            Debug.Log("Inventory Loaded");
            return wrapper.items;
        }
        else
        {
            Debug.LogWarning("Inventory Save file not found!");
            return new List<InventoryItem>();
        }
    }

    /// <summary>
    /// Loads active quests from JSON save file.
    /// </summary>
    /// <returns>List of Quest objects loaded from disk.</returns>
    public List<Quest> LoadQuests()
    {
        string path = Path.Combine(Application.persistentDataPath, "quest.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ListWrapper<Quest> wrapper = JsonUtility.FromJson<ListWrapper<Quest>>(json);
            Debug.Log("Quest Loaded");
            return wrapper.items;
        }
        else
        {
            Debug.LogWarning("Quest Save file not found!");
            return new List<Quest>();
        }
    }

    /// <summary>
    /// Loads NPC trust levels from JSON save file.
    /// </summary>
    /// <returns>Dictionary mapping NPC names to trust levels.</returns>
    public Dictionary<string, float> LoadTrustLevels()
    {
        string path = Path.Combine(Application.persistentDataPath, "npc_trust.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning("Trust level file not found.");
            return new Dictionary<string, float>();
        }

        string json = File.ReadAllText(path);
        NPCTrustData data = JsonUtility.FromJson<NPCTrustData>(json);

        Dictionary<string, float> trustLevels = new Dictionary<string, float>();

        foreach (var entry in data.entries)
        {
            trustLevels[entry.npcName] = entry.trustLevel;
        }

        return trustLevels;
    }

    /// <summary>
    /// Loads the player's saved position from disk.
    /// </summary>
    /// <returns>Vector3 position loaded from save file, or current player position if no save found.</returns>
    public Vector3 LoadPlayer()
    {
        Player player = GameManager.Instance.GetPlayer();

        string path = Path.Combine(Application.persistentDataPath, "player.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Player Loaded");
            return data.position;
        }
        else
        {
            Debug.LogWarning("Player save file not found.");
            return player.transform.position;
        }
    }

    /// <summary>
    /// Helper method to load photo metadata list from JSON file or create a new empty list if none exists.
    /// </summary>
    /// <returns>ImageMetaDataList instance loaded from file or new empty list.</returns>
    private ImageMetaDataList LoadMetaDataList()
    {
        if (File.Exists(photoDataPath))
        {
            string json = File.ReadAllText(photoDataPath);
            return JsonUtility.FromJson<ImageMetaDataList>(json);
        }
        return new ImageMetaDataList();
    }
    #endregion

    /// <summary>
    /// Deletes all saved data in the persistent data path directory.
    /// </summary>
    public void DeleteAllSaveData()
    {
        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            // Delete all files
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to delete file: {file}\n{e}");
                }
            }

            // Delete all subdirectories
            string[] directories = Directory.GetDirectories(path);
            foreach (string dir in directories)
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to delete directory: {dir}\n{e}");
                }
            }

            Debug.Log("All save data in persistentDataPath deleted.");
        }
        else
        {
            Debug.LogWarning("persistentDataPath does not exist.");
        }
    }
}
