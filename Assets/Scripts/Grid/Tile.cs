using Pointo.Unit;
using System.Diagnostics;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private bool isWalkable;

    public Unit occupiedUnit;
    public bool Walkable => isWalkable && occupiedUnit != null;

    public virtual void Init(int x, int y)
    {
    }

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    public void SetUnit(Unit unit)
    {
        if (unit.occupiedTile != null) unit.occupiedTile.occupiedUnit = null;
        unit.transform.position = transform.position;
        occupiedUnit = unit;
        unit.occupiedTile = this;
    }
}
