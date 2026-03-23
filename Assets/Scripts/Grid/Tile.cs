using Pointo.Unit;
using System.Diagnostics;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using Debug = UnityEngine.Debug;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private bool isWalkable;

    public Unit occupiedUnit;
    public bool Walkable => isWalkable && occupiedUnit == null;

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

    private void OnMouseDown()
    {
        if(Gamemanager.instance.state != GameState.Player_Turn) return;

        Debug.LogFormat("Unit on tile: {0}", occupiedUnit);

        if (occupiedUnit != null)
        {
            if (occupiedUnit.unitSo.unitRaceType == UnitRaceType.Human) UnitManager.instance.SetSelectedUnit((Unit)occupiedUnit);
            else
            {
                if (UnitManager.instance.selectedUnit != null)
                {
                    var enemy = (Unit)occupiedUnit;
                    //TODO: make attack logic and use it here
                    Destroy(enemy.gameObject);
                    UnitManager.instance.SetSelectedUnit(null);
                }
            }
            
        }
        else
        {
            if (UnitManager.instance.selectedUnit != null)
            {
                SetUnit(UnitManager.instance.selectedUnit);
                UnitManager.instance.SetSelectedUnit(null);
            }
        }
    }

    public void SetUnit(Unit unit)
    {
        if (unit.occupiedTile != null) unit.occupiedTile.occupiedUnit = null;
        //unit.SelectUnit();
        GameObject unitParent = unit.gameObject;
        unitParent.transform.position = transform.position;
        occupiedUnit = unit;
        unit.occupiedTile = this;
    }
}
