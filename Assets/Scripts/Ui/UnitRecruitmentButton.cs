using UnityEngine;

public class UnitRecruitmentButton : MonoBehaviour
{
    [SerializeField] private GameObject unit;
    [SerializeField] private int cost;

    public void SetSpawnedUnit()
    {
        if (Money.instance.resources >= cost && Gamemanager.instance.state == GameState.Player_Turn)
        {
            UnitManager.instance.SpawningUnit = unit;
            Money.instance.resources -= cost;
            Money.instance.SetCountText();
        }
    }
}
