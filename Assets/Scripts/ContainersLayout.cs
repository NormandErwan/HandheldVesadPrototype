using UnityEngine;
using UnityEngine.UI;

public class ContainersLayout : MonoBehaviour
{
    // Constants

    public const float scale = 0.0005f;

    // Editor fields

    public Camera mainCamera;
    public Canvas canvas;
    public GridLayoutGroup grid;
    public Vector2Int gridSize = new Vector2Int(8, 4);
    public int containerMargins = 1;
    public GameObject containerPrefab;

    // Variables

    private RectTransform rect;

    // Methods

    protected void Start()
    {
        SetupLayout();
        SetupContainers();
    }

    protected void OnValidate()
    {
        SetupLayout();
        SetupContainers();
    }

    protected void SetupLayout()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }

        if (mainCamera != null && canvas != null)
        {
            if (rect == null)
            {
                rect = canvas.gameObject.GetComponent<RectTransform>();
            }

            canvas.renderMode = RenderMode.WorldSpace;
            rect.localScale = scale * Vector3.one;
            rect.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, rect.localPosition.z);

            float distance = Mathf.Abs(rect.localPosition.z - mainCamera.transform.localPosition.z);
            float heigth = 2 * distance * Mathf.Tan(mainCamera.fieldOfView / 2 * Mathf.Deg2Rad) / scale;
            float width = heigth * mainCamera.aspect;
            rect.sizeDelta = new Vector2(width, heigth);
        }
    }

    protected void SetupContainers()
    {
        if (grid != null)
        {
            grid.padding = new RectOffset(containerMargins, containerMargins, containerMargins, containerMargins);
            grid.spacing = containerMargins * Vector2.one;

            Vector2 gridSizeInverse = new Vector2(1f / gridSize.x, 1f / gridSize.y);
            /*float cellWidth = rect.sizeDelta.x / gridSize.x - (1 + 1 / (float)gridSize.x) * containerMargins;
            float cellHeigth = rect.sizeDelta.y / gridSize.y - (1 + 1 / (float)gridSize.y ) * containerMargins;
            grid.cellSize = new Vector2(cellWidth, cellHeigth);*/
            grid.cellSize = Vector2.Scale(rect.sizeDelta, gridSizeInverse) - (Vector2.one + gridSizeInverse) * containerMargins;

            int containerNumber = (int)gridSize.x * (int)gridSize.y;
            if (containerNumber != grid.transform.childCount)
            {
                foreach (Transform container in grid.transform)
                {
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
                }

                for (int i = 0; i < containerNumber; i++)
                {
                    Instantiate(containerPrefab, grid.transform);
                }
            }
        }
    }
}
