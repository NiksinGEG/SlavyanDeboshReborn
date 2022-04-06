using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Web;

public class MainMenuScript : MonoBehaviour
{
    public void StartBtnPressed()
    {
        GlobalVariables.Seed = new System.Random(300);

        SceneManager.LoadScene("SampleScene");
    }

    public void ExitBtnPressed()
    {
        Application.Quit();
    }
}
