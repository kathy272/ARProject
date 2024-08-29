using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.UI;

public class SceneLoader: MonoBehaviour
{
    [SerializeField] private string _sceneName;
    [SerializeField] private Button ContinueButton;



    void Start()
    {
        ContinueButton.onClick.AddListener(OnContinueButtonClick);
    }

    void OnContinueButtonClick()
    {
        LoadScene();
    }
    public void LoadScene()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneName);
        Debug.Log("Scene loaded");
    }
}