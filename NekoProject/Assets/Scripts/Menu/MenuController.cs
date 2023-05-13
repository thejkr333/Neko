using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject settings, mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        settings.SetActive(false);
    }
    public void NewGame()
    {
        GameManager.Instance.NewGame();
    }

    public void LoadGame()
    {

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
    }
}

