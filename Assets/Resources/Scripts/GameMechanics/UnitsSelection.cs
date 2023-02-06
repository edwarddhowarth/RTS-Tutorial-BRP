using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


/*
 * 
 * GameObject: GAME
 *
 * Handles mouse selection and command groups
 */

public class UnitsSelection : MonoBehaviour
{
    public UIManager uiManager;
    private bool _isDraggingMouseBox = false;
    private Vector3 _dragStartPosition;

    private bool _cursorOverUI;
    Ray _ray;
    RaycastHit _raycastHit;
    
    //Control Groups
    private Dictionary<int, List<UnitManager>> _selectionGroups = new Dictionary<int, List<UnitManager>>();
    
    private void Update()
    {
        MouseSelectionUpdate();
    }

    //Only gets called once on first Down Click and once when released
    //Will only work if cursor isnt over a UI element
    public void OnMouse1Down(InputAction.CallbackContext context)
    {
        if (context.performed && _cursorOverUI)
        {
            _isDraggingMouseBox = true;
            _dragStartPosition = Input.mousePosition;
        }
        else
        {
            _isDraggingMouseBox = false;
        }
        
    }

    private void MouseSelectionUpdate()
    {
        _cursorOverUI = !EventSystem.current.IsPointerOverGameObject();

        if (_isDraggingMouseBox && _dragStartPosition != Input.mousePosition)
            _SelectUnitsInDraggingBox();

        if(Globals.SELECTED_UNITS.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                _DeselectAllUnits();
            if(Input.GetMouseButtonDown(0))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(
                       _ray,
                       out _raycastHit,
                       1000f
                   ))
                {
                    if (_raycastHit.transform.tag == "Terrain")
                        _DeselectAllUnits();
                }
            }
        }
            
        
    }


    private void _SelectUnitsInDraggingBox()
    {
        Bounds selectionBounds = Utils.GetViewportBounds(
            Camera.main,
            _dragStartPosition,
            Input.mousePosition
            );
        GameObject[] selectableUnits = GameObject.FindGameObjectsWithTag("Unit");
        bool inBounds;
        
        //For all units owned by the player, check if they are in the selection box
        foreach(GameObject unit in selectableUnits)
        {
            inBounds = selectionBounds.Contains(
                Camera.main.WorldToViewportPoint(unit.transform.position)
                );
            if (inBounds)
                unit.GetComponent<UnitManager>().Select();
            else
            {
                //Only remove unit from global selected units list if it exists in there first
                if(Globals.SELECTED_UNITS.Contains(unit.GetComponent<UnitManager>()))
                    unit.GetComponent<UnitManager>().Deselect();
            }
                
        }
    }

    private void OnGUI()
    {
        if (_isDraggingMouseBox)
        {
            var rect = Utils.GetScreenRect(_dragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.1f));
            Utils.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
        }
    }

    private void _DeselectAllUnits()
    {
        List<UnitManager> selectedUnits = new List<UnitManager>(Globals.SELECTED_UNITS);
        foreach (UnitManager um in selectedUnits)
            um.Deselect();
    }
    
    //Selection Group

    public void SelectUnitsGroup(int groupIndex)
    {
        _ReselectGroup(groupIndex);
    }

    public void CreateSelectionGroup(int groupIndex)
    {
        //User clearing a control group by assign a group with no units selected
        if (Globals.SELECTED_UNITS.Count == 0)
        {
            //Reset group to empty
            if (_selectionGroups.ContainsKey(groupIndex))
                _RemoveSelectionGroup(groupIndex);
            return;
        }

        //Add selected units to control group
        List<UnitManager> groupUnits = new List<UnitManager>(Globals.SELECTED_UNITS);
        _selectionGroups[groupIndex] = groupUnits;
        
        uiManager.ToggleSelectionGroupButton(groupIndex, true);
    }

    private void _RemoveSelectionGroup(int groupIndex)
    {
        _selectionGroups.Remove(groupIndex);
        uiManager.ToggleSelectionGroupButton(groupIndex, false);
    }

    private void _ReselectGroup(int groupIndex)
    {
        //Selected group doesnt exist so do nothing
        if (!_selectionGroups.ContainsKey(groupIndex)) return;
        
        _DeselectAllUnits();
        foreach(UnitManager um in _selectionGroups[groupIndex])
            um.Select();
    }
    
    
}
