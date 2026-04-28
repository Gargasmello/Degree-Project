using NUnit.Framework;
using Pointo.Unit;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditor.SpeedTree.Importer;
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

public enum GameBalance
{
    AIDOMINANT,
    EVEN,
    PLAYERDOMINANT
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

    public AiMode state;

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

    private GameBalance EvaluateGameBalance()
    {
        GameBalance balance = new();

        int balanceScore = 0;

        if (Gamemanager.instance.playerTroops.Count >= Gamemanager.instance.aiTroops.Count) balanceScore += Gamemanager.instance.playerTroops.Count - Gamemanager.instance.aiTroops.Count;
        else if (Gamemanager.instance.aiTroops.Count >= Gamemanager.instance.playerTroops.Count) balanceScore -= Gamemanager.instance.aiTroops.Count - Gamemanager.instance.playerTroops.Count;

        int aiFlags = 0, playerFlags = 0;
        foreach (var tile in GridManager.instance.flagTiles)
        {
            if (tile.GetComponent<FlagTile>().playerControl) playerFlags += 1;
            else if (tile.GetComponent<FlagTile>().aiControl) aiFlags += 1;
        }

        if (aiFlags > playerFlags) balanceScore -= aiFlags - playerFlags;
        else if (playerFlags > aiFlags) balanceScore += playerFlags - aiFlags;

        if (balanceScore > 4) balance = GameBalance.PLAYERDOMINANT;
        else if (balanceScore < -4) balance = GameBalance.AIDOMINANT;
        else balance = GameBalance.EVEN;

        return balance;
    }

    public void UpdateAiState()
    {
        GameBalance balance = EvaluateGameBalance();
        if (balance == GameBalance.PLAYERDOMINANT)
        {
            state = AiMode.GATHERING;
        }
        else if (balance == GameBalance.AIDOMINANT)
        {
            state = AiMode.DEFEND;
        }
        else if (balance == GameBalance.EVEN)
        {

        }
    }

    public void MoveUnits()
    {
        if (state == AiMode.ATTACK)
        {
            foreach (var unit in Gamemanager.instance.aiTroops)
            {
                unit.GetComponent<Unit>().MoveTowardEnemy();
            }
        }
        else if (state == AiMode.GATHERING)
        {
            foreach (var unit in Gamemanager.instance.aiTroops)
            {
                unit.GetComponent<Unit>().MoveTowardFlag();
            }
        }
        else if (state != AiMode.DEFEND)
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

    public void AttackWave()
    {

    }
}
