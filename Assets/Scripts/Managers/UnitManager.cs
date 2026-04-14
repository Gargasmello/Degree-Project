using Pointo.Unit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    public List<GameObject> units;

    public Unit selectedUnit;

    public GameObject SpawningUnit;

    [SerializeField] private int meleeCount;
    [SerializeField] private int rangedCount;
    [SerializeField] private int artilleryCount;


    private void Awake()
    {
        instance = this;
    }

    public void SetSelectedUnit(Unit selectedUnit)
    {
        if (this.selectedUnit == null)
        {
            this.selectedUnit = selectedUnit;
            this.selectedUnit.SelectUnit();
            this.selectedUnit.GetEnemiesInRange();
            this.selectedUnit.ActivateTilesInRangeIcons();
        }
    }

    public void DeselectSelectedUnit()
    {
        this.selectedUnit.DeselectUnit();
        this.selectedUnit.ClearEnemiesInRangeIcons();
        this.selectedUnit.DeactivateTilesInRangeIcons();
        this.selectedUnit = null;
    }
}
