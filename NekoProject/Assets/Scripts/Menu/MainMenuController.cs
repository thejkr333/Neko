////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour, NekoInput.IPlayerUIActions
{
    [SerializeField] GameObject settings, mainMenu, newGameButton;

    private NekoInput controlsInput;
    private void Awake()
    {
        controlsInput = new NekoInput();
        controlsInput.PlayerUI.SetCallbacks(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusic("Menu");
        settings.SetActive(false);
        if (GameManager.Instance.currentScheme == Controllers.Controller) EventSystem.current.SetSelectedGameObject(newGameButton);
    }
    public void NewGame()
    {
        AudioManager.Instance.PlaySound("Button");
        AudioManager.Instance.PlayMusic("Game_1");
        GameManager.Instance.NewGame();
    }

    public void LoadGame()
    {
        AudioManager.Instance.PlayMusic("Game_1");
        AudioManager.Instance.PlaySound("Button");
        GameManager.Instance.LoadGame();
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySound("Button");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                 Application.Quit();
#endif
    }

    public void Settings()
    {
        AudioManager.Instance.PlaySound("Button");
        mainMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void MainMenu()
    {
        AudioManager.Instance.PlaySound("Button");
        mainMenu.SetActive(true);
        settings.SetActive(false);
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }

    private void OnEnable()
    {
        controlsInput.PlayerUI.Enable();
        GameManager.Instance.ControllerConected += ControllerConected;
    }


    private void OnDisable()
    {
        controlsInput.PlayerUI.Disable();
        GameManager.Instance.ControllerConected -= ControllerConected;
    }
    private void ControllerConected()
    {
        if (EventSystem.current.alreadySelecting) return;

        if(mainMenu.activeInHierarchy) EventSystem.current.SetSelectedGameObject(newGameButton);
        else EventSystem.current.SetSelectedGameObject(settings.transform.GetChild(0).GetChild(0).gameObject);
    }

    #region NotUsedInput
    public void OnRightClick(InputAction.CallbackContext context)
    {

    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {

    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {

    }
    public void OnPoint(InputAction.CallbackContext context)
    {

    }
    public void OnNavigate(InputAction.CallbackContext context)
    {

    }

    public void OnNextPage(InputAction.CallbackContext context)
    {

    }

    public void OnPreviousPage(InputAction.CallbackContext context)
    {

    }
    public void OnClose(InputAction.CallbackContext context)
    {
        
    }
    #endregion

    public void OnClick(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.currentScheme != Controllers.Controller) return;

        if (context.performed)
        {
            GameObject _selected = EventSystem.current.currentSelectedGameObject;
            if (_selected != null && _selected.activeSelf)
            {
                if (_selected.TryGetComponent(out IClickable clickable))
                {
                    clickable.OnSelected();
                }
            }
        }
    }
}

