using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the Phone UI panel and its state transitions.
/// Handles instantiation of the phone model, switching apps, and enabling/disabling phone camera.
/// </summary>
public class PhoneUI : UIState
{
    [SerializeField] private PhoneUIStateMachine stateMachine;

    // The UI panel this class controls
    public override UIManager.EUIPanels State => UIManager.EUIPanels.Phone;

    public GameObject phonePrefab;
    private PhoneCam phone;
    private GameObject currentPhone;

    private Player player;

    /// <summary>
    /// Called when the Phone UI panel is opened.
    /// Initializes the phone object if needed and activates the UI.
    /// </summary>
    public override void Enter(object data = null)
    {
        stateMachine.Start();
        gameObject.SetActive(true);

        if (currentPhone == null)
        {
            InstancePhone();
        }
        else
        {
            phone?.DisablePhone();
        }
    }

    /// <summary>
    /// Called when the Phone UI panel is closed.
    /// Deactivates the UI GameObject.
    /// </summary>
    public override void Exit()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Instantiates the phone prefab as a child of the player's hand.
    /// Sets proper positioning, rotation, rendering order, and layer.
    /// </summary>
    private void InstancePhone()
    {
        if (player == null) player = GameManager.Instance.GetPlayer();

        if (player != null)
        {
            currentPhone = Instantiate(phonePrefab, player.Hand.transform);
            currentPhone.transform.localPosition = Vector3.zero;
            currentPhone.transform.localRotation = Quaternion.identity;
            // Ensure phone renders on top of other objects in the scene
            currentPhone.GetComponentInChildren<MeshRenderer>().material.renderQueue = 4000;
            currentPhone.layer = LayerMask.NameToLayer("Hand");
            phone = currentPhone.GetComponent<PhoneCam>();

            currentPhone.SetActive(false);
        }
    }

    /// <summary>
    /// Handles phone app button presses.
    /// Switches the phone UI state machine to the requested app state,
    /// and enables the phone camera if the camera app is selected.
    /// </summary>
    /// <param name="app">The name of the app to switch to</param>
    public void OnAppPressed(string app)
    {
        PhoneUIStateMachine.PhoneStates switchTo = PhoneUIStateMachine.PhoneStates.MainMenu;

        switch (app)
        {
            case "delivery":
                switchTo = PhoneUIStateMachine.PhoneStates.Delivery;
                break;
            case "notes":
                switchTo = PhoneUIStateMachine.PhoneStates.Notes;
                break;
            case "map":
                switchTo = PhoneUIStateMachine.PhoneStates.Map;
                break;
            case "camera":
                phone?.EnablePhone();
                UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Phone);
                break;
            case "gallery":
                switchTo = PhoneUIStateMachine.PhoneStates.Gallery;
                break;
            case "call":
                switchTo = PhoneUIStateMachine.PhoneStates.Caller;
                break;
        }

        stateMachine.TransitionTo(switchTo);
    }
}
