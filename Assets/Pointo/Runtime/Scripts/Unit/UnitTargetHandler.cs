using System;
using System.Collections;
using UnityEngine;

namespace Pointo.Unit
{
    /// <summary>
    /// This class is in charge of detecting a target when moving.
    /// It uses a SphereCast to detect a target (Resource or Enemy for example) and
    /// notifies a Behaviour when it's reached 
    /// </summary>
    [RequireComponent(typeof(Unit))]
    public class UnitTargetHandler : MonoBehaviour
    {
        [SerializeField] private float searchRadius = 1f;

        [Header("Toggle Gizmos")] 
        [SerializeField] private bool showGizmos;

        private float coolDownTime;
        private LayerMask targetLayerMask;

        public UnitState currentState;

        public Action<GameObject> OnObjectReached { private get; set; }

        public Action OnJobCancelled { private get; set; }

        public bool ShouldScanWorld;

        private void Start()
        {
            var unit = GetComponent<Unit>();
            coolDownTime = unit.GetCooldownTime();
            targetLayerMask = unit.GetTargetLayerMask();

            currentState = UnitState.Available;
        }

        private void Update()
        {
            if (!ShouldScanWorld) return;
            
            switch (currentState)
            {
                case UnitState.Available: break;
                case UnitState.Collecting:
                    HandleAvailableState();
                    break;
                case UnitState.CancellingWork: break;
                case UnitState.Fighting: 
                    HandleAvailableState();
                    break;
            }
        }

        private void HandleAvailableState()
        {
            // we search for near resources to start collecting
            if (!Physics.SphereCast(
                origin: transform.position, 
                radius: searchRadius, 
                direction: transform.forward, 
                hitInfo: out RaycastHit hit, 
                maxDistance: searchRadius,
                layerMask: targetLayerMask)) return;

            OnObjectReached?.Invoke(hit.transform.gameObject);
        }

        public void CancelJobIfBusy()
        {
            if (currentState == UnitState.Collecting)
            {
                currentState = UnitState.CancellingWork;

                OnJobCancelled?.Invoke();
                
                StartCoroutine(CancelJob());
            }
            else
            {
                currentState = UnitState.Available;
                ShouldScanWorld = false;
            }
        }

        private IEnumerator CancelJob()
        {
            yield return new WaitForSeconds(coolDownTime);
            currentState = UnitState.Available;
            ShouldScanWorld = false;
        }

        public enum UnitState
        {
            Available,
            Collecting,
            CancellingWork,
            Fighting
        }

        public bool IsFighting()
        {
            return currentState == UnitState.Fighting;
        }
        
        #region Debug

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, searchRadius);
        }

        #endregion
    }
}