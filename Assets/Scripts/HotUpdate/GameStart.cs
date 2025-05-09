using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Start");
        Addressables.LoadSceneAsync("Assets/GameRes/Scene/Test.unity").Completed+=(handle)=> 
        {
            Debug.Log("Load Scene\"Test\"");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
