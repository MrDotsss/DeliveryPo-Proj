using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneGalleryState : PhoneState
{
    public override EPhoneStates StateType => EPhoneStates.Gallery;

    [SerializeField] private GameObject photoPrefab;
    [SerializeField] private Transform galleryGrid;
    [SerializeField] private GameObject galleryUI;
    [SerializeField] private GameObject imageView;


    private List<GameObject> displayedPhotos = new List<GameObject>();

    private bool isImageView = false;

    public override void Enter(Dictionary<string, object> data = null)
    {
        galleryUI.SetActive(true);
        LoadGallery();
    }

    public override void Exit()
    {
        galleryUI.SetActive(false);
        ClearGallery();
    }

    public override void PhysicsUpdate()
    {

    }

    public override void UpdateState()
    {
        if (phone.currentPressed != "gallery" && !isImageView)
        {
            stateMachine.TransitionTo(phone.GetState());
        }

        if (isImageView && phone.currentPressed == "back")
        {
            CloseImage();
        }

        phone.DoCamAim(isImageView);
    }

    private void LoadGallery()
    {
        List<Texture2D> savedPhotos = SaveLoadManager.Instance.LoadPhotos();

        foreach (Texture2D photo in savedPhotos)
        {
            GameObject photoObj = Instantiate(photoPrefab, galleryGrid);
            photoObj.transform.Find("Image").GetComponent<RawImage>().texture = photo;
            photoObj.transform.Find("Image").GetComponent<Button>().onClick.AddListener(() => ShowImage(photo));


            TextMeshProUGUI descText = photoObj.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            descText.text = photo.name;

            displayedPhotos.Add(photoObj);

        }
    }

    private void ClearGallery()
    {
        foreach (GameObject photo in displayedPhotos)
        {
            Destroy(photo);
        }
        displayedPhotos.Clear();
    }

    private void ShowImage(Texture2D photo)
    {
        isImageView = true;
        imageView.SetActive(true);
        imageView.GetComponent<RawImage>().texture = photo;

        phone.canInput = false;
        phone.player.canInput = false;
        phone.player.cam.canInput = false;
        InputManager.Instance.SetCursor(false, false);
    }

    private void CloseImage()
    {
        isImageView = false;
        imageView.SetActive(false);
        imageView.GetComponent<RawImage>().texture = null;

        phone.currentPressed = "gallery";
        phone.canInput = true;
        phone.player.canInput = true;
        phone.player.cam.canInput = true;
        InputManager.Instance.SetCursor(true, true);
    }
}
