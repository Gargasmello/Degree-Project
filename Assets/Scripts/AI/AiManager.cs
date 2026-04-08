using NUnit.Framework;
using Pointo.Unit;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class AiManager : MonoBehaviour
{
    public static AiManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO: Should give points
    // In range of attack, In range of movement,  
    public void EvaluateTiles(List<GameObject> tiles)
    {
        foreach (var tile in tiles)
        {
            tile.GetComponent<Tile>().TileInRangeOfPlayerUnit(tile);
        }
    }
}
