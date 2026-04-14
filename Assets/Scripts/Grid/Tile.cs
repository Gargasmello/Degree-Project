using Pointo.Unit;
using System.Diagnostics;
using TMPro;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using Debug = UnityEngine.Debug;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private bool isWalkable;

    public int MeleeScore;
    public int RangedScore;
    public int ArtilleryScore;

    public TextMeshProUGUI meleeScoreText;
    public TextMeshProUGUI rangedScoreText;
    public TextMeshProUGUI artilleryScoreText;

    public GameObject inRangeIcon;

    public Unit occupiedUnit;
    public bool Walkable => isWalkable && occupiedUnit == null;

    public virtual void Init(int x, int y)
    {
    }

    public void Start()
    {
        inRangeIcon = transform.Find("InRangeOfIcon").gameObject;
    }

    public void Update()
    {
        DisplayScore();
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
                    UnitManager.instance.selectedUnit.Attack(enemy);
                    UnitManager.instance.DeselectSelectedUnit();
                }
            }
            
        }
        else
        {
            if (UnitManager.instance.selectedUnit != null)
            {
                SetUnit(UnitManager.instance.selectedUnit);
                UnitManager.instance.DeselectSelectedUnit();
            }

            if (UnitManager.instance.SpawningUnit != null)
            {
                var spawnedUnit = Instantiate(UnitManager.instance.SpawningUnit);
                spawnedUnit.transform.position = transform.position;
                SetUnit(spawnedUnit.GetComponent<Unit>());
                UnitManager.instance.SpawningUnit = null;
                Gamemanager.instance.troopsCreated += 1;
            }
        }
    }

    public void SetUnit(Unit unit)
    {
        unit.MoveToTile(this);
    }

    public void TileInRangeOfPlayerUnit()
    {
        foreach(var playerUnit in Gamemanager.instance.playerTroops)
        {
            if (playerUnit.GetComponent<Unit>().unitSo.unitType == UnitType.Knight && RangeCalculation(playerUnit.GetComponent<Unit>())) MeleeScore += 1;
            else if (playerUnit.GetComponent<Unit>().unitSo.unitType == UnitType.Archer && RangeCalculation(playerUnit.GetComponent<Unit>())) RangedScore += 1;
            else if (playerUnit.GetComponent<Unit>().unitSo.unitType == UnitType.Catapult && RangeCalculation(playerUnit.GetComponent<Unit>())) ArtilleryScore += 1;
        }
    }

    public void FlagTilePoints()
    {
        if (GetComponent<FlagTile>() && GetComponent<FlagTile>().aiControl == false)
        {
            MeleeScore += 3;
            RangedScore += 3;
            ArtilleryScore += 3;
        }
        else if (GetComponent<FlagTile>() && AiManager.Instance.aiMode == AiMode.DEFEND)
        {
            MeleeScore += 0;
            RangedScore += 5;
            ArtilleryScore += 5;
        }
    }

    private bool RangeCalculation(Unit unit)
    {
        return Calc.IsWithinXRange(unit.transform, transform, unit.unitSo.range) && Calc.IsWithinYRange(unit.transform, transform, unit.unitSo.range);
    }

    private void DisplayScore()
    {
        string rangedText = $"{RangedScore}";
        string meleeText = $"{MeleeScore}";
        string artilleryText = $"{ArtilleryScore}";

        meleeScoreText.text = meleeText;
        rangedScoreText.text = rangedText;
        artilleryScoreText.text = artilleryText;
    }
}
