using Common;
using System;
using System.Collections;
using Michsky.UI.ModernUIPack;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZBoom.Common.SpatialMap
{
    public class SpatialMapGameObjectController1 : MonoBehaviour
    {
        public PropGridController PropGridController;
        public GameObject OutlinePrefab;

        public SwitchManager FreeMoveSwitch;
        public SwitchManager VideoSwitch;
        public SwitchManager CloudPointSwitch;

        public GameObject StopEditButton;
        public GameObject DeleteButton;
        public GameObject DragonAddPointButton;

        private PropItemController m_SelectedPropItemController;
        private TouchController m_TouchControl;
        private MapSession m_MapSession;
        private GameObject m_SelectedGameObject;
        private bool m_IsMoveFree = true;
        [SerializeField] private bool m_UseCenterPosition = true;

        public event Action<GameObject> CreateObjectEvent;
        public event Action<GameObject> DeleteObjectEvent;

        private void Awake()
        {
            m_SelectedPropItemController = null;

            m_TouchControl = GetComponentInChildren<TouchController>(true);
            OutlinePrefab = Instantiate(OutlinePrefab);
            OutlinePrefab.SetActive(false);

            StopEditButton.SetActive(false);
            DragonAddPointButton.SetActive(false);
            DeleteButton.SetActive(false);

            //VideoSwitch.isOn = false;
            CloudPointSwitch.isOn = true;
            FreeMoveSwitch.isOn = false;
        }

        private void Update()
        {
            transform.position = Input.mousePosition;
            var isEditorOrStandalone = Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer ||
                                       Application.platform == RuntimePlatform.OSXPlayer;

            /*
            var isPointerOverGameObject = (isEditorOrStandalone && EventSystem.current.IsPointerOverGameObject()) || 
                                          (Input.touchCount > 0
                                              && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
                                              && Input.touches[0].phase == TouchPhase.Began);
                                              */

            var isPointerOverGameObject = true;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if ((touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) &&
                    !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    isPointerOverGameObject = false;
                }
            }
            else
            {
                return;
            }

            if (m_SelectedPropItemController != null)
            {
                if (m_MapSession != null && !isPointerOverGameObject && Input.touchCount > 0)
                {
                    var point = m_MapSession.HitTestOne(new Vector2(Input.touches[0].position.x / Screen.width,
                        Input.touches[0].position.y / Screen.height));
                    if (point.OnSome)
                    {
                        StopEditGameObject();
                        CreateGameObject(m_SelectedPropItemController, point.Value);
                        StartEditGameObject(m_SelectedGameObject);
                    }

                    else
                    {
                        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hitInfo;
                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            if (hitInfo.transform.GetComponent<PlaneShadowController>())
                            {
                                StopEditGameObject();
                                CreateGameObject(m_SelectedPropItemController, hitInfo.point);
                                StartEditGameObject(m_SelectedGameObject);
                            }
                        }
                    }
                }
            }

            if (m_SelectedGameObject == null && m_SelectedPropItemController == null)
            {
                if (((isEditorOrStandalone && Input.GetMouseButtonDown(0)) ||
                     (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began &&
                      !isPointerOverGameObject)))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        StopEditGameObject();
                        StartEditGameObject(hitInfo.collider.gameObject);
                    }
                }
            }
            else
            {
                if (m_MapSession != null && !isPointerOverGameObject && Input.touchCount > 0)
                {
                    if (!m_IsMoveFree)
                    {
                        var point = m_MapSession.HitTestOne(new Vector2(Input.touches[0].position.x / Screen.width,
                            Input.touches[0].position.y / Screen.height));
                        if (point.OnSome)
                        {
                            if (m_UseCenterPosition)
                            {
                                m_SelectedGameObject.transform.position = point.Value;
                            }
                            else
                            {
                                m_SelectedGameObject.transform.position = point.Value +
                                                                          Vector3.up * m_SelectedGameObject.transform
                                                                              .localScale.y / 2;
                            }
                        }
                    }
                }
            }
        }

        private void OnDisable()
        {
            m_MapSession = null;
            StopEditGameObject();
        }

        public void SetMapSession(MapSession session)
        {
            m_MapSession = session;
            if (m_MapSession.MapWorker)
            {
                m_MapSession.MapWorker.MapLoad += (arg1, arg2, arg3, arg4) =>
                {
                    //TODO
                };
            }
        }

        public void SetFreeMove(bool isFree)
        {
            m_IsMoveFree = isFree;
            if (m_SelectedGameObject)
            {
                if (isFree)
                {
                    m_TouchControl.TurnOn(m_SelectedGameObject.transform, Camera.main, true, true, true, true);
                }
                else
                {
                    m_TouchControl.TurnOn(m_SelectedGameObject.transform, Camera.main, false, false, true, true);
                }
            }
        }

        public void SelectTemplate(PropItemController propItemController)
        {
            m_SelectedPropItemController = propItemController;
            StopEditGameObject();
        }

        public void DeselectTemplate()
        {
            PropGridController.Deselect();
            m_SelectedPropItemController = null;

            /*
            StopEditButton.SetActive(false);
            DeleteButton.SetActive(false);
            */
        }

        public void CreateGameObject(PropItemController controller, Vector3 initPosition)
        {
            m_SelectedGameObject = Instantiate(controller.Templet.Object);
            m_SelectedGameObject.name = controller.Templet.Object.name;
            if (m_UseCenterPosition)
            {
                m_SelectedGameObject.transform.position = initPosition;
            }
            else
            {
                m_SelectedGameObject.transform.position =
                    initPosition + Vector3.up * m_SelectedGameObject.transform.localScale.y / 2;
            }

            if (CreateObjectEvent != null)
            {
                CreateObjectEvent(m_SelectedGameObject);
            }

            FreeMoveSwitch.gameObject.SetActive(true);
            DeselectTemplate();
        }

        public void StartEditGameObject(GameObject obj)
        {
            DeselectTemplate();

            StopEditButton.SetActive(true);
            DeleteButton.SetActive(true);

            m_SelectedGameObject = obj;

            var meshFilter = m_SelectedGameObject.GetComponentInChildren<MeshFilter>();

            BoxCollider boxCollider = m_SelectedGameObject.GetComponent<BoxCollider>();

            if (boxCollider != null)
            {
                OutlinePrefab.SetActive(true);
                //OutlinePrefab.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
                OutlinePrefab.transform.parent = m_SelectedGameObject.transform;
                //OutlinePrefab.transform.localPosition = Vector3.zero;
                OutlinePrefab.transform.localPosition = boxCollider.center;
                OutlinePrefab.transform.localRotation = Quaternion.identity;
                OutlinePrefab.transform.localScale = boxCollider.size;
            }
            else
            {
                return;
                /*
                OutlinePrefab.SetActive(true);
                OutlinePrefab.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
                OutlinePrefab.transform.parent = meshFilter.transform;
                OutlinePrefab.transform.localPosition = Vector3.zero;
                OutlinePrefab.transform.localRotation = Quaternion.identity;
                OutlinePrefab.transform.localScale = Vector3.one;
                */
            }

            SetFreeMove(m_IsMoveFree);
        }

        public void StopEditGameObject()
        {
            StopEditButton.SetActive(false);
            DeleteButton.SetActive(false);

            DragonAddPointButton.gameObject.SetActive(false);

            m_SelectedGameObject = null;
            if (OutlinePrefab)
            {
                OutlinePrefab.transform.parent = null;
                OutlinePrefab.SetActive(false);
            }

            if (m_TouchControl)
            {
                m_TouchControl.TurnOff();
            }
        }

        public void DeleteSelection()
        {
            if (!m_SelectedGameObject)
            {
                return;
            }

            if (DeleteObjectEvent != null)
            {
                DeleteObjectEvent(m_SelectedGameObject);
            }

            Destroy(m_SelectedGameObject);
            StopEditGameObject();
        }
    }
}