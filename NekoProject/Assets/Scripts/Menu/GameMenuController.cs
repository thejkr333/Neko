using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    [SerializeField] GameObject settings, mainMenu;

    bool inMenu;
    // Start is called before the first frame update
    void Start()
    {
        settings.SetActive(false);
        inMenu = false;
        mainMenu.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        { 
            inMenu = !inMenu;
            Time.timeScale = inMenu ? 0 : 1;
            mainMenu.SetActive(inMenu);
            settings.SetActive(false);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                 Application.Quit();
#endif
    }

    public void ReturnToGame()
    {
        inMenu = false;
        Time.timeScale = 1;
        mainMenu.SetActive(false);
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

