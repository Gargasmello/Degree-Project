using NUnit.Framework;
using Pointo.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{

    public static Gamemanager instance;

    public GameState state;

    public static event Action<GameState> StateChanged;

    public List<GameObject> playerTroops;
    public List<GameObject> aiTroops;

    public TextMeshProUGUI winText, loseText;

    public float turns, troopsCreated;
    public bool won;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Start);
        UpdateTroops();
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Start:
                HandleStart();
                break;
            case GameState.Spawn_Unit:
                HandleUnitSpawn();
                break;
            case GameState.Player_Turn:
                HandlePlayerTurn();
                break;
            case GameState.Enemy_Turn:
                HandleEnemyTurn();
                break;
            case GameState.Won:
                HandleWin();
                break;
            case GameState.Lost:
                HandleLoss();
                break;
        }

        StateChanged?.Invoke(newState);
    }

    private void HandleEnemyTurn()
    {
        RefreshAiTroops();
        RefreshTileScores();
        Money.instance.GainResourcesAi(Money.instance.UpdateFlagOwnership());
        AiManager.Instance.SpawnUnit();
        AiManager.Instance.EvaluateTiles(GridManager.instance.tilesList);
        AiManager.Instance.MoveUnits();
        AiManager.Instance.AttackEnemies();
        UpdateGameState(GameState.Won);
    }

    private void HandleWin()
    {
        if (GridManager.instance.flagTiles.All(tile => tile.GetComponent<FlagTile>().playerControl) || aiTroops == null)
        {
            winText.gameObject.SetActive(true);
            won = true;
            Debug.Log($"Player win {won}, turns played {turns}, troops created {troopsCreated}");
        }
        else
        {
            UpdateGameState(GameState.Player_Turn);
        }
    }

    private void HandleLoss()
    {
        if (GridManager.instance.flagTiles.All(tile => tile.GetComponent<FlagTile>().aiControl) || playerTroops == null)
        {
            loseText.gameObject.SetActive(true);
            won = false;
            Debug.Log($"Player win {won}, turns played {turns}, troops created {troopsCreated}");
        }
        else
        {
            UpdateGameState(GameState.Enemy_Turn);
        }
    }

    private void HandlePlayerTurn()
    {
        turns += 1;
        Money.instance.GainResourcesPlayer(Money.instance.UpdateFlagOwnership());
        RefreshPlayerTroops();
    }

    private void HandleStart()
    {
        UpdateGameState(GameState.Spawn_Unit);
    }

    private void HandleUnitSpawn()
    {
        UpdateGameState(GameState.Player_Turn);
    }

    private void UpdateTroops()
    {
        playerTroops = GameObject.FindGameObjectsWithTag("Player").ToList();
        aiTroops = GameObject.FindGameObjectsWithTag("Ai").ToList();
    }
    private void RefreshPlayerTroops()
    {
        UpdateTroops();
        foreach (var player in playerTroops)
        {
            player.GetComponent<Unit>().RefreshUnit();
        }
    }

    private void RefreshAiTroops()
    {

        UpdateTroops();
        foreach (var player in aiTroops)
        {
            player.GetComponent<Unit>().RefreshUnit();
        }
    }

    private void RefreshTileScores()
    {
        foreach (var tile in GridManager.instance.tilesList)
        {
            tile.GetComponent<Tile>().ArtilleryScore = 0;
            tile.GetComponent<Tile>().MeleeScore = 0;
            tile.GetComponent<Tile>().RangedScore = 0;
        }
    }
}

public enum GameState
{
    Start,
    End,
    Spawn_Unit,
    Player_Turn,
    Enemy_Turn,
    Won,
    Lost
}