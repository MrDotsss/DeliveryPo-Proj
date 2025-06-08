using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIState : MonoBehaviour
{
    public abstract UIManager.EUIPanels State { get; }

    protected virtual void Awake()
    {
        UIManager.Instance.RegisterUIPanel(this);
        gameObject.SetActive(false);
    }

    public abstract void Enter(object data = null);

    public virtual void UpdateState()
    {

    }

    public abstract void Exit();
}
