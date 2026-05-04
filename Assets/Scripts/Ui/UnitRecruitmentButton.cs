using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UnitRecruitmentButton : MonoBehaviour
{
    [SerializeField] private GameObject unit;
    [SerializeField] private int cost;

    private void Awake()
    {
        
    }

    public void SetSpawnedUnit()
    {
        if (Money.instance.resources >= cost && Gamemanager.instance.state == GameState.Player_Turn)
        {
            UnitManager.instance.SpawningUnit = unit;
            Money.instance.resources -= cost;
            Money.instance.SetCountText();
            foreach (var tile in Gamemanager.instance.lastColumnTiles)
            {
                tile.GetComponent<Tile>().inRangeIcon.SetActive(true);
            }
        }
    }
}
