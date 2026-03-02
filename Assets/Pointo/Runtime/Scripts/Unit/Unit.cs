using UnityEngine;
using UnityEngine.AI;

namespace Pointo.Unit
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(UnitTargetHandler))]
    public abstract class Unit : MonoBehaviour
    {
        public UnitSO unitSo;
        protected bool IsSelected { get; private set; }

        private NavMeshAgent navAgent;
        private Vector3 offset;

        private GameObject selectedIcon;
        private Vector3 startingPos;

        protected UnitTargetHandler UnitTargetHandler;

        public UnitRaceType UnitRaceType => unitSo.unitRaceType;

        protected void Start()
        {
            selectedIcon = transform.Find("Selector").gameObject;
            selectedIcon.SetActive(false);

            startingPos = transform.position;

            navAgent = GetComponent<NavMeshAgent>();
            UnitTargetHandler = GetComponent<UnitTargetHandler>();

            if (unitSo.mat != null) GetComponent<MeshRenderer>().material = unitSo.mat;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G)) navAgent.SetDestination(startingPos);
        }

        protected void OnEnable()
        {
            PointoController.Actions.onGroundClicked += MoveToSpot;
            PointoController.Actions.onObjectRightClicked += HandleObjectClicked;
        }

        protected void OnDisable()
        {
            PointoController.Actions.onGroundClicked -= MoveToSpot;
            PointoController.Actions.onObjectRightClicked -= HandleObjectClicked;
        }

        public float GetCooldownTime()
        {
            return unitSo.coolDownTime;
        }

        public LayerMask GetTargetLayerMask()
        {
            return unitSo.targetMask;
        }

        public void SelectUnit()
        {
            selectedIcon.SetActive(true);
            IsSelected = true;
        }

        public void DeselectUnit()
        {
            selectedIcon.SetActive(false);
            offset = Vector3.zero;
            IsSelected = false;
        }

        public void CalculateOffset(Vector3 _center)
        {
            var center = new Vector3(_center.x, transform.position.y, _center.z);
            offset = center - transform.position;
        }

        /// Used when Unit is already selected and it doesn't need to know again where the center
        /// of the selection is
        private void MoveToSpot(Vector3 worldPosition)
        {
            if (!IsSelected) return;
            
            UnitTargetHandler.CancelJobIfBusy();

            var pos = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
            var moveToPos = pos + offset;
            navAgent.SetDestination(moveToPos);
        }

        private void HandleObjectClicked(GameObject targetObject)
        {
            if (!IsSelected) return;
            
            var targetPos = targetObject.transform.position;
            UnitTargetHandler.CancelJobIfBusy();
            UnitTargetHandler.ShouldScanWorld = true;

            var pos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            var moveToPos = pos + offset;
            navAgent.SetDestination(moveToPos);
        }
    }
}