using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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

public class SaveNLoadManager : BaseManager<SaveNLoadManager>
{
    private string photoSavePath;
    private string photoDataPath;

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
    // Container class for JSON serialization of list

    [Serializable]
    private class PlayerData
    {
        public Vector3 position;

        //Add more data to save if necessary

        public PlayerData(Vector3 position)
        {
            this.position = position;
        }
    }

    [Serializable]
    private class ImageMetaDataList
    {
        public List<ImageMetaData> photos = new List<ImageMetaData>();
    }
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

    [Serializable]
    public class NPCTrustEntry
    {
        public string npcName;
        public float trustLevel;
    }

    [Serializable]
    public class NPCTrustData
    {
        public List<NPCTrustEntry> entries = new List<NPCTrustEntry>();
    }

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

    public void MarkCheckpoint()
    {
        SavePlayer();
        SaveInventory();
        SaveQuest();
        SaveTrustLevels();
    }

    #region Saving
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

    public void SaveInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, "inventory.json");
        List<InventoryItem> items = InventoryManager.Instance.GetInventoryList().ToList<InventoryItem>();

        ListWrapper<InventoryItem> wrapper = new ListWrapper<InventoryItem>(items);

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
        Debug.Log("Inventory Saved to: " + path);
    }

    public void SaveQuest()
    {
        string path = Path.Combine(Application.persistentDataPath, "quest.json");
        List<Quest> items = QuestManager.Instance.GetActiveQuest();

        ListWrapper<Quest> wrapper = new ListWrapper<Quest>(items);

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
        Debug.Log("Quest Saved to: " + path);
    }

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

    public List<Quest> LoadQuests()
    {
        string path = Path.Combine(Application.persistentDataPath, "quest.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ListWrapper<Quest> wrapper = JsonUtility.FromJson<ListWrapper<Quest>>(json);
            Debug.Log("Inventory Loaded");
            return wrapper.items;
        }
        else
        {
            Debug.LogWarning("Inventory Save file not found!");
            return new List<Quest>();
        }
    }

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

    public Vector3 LoadPlayer()
    {
        Player player = GameManager.Instance.GetPlayer();

        string path = Path.Combine(Application.persistentDataPath, "player.json");

        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Player Loaded");
            return data.position;
        } else
        {
            Debug.LogWarning("Trust level file not found.");
            return player.transform.position;
        }

    }

    // Helper to load metadata list from JSON or create new empty one
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