using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public GameObject MainCanvasUI;
    public Camera MainCamera;
    public Camera UICamera;
    private void Awake()
    {
        DontDestroyOnLoad(MainCanvasUI);
        DontDestroyOnLoad(MainCamera);
        DontDestroyOnLoad(UICamera);
    }
    public bool HasNetwork()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
