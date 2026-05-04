using Assets.Scripts.AI;
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

    List<List<GameObject>> columns;

    private void Awake()
    {
        Instance = this;
    }

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
        GridManager.instance.TilePointsTowardRightSide();
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
            int random = Random.Range(0, 4);
            switch (random)
            {
                case 0:
                    state = AiMode.GATHERING;
                    break;
                case 1:
                    state = AiMode.DEFEND; 
                    break;
                case 2:
                    state = AiMode.ATTACK;
                    break;
                case 3:
                    state = AiMode.ATTACK;
                    break;
                case 4:
                    state = AiMode.ATTACK;
                    break;
            }
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
            foreach (var unit in Gamemanager.instance.aiTroops)
            {
                unit.GetComponent<Unit>().MoveTowardFlagDefence();
            }
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

    public void ColumnsUpdate()
    {
        foreach(var wave in columns)
        {
            AiStrategies.Instance.Column(wave);
        }
    }

    public void GroupUnits()
    {
        int random = Random.Range(0, 2);

        switch (random)
        {
            case 0:
                List<GameObject> waveMelee = new();
                int number = Random.Range(2, 4);
                for (int i = 0; i < number; i++)
                {
                    int randomIndex = Random.Range(0, Gamemanager.instance.aiTroops.Count);
                    GameObject waveUnit = Gamemanager.instance.aiTroops[randomIndex];
                    if (waveUnit.GetComponent<Unit>().unitSo.unitType != UnitType.Archer)
                    {
                        waveMelee.Add(waveUnit);
                    }
                    else
                    {
                        //Made with claude
                        bool hasNonMelee = Gamemanager.instance.aiTroops.Exists(u => u.GetComponent<Unit>().unitSo.unitType
                        != UnitType.Knight);

                        if (hasNonMelee) i--;
                        else break;
                    }
                }

                AiStrategies.Instance.Column(waveMelee);
                columns.Add(waveMelee);

                break;
            case 1:
                List<GameObject> waveRange = new();
                int numberRanged = Random.Range(1, 2);
                if (numberRanged != 2) return;
                for (int i = 0; i < numberRanged; i++)
                {
                    int randomIndex = Random.Range(0, Gamemanager.instance.aiTroops.Count);
                    GameObject waveUnit = Gamemanager.instance.aiTroops[randomIndex];
                    if (waveUnit.GetComponent<Unit>().unitSo.unitType != UnitType.Archer)
                    {
                        waveRange.Add(waveUnit);
                    }
                    else
                    {
                        //Made with claude
                        bool hasNonArcher = Gamemanager.instance.aiTroops.Exists(u => u.GetComponent<Unit>().unitSo.unitType
                        != UnitType.Archer);

                        if (hasNonArcher) i--;
                        else break;
                    }
                }

                AiStrategies.Instance.Column(waveRange);
                columns.Add(waveRange);

                break;
        }
    }
}
