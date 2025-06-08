using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the Gallery UI state of the phone,
/// displaying saved photos as a list of UI elements.
/// </summary>
public class PhoneGallery : PhoneUIState
{
    public GameObject imageUIPrefab;     // Prefab for each image UI element
    public RectTransform content;        // Parent container for image UI elements

    /// <summary>
    /// Returns the enum state type for this UI state.
    /// </summary>
    public override PhoneUIStateMachine.PhoneStates StateType => PhoneUIStateMachine.PhoneStates.Gallery;

    /// <summary>
    /// Called when entering the Gallery state.
    /// Activates the UI and builds the list of saved images.
    /// </summary>
    public override void EnterState(Dictionary<string, object> data = null)
    {
        gameObject.SetActive(true);

        BuildUIList();
    }

    /// <summary>
    /// Called when exiting the Gallery state.
    /// Deactivates the UI.
    /// </summary>
    public override void ExitState()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called each frame while in the Gallery state.
    /// (No implementation needed currently.)
    /// </summary>
    public override void UpdateState()
    {
        // No dynamic updates required in gallery state for now
    }

    /// <summary>
    /// Builds the gallery UI by loading saved image data,
    /// clearing previous UI elements, and instantiating new ones.
    /// </summary>
    private void BuildUIList()
    {
        List<ImageData> items = SaveNLoadManager.Instance.LoadPhotoData();

        ClearUIList();

        for (int i = 0; i < items.Count; i++)
        {
            ImageItemUI itemUI = Instantiate(imageUIPrefab, content.transform).GetComponent<ImageItemUI>();
            itemUI.data = items[i];
            itemUI.Initialize();
        }
    }

    /// <summary>
    /// Clears existing image UI elements from the content container.
    /// </summary>
    private void ClearUIList()
    {
        if (content.transform.childCount <= 0) return;

        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
}
