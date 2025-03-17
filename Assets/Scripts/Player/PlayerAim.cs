using UnityEngine;
using TMPro; // For UI Text Display

public class PlayerAim : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float reachDistance = 3f; // How far the player can interact
    public LayerMask interactableLayer; // Assign a layer for interactables

    [Header("UI Elements")]
    public TextMeshProUGUI interactionText; // UI Text for interaction display
    public GameObject interactionPanel; // UI Panel that holds the text

    private Camera playerCamera;
    private IInteractable currentInteractable;

    void Start()
    {
        playerCamera = Camera.main;
        interactionPanel.SetActive(false); // Hide interaction UI initially
    }

    void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, reachDistance, interactableLayer))
        {
            // Check if the object implements IInteractable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                    ShowInteraction(interactable.GetInteractionText);
                }

                if (InputManager.Instance.input.actions["interact"].WasPressedThisFrame())
                {
                    interactable.Interact();
                }
            }
        }
        else
        {
            HideInteraction();
            currentInteractable = null;
        }
    }

    private void ShowInteraction(string interactionType)
    {
        interactionText.text = interactionType;
        interactionPanel.SetActive(true);
    }

    private void HideInteraction()
    {
        interactionPanel.SetActive(false);
    }
}
