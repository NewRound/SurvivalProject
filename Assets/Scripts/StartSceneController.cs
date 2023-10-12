using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private GameObject SetUI;
    [SerializeField] private GameObject EndUI;

    [SerializeField] private string startSceneName = "SampleScene";

    private void Start()
    {
        SetUI.SetActive(false);
        EndUI.SetActive(false);
    }


    public void StartGame()
    {
        SceneManager.LoadScene(startSceneName);
    }

    public void SetUIEnter()
    {
        SetUI.SetActive(true);
    }

    public void SetUIExit()
    {
        SetUI.SetActive(false);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    // -----------------------------------------------

    public void OnEndMessage()
    {
        EndUI.SetActive(true);
    }
    public void OffEndMessage()
    {
        EndUI.SetActive(false);
    }
}
