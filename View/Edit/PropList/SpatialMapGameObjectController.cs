using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using AllArt.Solana.Nft;
using easyar;
using Michsky.UI.ModernUIPack;
using Rarible;
using RTG;
using SpatialMap_SparseSpatialMap;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZBoom.Common.SpatialMap
{
    public class SpatialMapGameObjectController : MonoBehaviour
    {
        public PropGridController PropGridController;
        public PropController PropController;
        public GameObject PropCollectionGameObject;
        public PrefabGridController PrefabGridController;
        public GameObject SolanaNftCollectionGameObject;
        public SolanaNftGridController SolanaGridController;
        public SolanaNftOwnedGridController SolanaOwnedGridController;

        public GameObject OutlinePrefab;

        public SwitchManager FreeMoveSwitch;
        public SwitchManager VideoSwitch;
        public SwitchManager CloudPointSwitch;
        public SwitchManager GizmoSwitch;

        public GameObject AddPanel;
        public GameObject TapPanel;
        public GameObject EditPanel;
        public GameObject GizmoPanel;
        public GameObject RotateScalePanel;

        public SliderManager RotateSlider;
        public SliderManager ScaleSlider;

        public GameObject StopEditButton;
        public GameObject DeleteButton;
        //public GameObject DragonAddPointButton;

        private PropItemController m_SelectedPropItemController;

        //private BaseRaribleItemController m_SelectedRaribleItemController;
        private RaribleItem m_SelectedRaribleItem;
        private Nft m_SelectedSolanaNft;

        [SerializeField] private TouchController m_TouchController;
        [SerializeField] private GizmoController m_GizmoController;
        [SerializeField] private RaribleContentCreator m_RaribleContentCreator;
        [SerializeField] private SolanaNftContentCreator m_SolanaNftContentCreator;
        private MapSession m_MapSession;
        private GameObject m_SelectedGameObject;
        private bool m_IsNft = false;
        private bool m_IsMoveFree = true;
        private bool m_UseGizmo = false;
        [SerializeField] private bool m_UseCenterPosition = true;
        [SerializeField] private bool m_UseAutoScale = true;
        [SerializeField] private float m_ScaleStep = 0.25f;

        public event Action<GameObject> CreateObjectEvent;
        public event Action<GameObject> DeleteObjectEvent;

        private List<GameObject> m_ArObjects = new List<GameObject>();

        private void Awake()
        {
            m_SelectedPropItemController = null;
            m_SelectedRaribleItem = null;
            m_SelectedSolanaNft = null;

            //m_TouchController = GetComponentInChildren<TouchController>(true);
            //m_GizmoController = GetComponentInChildren<GizmoController>(true);

            m_TouchController.ScaleAction += localScale =>
            {
                if (m_SelectedGameObject != null)
                {
                }
            };

            OutlinePrefab = Instantiate(OutlinePrefab);
            OutlinePrefab.SetActive(false);

            StopEditButton.SetActive(false);
            //DragonAddPointButton.SetActive(false);
            DeleteButton.SetActive(false);

            AddPanel.SetActive(true);
            TapPanel.SetActive(false);
            EditPanel.SetActive(false);

            RotateScalePanel.SetActive(false);
            GizmoPanel.SetActive(false);

            //VideoSwitch.isOn = false;
            CloudPointSwitch.isOn = true;
            FreeMoveSwitch.isOn = true;
            GizmoSwitch.isOn = false;

            m_IsMoveFree = FreeMoveSwitch.isOn;
            m_UseGizmo = GizmoSwitch.isOn;

            if (m_UseGizmo)
            {
                m_TouchController.enabled = false;
                m_GizmoController.enabled = true;
            }
            else
            {
                m_TouchController.enabled = true;
                m_GizmoController.enabled = false;
            }
        }

        private void Update()
        {
            //transform.position = Input.mousePosition;

            bool isPointerOverGameObject = true;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if ((touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) &&
                    !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    isPointerOverGameObject = false;
                }
            }

            if (isPointerOverGameObject || m_MapSession == null)
            {
                return;
            }

            if (m_SelectedPropItemController != null)
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
                    //TODO

                    Debug.Log("ViewManager: 5 !point.OnSome");

                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.transform.GetComponent<DenseSpatialMapBlockController>())
                        {
                            StopEditGameObject();
                            CreateGameObject(m_SelectedPropItemController, hitInfo.point);
                            StartEditGameObject(m_SelectedGameObject);
                        }
                        /*
                        if (hitInfo.transform.GetComponent<PlaneShadowController>())
                        {
                            StopEditGameObject();
                            CreateGameObject(m_SelectedPropItemController, hitInfo.point);
                            StartEditGameObject(m_SelectedGameObject);
                        }
                        */
                    }
                }
            }

            if (m_SelectedRaribleItem != null)
            {
                var point = m_MapSession.HitTestOne(new Vector2(Input.touches[0].position.x / Screen.width,
                    Input.touches[0].position.y / Screen.height));

                if (point.OnSome)
                {
                    StopEditGameObject();
                    CreateNftGameObject(m_SelectedRaribleItem, point.Value);
                    StartEditGameObject(m_SelectedGameObject);
                }
                else
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.transform.GetComponent<DenseSpatialMapBlockController>())
                        {
                            StopEditGameObject();
                            CreateNftGameObject(m_SelectedRaribleItem, hitInfo.point);
                            StartEditGameObject(m_SelectedGameObject);
                        }
                    }
                }
            }


            if (m_SelectedSolanaNft != null)
            {
                var point = m_MapSession.HitTestOne(new Vector2(Input.touches[0].position.x / Screen.width,
                    Input.touches[0].position.y / Screen.height));

                if (point.OnSome)
                {
                    StopEditGameObject();
                    CreateSolanaNftGameObject(m_SelectedSolanaNft, point.Value);
                    StartEditGameObject(m_SelectedGameObject);
                }
                else
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.transform.GetComponent<DenseSpatialMapBlockController>())
                        {
                            StopEditGameObject();
                            CreateSolanaNftGameObject(m_SelectedSolanaNft, hitInfo.point);
                            StartEditGameObject(m_SelectedGameObject);
                        }
                    }
                }
            }

            //if (m_SelectedGameObject == null && m_SelectedPropItemController == null)
            if (m_SelectedGameObject == null)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    StopEditGameObject();
                    StartEditGameObject(hitInfo.collider.gameObject);
                }
            }
            else
            {
                if (!m_IsMoveFree)
                {
                    var point = m_MapSession.HitTestOne(new Vector2(Input.touches[0].position.x / Screen.width,
                        Input.touches[0].position.y / Screen.height));
                    if (point.OnSome)
                    {
                        Debug.Log("ViewManager: 4 !m_IsMoveFree");

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
                    else
                    {
                        Debug.Log("ViewManager: 6 !point.OnSome");
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
                    m_TouchController.TurnOn(m_SelectedGameObject.transform, Camera.main, true, true, true, true);
                }
                else
                {
                    m_TouchController.TurnOn(m_SelectedGameObject.transform, Camera.main, false, false, true, true);
                }
            }
        }

        public void SelectNftTemplate(RaribleItem raribleItem)
        {
            m_SelectedSolanaNft = null;
            m_SelectedPropItemController = null;
            m_SelectedRaribleItem = raribleItem;

            SelectGameObject();

            //CreateNftGameObject(m_SelectedRaribleItem, Vector3.zero);
        }

        public void SelectSolanaNftTemplate(Nft solanaNft)
        {
            m_SelectedPropItemController = null;
            m_SelectedRaribleItem = null;
            m_SelectedSolanaNft = solanaNft;

            SelectGameObject();

            //CreateSolanaNftGameObject(m_SelectedSolanaNft, Vector3.zero);
        }

        public void SelectTemplate(PropItemController propItemController)
        {
            m_SelectedSolanaNft = null;
            m_SelectedRaribleItem = null;
            m_SelectedPropItemController = propItemController;


            SelectGameObject();
        }

        private void SelectGameObject()
        {
            StopEditGameObject();

            RotateScalePanel.SetActive(false);
            GizmoPanel.SetActive(false);

            PropCollectionGameObject.SetActive(false);
            SolanaNftCollectionGameObject.SetActive(false);
            
            AddPanel.SetActive(false);
            TapPanel.SetActive(true);
            EditPanel.SetActive(false);
        }

        public void CreateGameObject(PropItemController controller, Vector3 initPosition)
        {
            m_IsNft = false;

            m_SelectedGameObject = Instantiate(controller.Templet.Object);
            m_SelectedGameObject.name = controller.Templet.Object.name;

            Create(m_SelectedGameObject, initPosition);
        }

        public void CreateNftGameObject(RaribleItem raribleItem, Vector3 initPosition)
        {
            m_IsNft = true;

            BaseRaribleItemController baseRaribleItemController =
                m_RaribleContentCreator.GetRaribleItemPrefab(raribleItem);
            if (baseRaribleItemController != null)
            {
                m_SelectedGameObject = baseRaribleItemController.gameObject;
                //
                baseRaribleItemController.Create();

                Create(m_SelectedGameObject, initPosition);
            }
        }

        public void CreateSolanaNftGameObject(Nft solanaNft, Vector3 initPosition)
        {
            m_IsNft = true;
            BaseSolanaNftItemController baseNftItemController = m_SolanaNftContentCreator.GetNftPrefab(solanaNft);
            if (baseNftItemController != null)
            {
                m_SelectedGameObject = baseNftItemController.gameObject;
                baseNftItemController.Create();
                Create(m_SelectedGameObject, initPosition);
            }
        }

        private void Create(GameObject selectedGameObject, Vector3 initPosition)
        {
            float localScale = 1f;
            if (m_UseAutoScale)
            {
                float distance = Vector3.Distance(Camera.main.transform.position, initPosition);
                localScale = distance * m_ScaleStep;
                if (localScale >= 1)
                {
                    localScale = 1f;
                }
            }

            selectedGameObject.transform.localScale = new Vector3(localScale, localScale, localScale);

            if (m_UseCenterPosition)
            {
                selectedGameObject.transform.position = initPosition;
            }
            else
            {
                selectedGameObject.transform.position =
                    initPosition + Vector3.up * selectedGameObject.transform.localScale.y / 2;
            }

            if (CreateObjectEvent != null)
            {
                CreateObjectEvent(selectedGameObject);
            }

            ObjectAR objectAR = selectedGameObject.GetComponent<ObjectAR>();
            if (objectAR != null)
            {
                objectAR.SetState(ObjectAR.State.EDIT);
            }

            FreeMoveSwitch.gameObject.SetActive(true);
            DeselectTemplate();
        }

        public void DeselectTemplate()
        {
            Debug.LogWarning("SpatialMapGameObjectController DeselectTemplate " + "1");

            if (PropGridController != null)
            {
                PropGridController.Deselect();
            }

            if (PrefabGridController != null)
            {
                PrefabGridController.Deselect();
            }

            if (SolanaGridController != null)
            {
                SolanaGridController.Deselect();
            }
            
            if (SolanaOwnedGridController != null)
            {
                SolanaOwnedGridController.Deselect();
            }

            Debug.LogWarning("SpatialMapGameObjectController DeselectTemplate " + "2");


            if (PropGridController != null)
            {
                PropController.Deselect();
            }

            Debug.LogWarning("SpatialMapGameObjectController DeselectTemplate " + "3");

            m_SelectedPropItemController = null;
            m_SelectedRaribleItem = null;
            m_SelectedSolanaNft = null;

            Debug.LogWarning("SpatialMapGameObjectController DeselectTemplate " + "4");


            //TODO под вопросом???
            //Возможен баг
            AddPanel.SetActive(true);
            TapPanel.SetActive(false);
            EditPanel.SetActive(false);
            RotateScalePanel.SetActive(false);
            GizmoPanel.SetActive(false);
            Debug.LogWarning("SpatialMapGameObjectController DeselectTemplate " + "5");

            /*
            StopEditButton.SetActive(false);
            DeleteButton.SetActive(false);
            */
        }

        public void StartEditGameObject(GameObject obj)
        {
            if (obj.transform.GetComponent<ObjectAR>() == null)
            {
                return;
            }

            DeselectTemplate();

            if (!GizmoSwitch.isOn)
            {
                RotateScalePanel.SetActive(true);
            }
            else
            {
                GizmoPanel.SetActive(true);
            }

            Debug.LogWarning("SpatialMapGameObjectController " + "StartEditGameObject 1");

            RotateSlider.mainSlider.value = obj.transform.localRotation.eulerAngles.y;
            //RotateSlider.UpdateUI();
            ScaleSlider.mainSlider.value = obj.transform.localScale.x;
            //ScaleSlider.UpdateUI();
            Debug.LogWarning("SpatialMapGameObjectController " + "StartEditGameObject 2");

            AddPanel.SetActive(false);
            TapPanel.SetActive(false);
            EditPanel.SetActive(true);

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

            if (!m_UseGizmo)
            {
                m_TouchController.TurnOn(m_SelectedGameObject.transform, Camera.main, true, true, true, true);
            }
            else
            {
                m_GizmoController.OnTargetObjectChanged(m_SelectedGameObject);
            }

            SetFreeMove(m_IsMoveFree);
        }

        public void StopEditGameObject()
        {
            RotateScalePanel.SetActive(false);
            GizmoPanel.SetActive(false);

            AddPanel.SetActive(true);
            TapPanel.SetActive(false);
            EditPanel.SetActive(false);

            //DragonAddPointButton.gameObject.SetActive(false);

            m_SelectedGameObject = null;

            if (OutlinePrefab)
            {
                OutlinePrefab.transform.parent = null;
                OutlinePrefab.SetActive(false);
            }

            if (!m_UseGizmo)
            {
                if (m_TouchController != null)
                {
                    m_TouchController.TurnOff();
                }
            }
            else
            {
                m_GizmoController.OnTargetObjectChanged(null);
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

        public void CopySelection()
        {
            if (m_SelectedGameObject != null)
            {
                if (OutlinePrefab)
                {
                    OutlinePrefab.transform.parent = null;
                    OutlinePrefab.SetActive(false);
                }

                var copySelectedGameObject = Instantiate(m_SelectedGameObject);
                copySelectedGameObject.transform.parent = copySelectedGameObject.transform.parent;
                
                copySelectedGameObject.transform.position = m_SelectedGameObject.transform.position;
                copySelectedGameObject.transform.rotation = m_SelectedGameObject.transform.rotation;
                copySelectedGameObject.transform.localScale = m_SelectedGameObject.transform.localScale;

                m_ArObjects.Add(copySelectedGameObject);

                StopEditGameObject();
                StartEditGameObject(copySelectedGameObject);
            }
        }

        public void UseGizmo(bool useGizmo)
        {
            StopEditGameObject();

            m_UseGizmo = useGizmo;
            if (!useGizmo)
            {
                m_GizmoController.enabled = false;
                m_TouchController.enabled = true;
            }
            else
            {
                m_GizmoController.enabled = true;
                m_TouchController.enabled = false;
            }
        }

        public void Rotate(float rotation)
        {
            if (m_SelectedGameObject != null)
            {
                m_SelectedGameObject.transform.localRotation = Quaternion.Euler(0f, rotation, 0f);
            }
        }

        public void Scale(float scale)
        {
            if (m_SelectedGameObject != null)
            {
                m_SelectedGameObject.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}