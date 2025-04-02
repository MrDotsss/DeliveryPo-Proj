using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;
    private string photoSavePath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        photoSavePath = Path.Combine(Application.persistentDataPath, "Photos");
        if (!Directory.Exists(photoSavePath))
        {
            Directory.CreateDirectory(photoSavePath);
        }
    }

    public void SavePhoto(Texture2D photo, string fileName)
    {
        byte[] bytes = photo.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(photoSavePath, fileName + ".png"), bytes);
    }

    public List<Texture2D> LoadPhotos()
    {
        List<Texture2D> photos = new List<Texture2D>();

        foreach (string file in Directory.GetFiles(photoSavePath, "*.png"))
        {
            byte[] bytes = File.ReadAllBytes(file);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            // Extract file name without extension
            string fileName = Path.GetFileNameWithoutExtension(file);
            texture.name = fileName; // Assign name to texture

            photos.Add(texture);
        }

        return photos;
    }


    public void DeletePhoto(string fileName)
    {
        string filePath = Path.Combine(photoSavePath, fileName + ".png");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
