using UnityEngine;

namespace Pointo.Unit
{
    [RequireComponent(typeof(PeonBehaviour))]
    public class Peon : Unit
    {
        private PeonBehaviour peonBehaviour;

        private new void Start()
        {
            base.Start();
            peonBehaviour = GetComponent<PeonBehaviour>();
        }
        
        private new void OnEnable()
        {
            base.OnEnable();
            PointoController.Actions.onObjectRightClicked += HandleObjectClicked;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            PointoController.Actions.onObjectRightClicked -= HandleObjectClicked;
        }
        
        private void HandleObjectClicked(GameObject targetObject)
        {
            if (!IsSelected || !unitSo.CanHandleResource()) return;
            
            // we check if it's a resource
            Resource.Resource targetResource = targetObject.GetComponent<Resource.Resource>();
            if (targetResource != null)
            {
                peonBehaviour.targetResource = targetResource;
                UnitTargetHandler.currentState = UnitTargetHandler.UnitState.Collecting;

                Debug.LogFormat("{0} is gathering {1}", UnitRaceType, targetResource.ResourceType);      
            }
            else
            {
                // if not a resource, we check if it's a enemy
                Unit targetUnit = targetObject.GetComponent<Unit>();
                if (targetUnit == null) return;
                
                // we check on the Scriptable Object if we should attack
                if (unitSo.ShouldAttack(targetUnit.UnitRaceType))
                {
                    // attack!
                    Debug.LogFormat("{0} is attacking {1} with {2} strength", UnitRaceType, targetUnit.UnitRaceType, unitSo.attackStrength);
                }
            }
        }
    }    
}