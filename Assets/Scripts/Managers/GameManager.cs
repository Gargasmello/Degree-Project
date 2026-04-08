using NUnit.Framework;
using Pointo.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{

    public static Gamemanager instance;

    public GameState state;

    public static event Action<GameState> StateChanged;

    public List<GameObject> playerTroops;
    public List<GameObject> aiTroops;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Start);
        UpdateAiTroops();
        UpdatePlayerTroops();
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Start:
                HandleStart();
                break;
            case GameState.End:
                HandleEnd();
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
        UpdateAiTroops();
        RefreshTileScores();
        AiManager.Instance.EvaluateTiles(GridManager.instance.tilesList);
        UpdateGameState(GameState.Player_Turn);
    }

    private void HandleWin()
    {

    }

    private void HandleLoss()
    {

    }

    private void HandlePlayerTurn()
    {
        Money.instance.GainResources();
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

    private void HandleEnd()
    {

    }

    private void UpdateAiTroops()
    {
        playerTroops = GameObject.FindGameObjectsWithTag("Player").ToList();
    }

    private void UpdatePlayerTroops()
    {
        aiTroops = GameObject.FindGameObjectsWithTag("Ai").ToList();
    }

    private void RefreshPlayerTroops()
    {
        UpdatePlayerTroops();
        foreach (var player in playerTroops)
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