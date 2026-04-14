using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;

namespace Pointo.Unit
{
    //[RequireComponent(typeof(MeshRenderer))]
    //[RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(UnitTargetHandler))]
    public abstract class Unit : MonoBehaviour
    {
        public UnitSO unitSo;
        protected bool IsSelected { get; private set; }

        private NavMeshAgent navAgent;
        private Vector3 offset;
        public Tile occupiedTile;
        private float remainingMovePoints = 0;

        private float Health = 0;

        public bool canMove = true;
        public bool canAttack = true;

        [SerializeField]private GameObject selectedIcon;
        private GameObject inRangeIcon;
        private Vector3 startingPos;

        protected UnitTargetHandler UnitTargetHandler;

        public List<GameObject> tilesInRange;

        public UnitRaceType UnitRaceType => unitSo.unitRaceType;

        protected void Start()
        {
            if(unitSo.unitRaceType == UnitRaceType.Elf)
            {
                inRangeIcon = transform.Find("InRangeIcon").gameObject;
            }

            startingPos = transform.position;

            navAgent = GetComponent<NavMeshAgent>();
            UnitTargetHandler = GetComponent<UnitTargetHandler>();

            remainingMovePoints = unitSo.movementPoints;
            Health = unitSo.health;


        }

        private void Update()
        {
            if (Health <= 0)
            {
                //TODO: Might have to remove occupied unit and remove from list
                Destroy(gameObject);
            }
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
        }

        public void DeselectUnit()
        {
            selectedIcon.SetActive(false);
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
            if (RangeCalculation(tile) && canMove == true && tile.occupiedUnit == null)
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

            return Calc.IsWithinRange(tile.transform, transform, remainingMovePoints);
        }

        public void RefreshUnit()
        {
            canMove = true;
            canAttack = true;
            remainingMovePoints = unitSo.movementPoints;
        }

        public void MoveTowardEnemy()
        {
            Vector2 moveDir = GetMoveDirection(transform.position, ClosestUnit());
            GetTilesInRange();
            switch (unitSo.unitType)
            {
                case UnitType.Knight:
                    tilesInRange.Sort((a, b) => b.GetComponent<Tile>().MeleeScore.CompareTo(a.GetComponent<Tile>().MeleeScore));
                    break;
                case UnitType.Archer:
                    tilesInRange.Sort((a, b) => b.GetComponent<Tile>().RangedScore.CompareTo(a.GetComponent<Tile>().RangedScore));
                    break;
                case UnitType.Catapult:
                    tilesInRange.Sort((a, b) => b.GetComponent<Tile>().ArtilleryScore.CompareTo(a.GetComponent<Tile>().ArtilleryScore));
                    break;
            }

            //Made by claude
            GameObject bestTile = tilesInRange.FirstOrDefault(tileObj =>
            {
                Vector2 toTile = tileObj.transform.position - transform.position;
                return Vector2.Dot(toTile.normalized, moveDir) > 0.5f;
            });

            if(bestTile != null)
            {
                bestTile.GetComponent<Tile>().SetUnit(this);
            }

            tilesInRange.Clear();
        }

        public void MoveTowardFlag()
        {
            Vector2 moveDir = GetMoveDirection(transform.position, ClosestFlag());
            GetTilesInRange();
            switch (unitSo.unitType)
            {
                case UnitType.Knight:
                    tilesInRange.Sort((a, b) => b.GetComponent<Tile>().MeleeScore.CompareTo(a.GetComponent<Tile>().MeleeScore));
                    break;
                case UnitType.Archer:
                    tilesInRange.Sort((a, b) => b.GetComponent<Tile>().RangedScore.CompareTo(a.GetComponent<Tile>().RangedScore));
                    break;
                case UnitType.Catapult:
                    tilesInRange.Sort((a, b) => b.GetComponent<Tile>().ArtilleryScore.CompareTo(a.GetComponent<Tile>().ArtilleryScore));
                    break;
            }

            //Made by claude
            GameObject bestTile = tilesInRange.FirstOrDefault(tileObj =>
            {
                Vector2 toTile = tileObj.transform.position - transform.position;
                return Vector2.Dot(toTile.normalized, moveDir) > 0.5f;
            });

            if (bestTile != null)
            {
                bestTile.GetComponent<Tile>().SetUnit(this);
            }

            tilesInRange.Clear();
        }

