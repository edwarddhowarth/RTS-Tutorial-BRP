using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerInput _playerInput;

    // Start is called before the first frame update
    void Start()
    {
        _playerInput.actions.FindActionMap("ControlGroups").Enable();
        _playerInput.actions.FindActionMap("UnitSelection").Enable();
    }

    public void ToggleControlGroups(bool toggle)
    {
        if(toggle)
            _playerInput.actions.FindActionMap("ControlGroups").Enable();
        else
            _playerInput.actions.FindActionMap("ControlGroups").Disable();
    }

    public void ToggleUnitSelection(bool toggle)
    {
        if(toggle)
            _playerInput.actions.FindActionMap("UnitSelection").Enable();
        else
            _playerInput.actions.FindActionMap("UnitSelection").Disable();
    }

    public void ToggleUI(bool toggle)
    {
        if(toggle)
            _playerInput.actions.FindActionMap("UI").Enable();
        else
            _playerInput.actions.FindActionMap("UI").Disable();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
