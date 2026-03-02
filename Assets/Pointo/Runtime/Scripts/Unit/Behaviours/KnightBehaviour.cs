using UnityEngine;

namespace Pointo.Unit
{
    [RequireComponent(typeof(UnitTargetHandler))]
    [RequireComponent(typeof(Knight))]
    public class KnightBehaviour : MonoBehaviour
    {
        private UnitTargetHandler targetHandler;
        private Knight knightScript;
        
        private void Start()
        {
            knightScript = GetComponent<Knight>();
            targetHandler = GetComponent<UnitTargetHandler>();
            targetHandler.OnObjectReached = HandleEnemyReached;
        }

        private void HandleEnemyReached(GameObject targetObject)
        {
            Unit targetUnit = targetObject.GetComponent<Unit>();
            if (targetUnit == null) return;

            // we check on the Scriptable Object if we should attack
            if (targetHandler.IsFighting() && knightScript.unitSo.ShouldAttack(targetUnit.UnitRaceType))
            {
                // attack!
                Debug.LogFormat("{0} is attacking {1} with {2} strength", knightScript.UnitRaceType, targetUnit.UnitRaceType, knightScript.unitSo.attackStrength);
            }
        }
    }   
}