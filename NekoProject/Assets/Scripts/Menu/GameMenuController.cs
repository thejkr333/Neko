////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameMenuController : MonoBehaviour
{
    [SerializeField] GameObject settings, mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        settings.SetActive(false);
        mainMenu.SetActive(false);
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
        Time.timeScale = 1;
        mainMenu.SetActive(false);
    }

    public void Settings()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settings.transform.GetChild(0).gameObject);
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenu.transform.GetChild(0).gameObject);
    }
}

