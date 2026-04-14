using NUnit.Framework;
using Pointo.Unit;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;
using Unit = Pointo.Unit.Unit;

public enum AiMode
{
    ATTACK,
    DEFEND,
    GATHERING,
    INACTIVE
}

public class AiManager : MonoBehaviour
{
    public static AiManager Instance;

    public float resources;
    private float meleeCost = 30, rangedCost = 60, artilleryCost = 150;
    public GameObject spawnUnit;
    private GameObject spawnTile;
    [SerializeField] private List<GameObject> units;
    public List<GameObject> firstColumnTiles;

    public AiMode aiMode;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnUnit = units[Random.Range(0, units.Count)];

        foreach (var tile in GridManager.instance.tilesList)
        {
            if (tile.transform.position.x == 0.5)
            {
                firstColumnTiles.Add(tile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnUnit()
    {
        float cost = 0;
        if (spawnUnit.GetComponent<Unit>().unitSo.unitType == UnitType.Knight)
        {
            cost = meleeCost;
        }
        else if (spawnUnit.GetComponent<Unit>().unitSo.unitType == UnitType.Archer)
        {
            cost = rangedCost;
        }
        else if (spawnUnit.GetComponent<Unit>().unitSo.unitType == UnitType.Catapult)
        {
            cost = artilleryCost;
        }

        if (resources > cost)
        {
            resources -= cost;
            spawnTile = firstColumnTiles[Random.Range(0, firstColumnTiles.Count)];
            var spawnedUnit = Instantiate(spawnUnit);
            spawnedUnit.transform.position = spawnTile.transform.position;
            spawnTile.GetComponent<Tile>().SetUnit(spawnedUnit.GetComponent<Unit>());
            spawnUnit = units[Random.Range(0, units.Count)];
        }
    }

    //TODO: Should give points
    // In range of attack, In range of movement,  
    public void EvaluateTiles(List<GameObject> tiles)
    {
        foreach (var tile in tiles)
        {
            tile.GetComponent<Tile>().TileInRangeOfPlayerUnit();
            tile.GetComponent<Tile>().FlagTilePoints();
        }
    }

    public void MoveUnits()
    {
        if (aiMode == AiMode.ATTACK)
        {
            foreach (var unit in Gamemanager.instance.aiTroops)
            {
                unit.GetComponent<Unit>().MoveTowardEnemy();
            }
        }
        else if (aiMode == AiMode.GATHERING)
        {
            foreach (var unit in Gamemanager.instance.aiTroops)
            {
                unit.GetComponent<Unit>().MoveTowardFlag();
            }
        }
        else if (aiMode != AiMode.DEFEND)
        {

        }
    }

    public void AttackEnemies()
    {
        foreach (var unit in Gamemanager.instance.aiTroops)
        {
            foreach (var enemy in Gamemanager.instance.playerTroops)
            {
                unit.GetComponent<Unit>().Attack(enemy.GetComponent<Unit>());
            }
        }
    }
}
