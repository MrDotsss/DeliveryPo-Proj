using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneGallery : PhoneUIState
{
    public GameObject imageUIPrefab;
    public RectTransform content;

    public override PhoneUIStateMachine.PhoneStates StateType => PhoneUIStateMachine.PhoneStates.Gallery;

    public override void EnterState(Dictionary<string, object> data = null)
    {
        gameObject.SetActive(true);

        BuildUIList();
    }
    public override void ExitState()
    {
        gameObject.SetActive(false);

    }

    public override void UpdateState()
    {

    }
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

    private void ClearUIList()
    {
        if (content.transform.childCount <= 0) return;

        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
}
