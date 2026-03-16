using System.Diagnostics;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private GameObject highlight;

    public void Init(bool isOffset)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = isOffset ? offsetColor : baseColor;
    }

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}
