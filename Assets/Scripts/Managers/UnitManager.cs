using Pointo.Unit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    public List<GameObject> units;

    [SerializeField] private int meleeCount;
    [SerializeField] private int rangedCount;
    [SerializeField] private int artilleryCount;


    private void Awake()
    {
        instance = this;
    }

    public void SpawnBaseUnits()
    {
        foreach (var unit in units) 
        { 
            var spawnedUnit = Instantiate(unit);
            var randomSpawnTile = GridManager.instance.GetPlayerSpawnTile();

            randomSpawnTile.SetUnit(spawnedUnit.GetComponent<Knight>());
            
        }
    }

}
