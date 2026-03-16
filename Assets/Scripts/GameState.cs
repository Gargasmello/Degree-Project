using UnityEngine;

public enum GameState
{
    Start,
    Player_Turn,
    Enemy_Turn,
    Won,
    Lost
}

public class NewMonoBehaviourScript : MonoBehaviour
{

    public GameState state;

    private void Start()
    {
        state = GameState.Start;

    }
}
