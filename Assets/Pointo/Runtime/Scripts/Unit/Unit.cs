using System.Net;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;

namespace Pointo.Unit
{
    //[RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(UnitTargetHandler))]
    public abstract class Unit : MonoBehaviour
    {
        public UnitSO unitSo;
        protected bool IsSelected { get; private set; }

        private NavMeshAgent navAgent;
        private Vector3 offset;
        public Tile occupiedTile;
        private float remainingMovePoints = 0;

        public bool canMove = true;
        public bool canAttack = true;

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

            remainingMovePoints = unitSo.movementPoints;

            //if (unitSo.mat != null) GetComponent<MeshRenderer>().material = unitSo.mat;
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

        //TODO: Make unit not move toward enemy unit if the unit is within range.
        //  Make unit only move up into range if used to target enemy unit outside range
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
        public void MoveToTile(Tile tile)
        {
            if (RangeCalculation(tile) && canMove == true)
            {
                if (occupiedTile != null) occupiedTile.occupiedUnit = null;
                transform.position = tile.transform.position;
                tile.occupiedUnit = this;
                occupiedTile = tile;
                canMove = false;
            }
        }

        private bool RangeCalculation(Tile tile)
        {
            return IsWithinXRange(tile.transform) && IsWithinYRange(tile.transform);
        }

        private bool IsWithinXRange(Transform target)
        {
            float range = remainingMovePoints;
            return Mathf.Abs(target.position.x - transform.position.x) <= range;
        }

        private bool IsWithinYRange(Transform target)
        {
            float range = remainingMovePoints;
            return Mathf.Abs(target.position.y - transform.position.y) <= range;
        }

        private void RefreshUnit()
        {
            canMove = true;
            canAttack = true;
            remainingMovePoints = unitSo.movementPoints;
        }
    }
}