using System;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{

    public static Gamemanager instance;

    public GameState state;

    public static event Action<GameState> StateChanged;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Start);
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
        
    }

    private void HandleStart()
    {
        GridManager.instance.GenerateGrid();
        UpdateGameState(GameState.Spawn_Unit);
    }

    private void HandleUnitSpawn()
    {
        UnitManager.instance.SpawnPlayerBaseUnits();
        UnitManager.instance.SpawnAiBaseUnits();
        UpdateGameState(GameState.Player_Turn);
    }

    private void HandleEnd()
    {

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