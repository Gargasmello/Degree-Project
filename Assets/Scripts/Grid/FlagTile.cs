using UnityEngine;
using UnityEditor;
using Pointo.Unit;

public class FlagTile : Tile
{

    [SerializeField] private Color playerColor, AiColor;

    public override void Init(int x, int y)
    {
        base.Init(x, y);
    }

    private void OnValidate()
    {
        name = $"Tile {transform.position.x} {transform.position.y}";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Unit>().UnitRaceType == UnitRaceType.Human)
        {
            PlayerControl();
        }
        else if (collision.gameObject.GetComponent<Unit>().UnitRaceType == UnitRaceType.Elf)
        {
            AiControl();
        }
    }

    private void PlayerControl()
    {
        GetComponent<SpriteRenderer>().color = playerColor;
    }

    private void AiControl()
    {
        GetComponent<SpriteRenderer>().color = AiColor;
    }
}
