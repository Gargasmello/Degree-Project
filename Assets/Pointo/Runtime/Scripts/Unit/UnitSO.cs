using UnityEngine;

namespace Pointo.Unit
{
    /// <summary>
    /// Each Unit has a specific set of attributes that can bee found here.
    /// Tailor it for your purposes as much as you want and use it on the
    /// <see cref="Pointo.Unit.Unit"/> script attached to your GameObject.
    /// </summary>
    [CreateAssetMenu(fileName = "Unit_", menuName = "Unit/New Unit", order = 0)]
    public class UnitSO : ScriptableObject
    {
        public UnitRaceType unitRaceType;
        public UnitType unitType;
        public LayerMask targetMask;
        public Material mat;
        public float attackStrength = 1.0f;

        [Tooltip("Time to rest after collecting a Resource")]
        public float coolDownTime = 2f;

        public bool CanHandleResource()
        {
            return unitType == UnitType.Peon;
        }

        public bool CanFight()
        {
            //return unitType > UnitType.Peon;
            return true;
        }

        public bool ShouldAttack(UnitRaceType enemyUnitRaceType)
        {
            return CanFight() && unitRaceType != enemyUnitRaceType;
        }
    }
}