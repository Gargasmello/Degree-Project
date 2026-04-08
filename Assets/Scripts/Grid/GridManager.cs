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

    [SerializeField] private Grid grid;

    private Dictionary<Vector2, Tile> tiles;

    public List<GameObject> tilesList;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        tilesList = GameObject.FindGameObjectsWithTag("Tile").ToList();
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
