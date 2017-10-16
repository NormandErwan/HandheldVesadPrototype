using UnityEngine;

public class ContainersLayout : MonoBehaviour
{
    // Constants

    public const float scale = 0.0005f;

    // Editor fields

    public Camera mainCamera;
    public Canvas canvas;
    public GridLayout grid;
    public Vector2 gridSize = new Vector2(8, 4);
    public GameObject containerPrefab;

    // Variables

    private RectTransform rect;

    // Methods

    protected void Start()
    {
        SetupLayoutSize();


    }

    protected void OnValidate()
    {
        SetupLayoutSize();
    }

    protected void SetupLayoutSize()
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

            rect.localScale = scale * Vector3.one;

            rect.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, rect.localPosition.z);
            float distance = Mathf.Abs(rect.localPosition.z - mainCamera.transform.localPosition.z);

            float heigth = 2 * distance * Mathf.Tan(mainCamera.fieldOfView / 2 * Mathf.Deg2Rad) / scale;
            float width = heigth * mainCamera.aspect;

            rect.sizeDelta = new Vector2(width, heigth);
        }
    }
}
