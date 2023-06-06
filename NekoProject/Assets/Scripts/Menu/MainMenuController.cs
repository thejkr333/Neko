using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class  MainMenuController : MonoBehaviour, NekoInput.IPlayerUIActions
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
        settings.SetActive(false);
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }
    public void NewGame()
    {
        GameManager.Instance.NewGame();
        GameManager.Instance.LoadScene("BosqueTurquesa");
    }

    public void LoadGame()
    {
        GameManager.Instance.LoadGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                 Application.Quit();
#endif
    }

    public void Settings()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }

    private void OnEnable()
    {
        controlsInput.PlayerUI.Enable();
    }
    private void OnDisable()
    {
        controlsInput.PlayerUI.Disable();
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
    #endregion

    public void OnClick(InputAction.CallbackContext context)
    {
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