        public void MoveTowardFlagDefence()
        {
            Vector2 moveDir = GetMoveDirection(transform.position, ClosestFlag());
            GetTilesInRange();
            switch (unitSo.unitType)
            {
                case UnitType.Knight:
                    tilesInRange.Sort((a, b) => b.GetComponent<Tile>().MeleeScore.CompareTo(a.GetComponent<Tile>().MeleeScore));
                    break;
                case UnitType.Archer:
                    tilesInRange.Sort((a, b) => b.GetComponent<Tile>().RangedScore.CompareTo(a.GetComponent<Tile>().RangedScore));
                    break;
                case UnitType.Catapult:
                    tilesInRange.Sort((a, b) => b.GetComponent<Tile>().ArtilleryScore.CompareTo(a.GetComponent<Tile>().ArtilleryScore));
                    break;
            }

            //Made by claude
            GameObject bestTile = tilesInRange.FirstOrDefault(tileObj =>
            {
                Vector2 toTile = tileObj.transform.position - transform.position;
                return Vector2.Dot(toTile.normalized, moveDir) > 0.5f;
            });

            if (bestTile != null)
            {
                bestTile.GetComponent<Tile>().SetUnit(this);
            }

            tilesInRange.Clear();
        }

        private void GetTilesInRange()
        {
            foreach (var tile in GridManager.instance.tilesList)
            {
                if (RangeCalculation(tile.GetComponent<Tile>()))
                {
                    tilesInRange.Add(tile);
                }
            }
        }

        //Claude made
        private Vector2 ClosestUnit()
        {
            Vector2 closest = Vector2.zero;
            float minDistance = float.MaxValue;

            foreach (var playerPos in Gamemanager.instance.playerTroops)
            {
                float dist = Mathf.Abs(transform.position.x - playerPos.transform.position.x) + Mathf.Abs(transform.position.y - playerPos.transform.position.y);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    
                    closest = new(playerPos.transform.position.x, playerPos.transform.position.y);
                }
            }

            return closest;
        }

        //Copy of code above
        private Vector2 ClosestFlag()
        {
            Vector2 closest = Vector2.zero;
            float minDistance = float.MaxValue;

            foreach (var flag in GridManager.instance.tilesList)
            {
                if (flag.GetComponent<FlagTile>() != null)
                {
                    float dist = Mathf.Abs(transform.position.x - flag.transform.position.x) + Mathf.Abs(transform.position.y - flag.transform.position.y);
                    if (dist < minDistance)
                    {
                        minDistance = dist;

                        closest = new(flag.transform.position.x, flag.transform.position.y);
                    }
                }
            }

            return closest;
        }

        //Claude made
        private Vector2 GetMoveDirection(Vector2 from, Vector2 to)
        {
            float dx = to.x - from.x;
            float dy = to.y - from.y;

            if (Mathf.Abs(dx) >= Mathf.Abs(dy))
                return new Vector2(Mathf.Sign(dx), 0);
            else
                return new Vector2(0, Mathf.Sign(dy));
        }

        public void GetEnemiesInRange()
        {
            foreach(var enemy in Gamemanager.instance.aiTroops)
            {
                if (Calc.IsWithinRange(enemy.transform, transform, unitSo.range))
                {
                    enemy.GetComponent<Unit>().inRangeIcon.SetActive(true);
                }
            }
        }

        public void ClearEnemiesInRangeIcons()
        {
            foreach (var enemy in Gamemanager.instance.aiTroops)
            {
                if (enemy.GetComponent<Unit>().inRangeIcon != null)
                {
                    enemy.GetComponent<Unit>().inRangeIcon.SetActive(false);
                }
            }
        }

        public void ActivateTilesInRangeIcons()
        {
            GetTilesInRange();
            foreach (var tile in tilesInRange)
            {
                if (canMove == true) tile.GetComponent<Tile>().inRangeIcon.SetActive(true);
            }
        }

        public void DeactivateTilesInRangeIcons()
        {
            foreach(var tile in tilesInRange)
            {
                tile.GetComponent<Tile>().inRangeIcon.SetActive(false);
            }
            tilesInRange.Clear();
        }

        public void Attack(Unit enemy)
        {
            if (unitSo.unitType == UnitType.Catapult)
            {
                if (unitSo.unitRaceType == UnitRaceType.Human)
                {
                    foreach (var enemies in Gamemanager.instance.aiTroops)
                    {
                        if (Calc.IsWithinRange(enemies.transform, transform, unitSo.range + 1) && canAttack)
                        {
                            enemies.GetComponent<Unit>().Health -= unitSo.attackStrength;

                            canAttack = false;

                            Debug.Log($"Attacked {enemies}");
                        }
                    }
                }
                else if (unitSo.unitRaceType == UnitRaceType.Elf)
                {
                    foreach (var enemies in Gamemanager.instance.playerTroops)
                    {
                        if (Calc.IsWithinRange(enemies.transform, transform, unitSo.range + 1) && canAttack)
                        {
                            enemies.GetComponent<Unit>().Health -= unitSo.attackStrength;

                            canAttack = false;

                            Debug.Log($"Attacked {enemies}");
                        }
                    }
                }
            }
            else if (Calc.IsWithinRange(enemy.transform, transform, unitSo.range) && canAttack)
            {
                enemy.Health -= unitSo.attackStrength;

                canAttack = false;

                Debug.Log($"Attacked {enemy}");

                //TODO: Add logic for artillery and make arrow point to enemy
            }
        }
    }
}