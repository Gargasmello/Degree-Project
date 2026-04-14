using Pointo.Unit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    [SerializeField] private int width, height;

    [SerializeField] private Tile grassTile, mountainTile;

    [SerializeField] private Transform cam;

    [SerializeField] public Grid grid;

    private Dictionary<Vector2, Tile> tiles;

    public List<GameObject> tilesList;
    public List<GameObject> flagTiles;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        tilesList = GameObject.FindGameObjectsWithTag("Tile").ToList();
        foreach (var tile in tilesList) {
            if (tile.GetComponent<FlagTile>() != null)
            {
                flagTiles.Add(tile);
            }
        }
    }

    public Tile GetTileFromPosition(Vector2 position)
    {
        if (tiles.TryGetValue(position, out var tile))
        {
            return tile;
        }

        return null;
    }
}
