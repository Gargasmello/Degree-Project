using UnityEngine;
using TMPro;

public class TurnButton : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Gamemanager.instance.state == GameState.Player_Turn)
        {
            textMeshProUGUI.text = "End Turn";
        }
        else
        {
            textMeshProUGUI.text = "Ai turn";
        }
    }

    public void EndTurn()
    {
        if (Gamemanager.instance.state != GameState.Player_Turn) return;
        Gamemanager.instance.UpdateGameState(GameState.Enemy_Turn);
    }
}
