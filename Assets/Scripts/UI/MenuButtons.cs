using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Level");
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("Menu");
        //Debug.Log("AAAAAAAAAAAAAAAA");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
