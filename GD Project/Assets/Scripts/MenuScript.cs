using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Map1()
    {
        SceneManager.LoadScene("SceneMap1");
    }

    public void Map2()
    {
        SceneManager.LoadScene("SceneMap2");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
