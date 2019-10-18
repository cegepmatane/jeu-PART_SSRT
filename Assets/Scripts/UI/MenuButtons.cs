using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("TestEnemyBehavior");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
