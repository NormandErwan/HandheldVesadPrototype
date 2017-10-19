using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.Task
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(ContentSizeFitter))]
    public class ContainersLayout : MonoBehaviour
    {
        // Editor fields

        public Camera mainCamera;
        public Vector2Int mobileDeviceSize;
        public float scale = 0.0001f;
        public GridLayoutGroup grid;
        public Vector2Int gridSize = new Vector2Int(8, 4);
        public int containerMargins = 1;
        public GameObject containerPrefab;

        // Variables

        private RectTransform rect;
        private ContentSizeFitter sizeFitter;

        // Methods

        protected void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        protected void Start()
        {
            SetupContainers();
            StartCoroutine(SetupCamera());
        }

        protected void SetupContainers()
        {
            rect.localScale = scale * Vector3.one;

            grid.padding = new RectOffset(containerMargins, containerMargins, containerMargins, containerMargins);
            grid.spacing = containerMargins * Vector2.one;
            grid.cellSize = mobileDeviceSize;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = gridSize.x;

            foreach (Transform container in grid.transform)
            {
                Destroy(container.gameObject);
            }

            int containerNumber = gridSize.x * gridSize.y;
            for (int i = 0; i < containerNumber; i++)
            {
                Instantiate(containerPrefab, grid.transform);
            }
        }

        protected IEnumerator SetupCamera()
        {
            yield return null;

#if UNITY_ANDROID
            // TODO: if in phone only condition
            if (mobileDeviceSize.x > 0 && mobileDeviceSize.y > 0)
            {
                mainCamera.aspect = mobileDeviceSize.x / (float)mobileDeviceSize.y;
                float maxSideLength = Mathf.Max(rect.rect.width / mainCamera.aspect, rect.rect.height);
                float distance = -maxSideLength * scale / (2 * Mathf.Tan(mainCamera.fieldOfView / 2 * Mathf.Deg2Rad));
                mainCamera.transform.localPosition = new Vector3(rect.transform.localPosition.x, rect.transform.localPosition.y, distance);
            }
#endif
        }
    }
}
