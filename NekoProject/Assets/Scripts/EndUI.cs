using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class EndUI : MonoBehaviour, NekoInput.IPlayerUIActions
{

    [SerializeField] TMP_Text endText;

    [SerializeField] GameObject playAgainButton;
    private NekoInput controlsInput;
    private void Awake()
    {
        controlsInput = new NekoInput();
        controlsInput.PlayerUI.SetCallbacks(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        endText.text = GameManager.Instance.FinalText;
        if (GameManager.Instance.currentScheme == Controllers.Controller) EventSystem.current.SetSelectedGameObject(playAgainButton);
    }

    public void PlayAgain()
    {
        AudioManager.Instance.PlaySound("Button");
        GameManager.Instance.LoadScene("Menu");
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

        EventSystem.current.SetSelectedGameObject(playAgainButton);
    }

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
