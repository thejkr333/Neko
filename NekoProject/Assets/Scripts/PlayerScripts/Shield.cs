////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public class Shield : MonoBehaviour, NekoInput.IShieldActions
{
    [SerializeField] Transform target;
    [SerializeField] float rotationSpeed, distanceFromPlayer;

    private NekoInput controlInput;

    [SerializeField] Vector3 inputPos;

    private void Awake()
    {
        controlInput = new NekoInput();
        controlInput.Shield.SetCallbacks(this);
    }

    // Update is called once per frame
    void Update()
    {
        MoveToCursor();
    }

    void RotateAroundPlayer()
    {
        transform.RotateAround(target.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    void MoveToCursor()
    {       
        Vector3 _position;
        if(GameManager.Instance.currentScheme == Controllers.KbMouse)
        {
            inputPos = Mouse.current.position.ReadValue();
            inputPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, Mathf.Abs(Camera.main.transform.position.z)));
            inputPos.z = target.position.z;
            _position = new Vector3(inputPos.x - target.position.x, inputPos.y - target.position.y, target.position.z);
        }
        else
        {
            inputPos = controlInput.Shield.ShieldPosition.ReadValue<Vector2>();
            _position = inputPos != Vector3.zero ? inputPos : new Vector3(1, 0, 0); 
        }

        _position = _position.normalized * distanceFromPlayer;
        transform.right = _position.normalized;
        transform.position = target.position + _position;
    }

    private void OnEnable()
    {
        controlInput.Shield.Enable();
    }

    private void OnDisable()
    {
        controlInput.Shield.Disable();
    }
    public void OnShieldPosition(InputAction.CallbackContext context)
    {
        
    }
}
