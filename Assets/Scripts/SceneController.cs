using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject CanvasMain;
    [SerializeField] private GameObject CanvasGameOver;
    [SerializeField] private GameObject CanvasGameUI;
    [SerializeField] private GameObject CanvasAds;
    
    private static SceneController _instance;
    public static SceneController Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    public void GoMainScene()
    {
        GameManager.Instance.goat = GameManager.Instance.SpawnGoat();
        CanvasGameOver.SetActive(false);
        CanvasGameUI.SetActive(false);
        CanvasMain.SetActive(true);
    }

    public void GoInGameScene()
    {
        CanvasGameOver.SetActive(false);
        CanvasGameUI.SetActive(true);
        CanvasMain.SetActive(false);
    }

    public void GoGameOverScene()
    {
        CanvasGameOver.SetActive(true);
        CanvasGameUI.SetActive(false);
        CanvasMain.SetActive(false);
    }

    public void ShowCanvasAds()
    {
        CanvasAds.SetActive(true);
    }
}
