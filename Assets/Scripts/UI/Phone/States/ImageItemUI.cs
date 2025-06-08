using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a single image item in the phone gallery UI.
/// Displays the image thumbnail and filename, and allows inspection of the full image.
/// </summary>
public class ImageItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI labelText;  // UI text to display the image file name
    [SerializeField] RawImage rawImage;           // UI element to show the image texture

    [Space]
    public ImageData data;                         // Data representing the saved image

    /// <summary>
    /// Initializes the UI with the data assigned.
    /// Sets the label and image texture if data is available.
    /// </summary>
    public void Initialize()
    {
        if (data != null)
        {
            labelText.text = data.fileName;
            rawImage.texture = data.texture;
        }
    }

    /// <summary>
    /// Called when the user wants to inspect the image in detail.
    /// Switches to the inspection panel, passing the image data.
    /// </summary>
    public void InspectImage()
    {
        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Inspection, data);
    }
}
