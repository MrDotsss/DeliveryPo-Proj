using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNPC : Character
{
    [Header("NPC Basis")]
    public string aliasName = "unknown";
    public string npcName = "unknown";
    [Range(-1, 1)] public float initialTrustLevel = 0;
    [Space]
    public List<Sprite> spriteImages = new List<Sprite>();

    [Header("Component Queue")]
    [SerializeField] protected List<BaseNPCComponent> npcComponents = new List<BaseNPCComponent>();

    private Queue<BaseNPCComponent> componentQueue = new Queue<BaseNPCComponent>();
    private BaseNPCComponent currentComponent;

    private float _trustLevel = 0;

    public float TrustLevel
    {
        get { return _trustLevel; }
        set
        {
            _trustLevel = value;
            _trustLevel = Mathf.Clamp(_trustLevel, -1, 1);
        }
    }

    protected virtual void Start()
    {
        NPCManager.Instance.RegisterNPC(this);

        foreach (BaseNPCComponent component in npcComponents)
        {
            component.SetOwner(this);
            componentQueue.Enqueue(component);
        }
    }

    public void ActivateComponent()
    {
        if (componentQueue.Count == 0)
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
            DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefault("NothingToSay"));
            return;
        }

        currentComponent = componentQueue.Peek();
        currentComponent.OnComponentFinished += DequeueComponent;
        currentComponent.Activate();
        Debug.Log(currentComponent + " activated");
    }

    private void DequeueComponent(BaseNPCComponent finishedComponent)
    {
        if (componentQueue.Count > 0 && componentQueue.Peek() == finishedComponent)
        {
            componentQueue.Dequeue();
            finishedComponent.OnComponentFinished -= DequeueComponent;
            currentComponent = null;
        }
    }

    public void RemoveFromQueue(BaseNPCComponent target)
    {
        var tempQueue = new Queue<BaseNPCComponent>();

        while (componentQueue.Count > 0)
        {
            var item = componentQueue.Dequeue();
            if (item != target)
                tempQueue.Enqueue(item);
        }

        componentQueue = tempQueue;
    }


}
