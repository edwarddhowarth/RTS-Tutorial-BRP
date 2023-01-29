using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * GameObject: GAME
 * 
 */

public class UnitsSelection : MonoBehaviour
{
    private bool _isDraggingMouseBox = false;
    private Vector3 _dragStartPosition;

    Ray _ray;
    RaycastHit _raycastHit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDraggingMouseBox = true;
            _dragStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDraggingMouseBox = false;
        }

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
        foreach(GameObject unit in selectableUnits)
        {
            Debug.Log(unit.name + " " + unit.tag);
            inBounds = selectionBounds.Contains(
                Camera.main.WorldToViewportPoint(unit.transform.position)
                );
            if (inBounds)
                unit.GetComponent<UnitManager>().Select();
            else
                unit.GetComponent<UnitManager>().Deselect();
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
}
