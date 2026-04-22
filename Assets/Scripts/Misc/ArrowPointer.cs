using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public static ArrowPointer instance;

    [Header("Arrow Settings")]
    public Material arrowMaterial;
    public float arrowWidth = 0.15f;
    public Color arrowColor;
    public float segmentLength = 0.4f; // length of each body segment
    public GameObject arrowheadPrefab; // optional: a sprite/mesh for the tip

    private LineRenderer lineRenderer;
    private GameObject arrowhead;

    void Awake()
    {
        instance = this;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = arrowMaterial;
        lineRenderer.startColor = arrowColor;
        lineRenderer.endColor = arrowColor;
        lineRenderer.startWidth = arrowWidth;
        lineRenderer.endWidth = arrowWidth * 0.3f; // taper toward tip
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
        lineRenderer.sortingOrder = 5;

        if (arrowheadPrefab != null)
        {
            arrowhead = Instantiate(arrowheadPrefab);
            arrowhead.SetActive(false);
        }
    }

    public void ShowArrow(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        float distance = Vector3.Distance(from, to);

        // Build segmented body points
        int segments = Mathf.Max(2, Mathf.CeilToInt(distance / segmentLength));
        lineRenderer.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            lineRenderer.SetPosition(i, Vector3.Lerp(from, to, t));
        }

        lineRenderer.enabled = true;

        // Position arrowhead at destination
        if (arrowhead != null)
        {
            arrowhead.transform.position = to;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowhead.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            arrowhead.SetActive(true);
        }
    }

    public void HideArrow()
    {
        lineRenderer.enabled = false;
        if (arrowhead != null)
            arrowhead.SetActive(false);
    }
}
