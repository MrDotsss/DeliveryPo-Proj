using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNPC : MonoBehaviour
{
    //titigan ni player
    public Transform lookAt;

    [Header("Properties")]
    public string aliasName = "unknown";
    public string npcName = "unknown";

    private float _trustLevel = 0;


    public void Initialize()
    {
        NPCManager.Instance.RegisterNPC(this);
    }

    public float TrustLevel
    {
        get { return _trustLevel; }
        set
        {
            _trustLevel = value;
            _trustLevel = Mathf.Clamp(_trustLevel, -1, 1);
        }
    }
}
