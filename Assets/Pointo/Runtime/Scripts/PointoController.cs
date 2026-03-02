using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Berserk.Utils.Mouse;
using Pointo.Unit;
using UnityEngine;
using Color = UnityEngine.Color;

public class PointoController : MonoBehaviour
{
    public static class Actions
    {
        public static Action<GameObject> onObjectRightClicked = delegate { };
        public static Action<Vector3> onGroundClicked = delegate { };
    }
    
    [Header("RayCasting")] [SerializeField]
    private LayerMask groundMask;

    [Tooltip("Use for the selection mask")]
    [SerializeField] private LayerMask unitMask;
    [Tooltip("What can be right clicked to label it as a Targets")]
    [SerializeField] private LayerMask targetsMask;

    [Header("GameObject for selection box")] [SerializeField]
    private GameObject selectorBox;

    [Range(0, 3)] [SerializeField] private float selectorBoxHeight = 0.5f;
    [SerializeField] private GameObject selectFx;

    [SerializeField] private bool prioritizeClosestToSelection = true;

    [Header("Toggle Gizmos")] [SerializeField]
    private bool showGizmos;

    private readonly List<Unit> selectedUnits = new List<Unit>();
    private Camera cam;
    private Vector3 endPoint;
    private Vector3 halfExtents;

    private Vector3 rectCenter;
    private Vector3 rectSize;

    // Selection Info
    private RectangleF selectionRect;

    // Selection Box start/end points
    private Vector3 startPoint;

    private void Start()
    {
        cam = Camera.main;
    }
    
    private void Update()
    {
        HandleLeftMouse();

        HandleRightMouse();
    }

    #region Input Handling

    private void HandleRightMouse()
    {
        if (Input.GetMouseButtonDown(1)) HandleUnitsToResource();
    }

    private void HandleLeftMouse()
    {
        // handle first click
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = DoRayToGround();
            selectorBox.SetActive(true);
        }

        // while we are dragging
        if (Input.GetMouseButton(0))
        {
            endPoint = DoRayToGround();
            HandleRectangle();
            selectorBox.transform.position = rectCenter;
            selectorBox.transform.localScale = rectSize + new Vector3(0f, selectorBoxHeight, 0f);
        }

        // handle release click
        if (Input.GetMouseButtonUp(0))
        {
            selectorBox.SetActive(false);
            endPoint = DoRayToGround();
            HandleRectangle();
            SelectAllUnits();
        }
    }

    private void HandleUnitsToResource()
    {
        var resourceGameObject = DoRayToResource();
        if (resourceGameObject != null)
        {
            AddPointFX(resourceGameObject.transform.position);
            Actions.onObjectRightClicked(resourceGameObject);
        }
        else
        {
            // if we just hit the ground mask, we move there
            Vector3 groundPos = DoRayToGround();
            AddPointFX(groundPos);
            Actions.onGroundClicked(groundPos);
        }
    }

    private void AddPointFX(Vector3 fxPos)
    {
        if (selectFx == null) return;

        Terrain terrain = Terrain.activeTerrain;

        float terrainHeight = terrain != null ? terrain.SampleHeight(fxPos) : 0f;
        fxPos.y = terrainHeight + 0.5f;
            
        GameObject fx = Instantiate(selectFx, fxPos, Quaternion.identity);

        if (!Physics.Raycast(fxPos, Vector3.down, out var hit)) return;
        // we adjust the rotation according to the ground click
        Quaternion slopeRotation = Quaternion.FromToRotation(fx.transform.up, hit.normal) * fx.transform.rotation;
        fx.transform.rotation = slopeRotation;
    }

    private void HandleRectangle()
    {
        //found at : https://forum.unity.com/threads/creating-a-rect-from-two-vectors.375447/

        rectSize = startPoint - endPoint;
        rectSize.x = Mathf.Abs(rectSize.x);
        rectSize.y = Mathf.Abs(rectSize.y);
        rectSize.z = Mathf.Abs(rectSize.z);

        rectCenter = (startPoint + endPoint) / 2f;

        halfExtents = rectSize / 2f;
    }

    #endregion

    #region Unit Handling

    private void ClearAllUnits()
    {
        foreach (var unit in selectedUnits) unit.DeselectUnit();

        selectedUnits.Clear();
    }

    private void SelectAllUnits()
    {
        ClearAllUnits();

        var check = Physics.BoxCastAll(
            rectCenter,
            halfExtents,
            Vector3.up,
            Quaternion.identity,
            Mathf.Infinity,
            unitMask,
            QueryTriggerInteraction.UseGlobal);

        if (prioritizeClosestToSelection)
            // we order comparing the distance to the center of the drawn rectangle
            check = check.OrderBy(d => (d.collider.transform.position - rectCenter).sqrMagnitude).ToArray();

        var firstUnitType = UnitRaceType.None;

        foreach (var hit in check)
        {
            var unit = hit.collider.GetComponent<Unit>();
            if (unit == null) continue;
            
            // if it's the first we find a selected object we add it and set that as the searched type
            if (firstUnitType == UnitRaceType.None)
            {
                firstUnitType = unit.UnitRaceType;

                unit.SelectUnit();
                unit.CalculateOffset(rectCenter);
                selectedUnits.Add(unit);
            }
            else if (firstUnitType == unit.UnitRaceType)
            {
                // if we already had one type selected, we select all of that type
                unit.SelectUnit();
                unit.CalculateOffset(rectCenter);
                selectedUnits.Add(unit);
            }
        }
    }

    #endregion

    #region Raycasting

    private Vector3 DoRayToGround()
    {
        return Mouse3D.GetMouseWorldPosition(cam, groundMask);
    }

    private GameObject DoRayToResource()
    {
        return Mouse3D.GetMouseWorldObject(cam, targetsMask);
    }

    #endregion
    
    #region Debug

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(rectCenter, rectSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startPoint, 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPoint, 0.5f);

        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(rectCenter, 0.5f);
    }

    #endregion
}