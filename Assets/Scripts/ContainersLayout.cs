using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(ContentSizeFitter))]
    public class ContainersLayout : MonoBehaviour
    {
        // Editor fields

        public Camera mainCamera;
        public Vector2Int deviceSize;
        public float scale = 0.0001f;
        public GridLayoutGroup grid;
        public Vector2Int gridSize = new Vector2Int(8, 4);
        public int containerMargins = 1;
        public GameObject containerPrefab;

        // Variables

        private Canvas canvas;
        private RectTransform rect;
        private ContentSizeFitter sizeFitter;

        // Methods

        protected void Start()
        {
            SetupContainers();
            StartCoroutine(SetupCameraCoroutine());
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            SetupContainers();
            UnityEditor.EditorApplication.delayCall += () =>
            {
                UnityEditor.EditorApplication.delayCall += () => // We must wait the first delay call in SetupContainers to finish
                {
                    SetupCamera();
                };
            };
        }
#endif

        protected void SetupContainers()
        {
            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
                rect = canvas.gameObject.GetComponent<RectTransform>();
            }
            canvas.renderMode = RenderMode.WorldSpace;
            rect.localScale = scale * Vector3.one;

            if (grid != null)
            {
                GetComponent<ContentSizeFitter>().enabled = true;

                grid.padding = new RectOffset(containerMargins, containerMargins, containerMargins, containerMargins);
                grid.spacing = containerMargins * Vector2.one;
                grid.cellSize = deviceSize;
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = gridSize.x;

                int containerNumber = gridSize.x * gridSize.y;
                if (containerNumber != grid.transform.childCount)
                {
                    foreach (Transform container in grid.transform)
                    {
#if UNITY_EDITOR
                        if (Application.isPlaying)
                        {
                            Destroy(container.gameObject);
                        }
                        else
                        {
                            UnityEditor.EditorApplication.delayCall += () =>
                            {
                                DestroyImmediate(container.gameObject);
                            };
                        }
#else
                    Destroy(container.gameObject);
#endif
                    }

                    for (int i = 0; i < containerNumber; i++)
                    {
                        Instantiate(containerPrefab, grid.transform);
                    }
                }

                if (gridSize.x == 0 || gridSize.y == 0)
                {
                    sizeFitter.enabled = false;
                    rect.sizeDelta = deviceSize;
                }
            }
        }

        protected IEnumerator SetupCameraCoroutine()
        {
            yield return null;
            SetupCamera();
        }

        protected void SetupCamera()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

#if UNITY_ANDROID
            // TODO: if in phone only condition
            if (mainCamera != null && deviceSize.x > 0 && deviceSize.y > 0)
            {
                mainCamera.aspect = deviceSize.x / (float)deviceSize.y;
                float maxSideLength = Mathf.Max(rect.rect.width / mainCamera.aspect, rect.rect.height);
                float distance = -maxSideLength * scale / (2 * Mathf.Tan(mainCamera.fieldOfView / 2 * Mathf.Deg2Rad));
                mainCamera.transform.localPosition = new Vector3(rect.transform.localPosition.x, rect.transform.localPosition.y, distance);
            }
#endif

            // Hacky, but prevent bugs in the Unity Editor caused by ContentSizeFitter in prefabs
            Vector2 rectSize = rect.sizeDelta;
            GetComponent<ContentSizeFitter>().enabled = false;
            rect.sizeDelta = rectSize;
        }
    }
}
