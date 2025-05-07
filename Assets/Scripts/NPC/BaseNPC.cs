using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNPC : MonoBehaviour
{
    //titigan ni player
    [Header("References")]
    public Transform lookAt;
    public Animator animator;

    [Header("Properties")]
    public string aliasName = "unknown";
    public string npcName = "unknown";

    private float _trustLevel = 0;

    private Player _player = null;


    private string currentAnimation = "";
    public string GetCurrentAnimation
    {
        get
        {
            return currentAnimation.Replace($"{npcName}|", "");
        }
    }

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

    #region SetGet
    public Player GetPlayer()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        return _player;
    }
    #endregion

    #region Actions
    public void DoAnimate(string newAnimation, float blend = 0)
    {
        if (currentAnimation != newAnimation)
        {
            currentAnimation = newAnimation;
            animator.CrossFade(currentAnimation, blend);
        }
    }

    public bool IsAnimationPlaying(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)
;
    }
    #endregion
}
