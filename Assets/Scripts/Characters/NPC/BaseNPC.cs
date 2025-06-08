using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BaseNPC serves as an abstract class for all NPC characters.
/// It extends the Character class and manages NPC-specific properties and behaviors.
/// </summary>
public abstract class BaseNPC : Character
{
    [Header("NPC Basis")]
    public string aliasName = "unknown"; // Alternative name for the NPC
    public string npcName = "unknown"; // Official NPC name
    [Range(-1, 1)] public float initialTrustLevel = 0; // Initial trust level on a scale from -1 to 1
    [Space]
    public List<Sprite> spriteImages = new List<Sprite>(); // Stores NPC images

    [Header("Component Queue")]
    [SerializeField] protected List<BaseNPCComponent> npcComponents = new List<BaseNPCComponent>(); // List of components the NPC manages

    private Queue<BaseNPCComponent> componentQueue = new Queue<BaseNPCComponent>(); // Queue for managing NPC components
    private BaseNPCComponent currentComponent; // Stores the currently active component

    private float _trustLevel = 0; // Internal trust level variable

    /// <summary>
    /// Gets or sets the NPC's trust level, clamped between -1 and 1.
    /// </summary>
    public float TrustLevel
    {
        get { return _trustLevel; }
        set
        {
            _trustLevel = Mathf.Clamp(value, -1, 1);
        }
    }

    /// <summary>
    /// Initializes the NPC, registering it and queuing components.
    /// </summary>
    private void Start()
    {
        originalHeight = controller.height; // Store the original height of the NPC

        // Register this NPC instance within the NPCManager.
        NPCManager.Instance.RegisterNPC(this);

        // Assign and initialize all NPC components.
        foreach (BaseNPCComponent component in npcComponents)
        {
            component.SetOwner(this);
            componentQueue.Enqueue(component);
            component.Initialize();
        }
    }

    /// <summary>
    /// Activates the next NPC component in the queue.
    /// If no components remain, switches to dialogue mode.
    /// </summary>
    public void ActivateComponent()
    {
        // If there are no components left, trigger the default dialogue.
        if (componentQueue.Count == 0)
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
            DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefault("NothingToSay"));
            return;
        }

        // Activate the next component in the queue.
        currentComponent = componentQueue.Peek();
        currentComponent.OnComponentFinished += DequeueComponent;
        currentComponent.Activate();
        Debug.Log(currentComponent + " activated");
    }

    /// <summary>
    /// Removes the completed component from the queue and resets the reference.
    /// </summary>
    private void DequeueComponent(BaseNPCComponent finishedComponent)
    {
        // Ensure the finished component is the one at the front of the queue before dequeuing.
        if (componentQueue.Count > 0 && componentQueue.Peek() == finishedComponent)
        {
            componentQueue.Dequeue();
            finishedComponent.OnComponentFinished -= DequeueComponent;
            currentComponent = null; // Clear the reference to the active component
        }
    }

    /// <summary>
    /// Removes a specific component from the queue without activating it.
    /// </summary>
    /// <param name="target">The component to remove.</param>
    public void RemoveFromQueue(BaseNPCComponent target)
    {
        var tempQueue = new Queue<BaseNPCComponent>();

        // Rebuild the queue excluding the target component.
        while (componentQueue.Count > 0)
        {
            var item = componentQueue.Dequeue();
            if (item != target)
                tempQueue.Enqueue(item);
        }

        componentQueue = tempQueue;
    }
}
