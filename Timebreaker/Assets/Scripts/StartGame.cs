using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {
    
    [SerializeField] private Button start;

    // Use this for initialization
    void Start()
    {
        start.onClick.AddListener(BeginGame);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void BeginGame()
    {
        Debug.Log("CLICK");
        SceneManager.LoadScene("TrainStation", LoadSceneMode.Single);
    }
}
