using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterManager : UnitManager
{
    public NavMeshAgent agent;
    
    private Character _character;
    
    private Ray _ray;
    private RaycastHit _raycastHit;

    public override Unit Unit
    {
        get { return _character; }
        set { _character = value is Character ? (Character)value : null; }
    }

    private void Update()
    {
        _CheckUnitsNavigation();
    }

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    
    //Check if any unit is selected and if so, move them to the mouse position if right click is pressed
    private void _CheckUnitsNavigation()
    {
        if (Globals.SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                    _ray,
                    out _raycastHit,
                    1000f,
                    Globals.TERRAIN_LAYER_MASK
                ))
            {
                foreach(UnitManager um in Globals.SELECTED_UNITS)
                    if (um.GetType() == typeof(CharacterManager))
                        ((CharacterManager)um).MoveTo(_raycastHit.point);
            }
        }
    }
    
}
