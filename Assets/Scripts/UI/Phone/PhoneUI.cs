using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneUI : UIState
{
    [SerializeField] private PhoneUIStateMachine stateMachine;

    public override UIManager.EUIPanels State => UIManager.EUIPanels.Phone;

    public GameObject phonePrefab;
    private PhoneCam phone;
    private GameObject currentPhone;

    private Player player;

    public override void Enter(object data = null)
    {
        stateMachine.Start();
        gameObject.SetActive(true);

        if(currentPhone == null)
        {
            InstancePhone();
        } else
        {
            phone?.DisablePhone();
        }
    }

    public override void Exit()
    {
        gameObject.SetActive(false);
    }

    private void InstancePhone()
    {
        if(player == null) player = GameManager.Instance.GetPlayer();

        if(player != null)
        {
            currentPhone = Instantiate(phonePrefab, player.Hand.transform);
            currentPhone.transform.localPosition = Vector3.zero;
            currentPhone.transform.localRotation = Quaternion.identity;
            currentPhone.GetComponentInChildren<MeshRenderer>().material.renderQueue = 4000;
            currentPhone.layer = LayerMask.NameToLayer("Hand");
            phone = currentPhone.GetComponent<PhoneCam>();

            currentPhone.SetActive(false);
        }
    }

    public void OnAppPressed(string app)
    {
        PhoneUIStateMachine.PhoneStates switchTo = PhoneUIStateMachine.PhoneStates.MainMenu;

        switch(app)
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
