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
}
