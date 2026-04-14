using UnityEditor;
using UnityEngine;

public class GrassTile : Tile
{
    [SerializeField] private Color baseColor, offsetColor;

    public override void Init(int x, int y)
    {
        var isOffset = (x + y) % 2 == 1;
        GetComponent<SpriteRenderer>().color = isOffset ? offsetColor : baseColor;
    }

    private void OnValidate()
    {
        name = $"Tile {transform.position.x} {transform.position.y}";
    }

    private new void Start()
    {
        Init((int)transform.position.x, (int)transform.position.y);
        inRangeIcon = transform.Find("InRangeOfIcon").gameObject;
    }
}
