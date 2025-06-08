using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI labelText;
    [SerializeField] RawImage rawImage;
    [Space]
    public ImageData data;

    public void Initialize()
    {
        if(data != null)
        {
            labelText.text = data.fileName;
            rawImage.texture = data.texture;
        }
    }

    public void InspectImage()
    {
        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Inspection, data);
    }
}
