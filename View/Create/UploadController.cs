using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class Zboom : MonoBehaviour
    {
        public GameObject UploadPanel;

        public TMP_InputField MapNameInput;
        public Button UploadOkButton;
        public Button UploadCancelButton;
        public Button SnapshotButton;
        public RawImage PreviewImage;
        public TextMeshProUGUI SaveStatus;

        private string m_MapName = "";
        private Texture2D m_CapturedImage;

        private void OnEnable()
        {
            StopUpload();
            UploadPanel.gameObject.SetActive(false);
            var buttonText = UploadOkButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "Сохранить";
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        public string MapName
        {
            get => m_MapName;
            set
            {
                m_MapName = value;
                MapNameInput.text = m_MapName;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetPreview(Texture2D capturedImage)
        {
            PreviewImage.texture = m_CapturedImage;
        }

        public void SetSuccess()
        {
        }

        public void SetError()
        {
            var buttonText = UploadOkButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "Повторить";
        }

        public void StartUpload()
        {
            UploadOkButton.interactable = false;
            MapNameInput.interactable = false;
            UploadCancelButton.interactable = false;
            SnapshotButton.interactable = false;
        }

        public void StopUpload()
        {
            UploadOkButton.interactable = true;
            MapNameInput.interactable = true;
            UploadCancelButton.interactable = true;
            SnapshotButton.interactable = true;
        }
    }
}