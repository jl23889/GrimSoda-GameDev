using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GMScript : MonoBehaviour {

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button reset;
    public Text pauseText;
    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;
    private CharacterManager p1cm;
    private CharacterManager p2cm;
    private CharacterManager p3cm;
    private CharacterManager p4cm;
    private bool gameOver;

    // Use this for initialization
    void Start() {
        p1cm = p1.GetComponent<CharacterManager>();
        p2cm = p2.GetComponent<CharacterManager>();
        p3cm = p3.GetComponent<CharacterManager>();
        p4cm = p4.GetComponent<CharacterManager>();
        gameOver = false;
        pauseMenu.SetActive(false);
        reset.onClick.AddListener(ResetGame);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeInHierarchy)
            {
                pauseText.text = "PAUSED";
                PauseGame();
            } else if (!gameOver)
            {
                ResumeGame();
            }
        }
        if (p2cm.IsDead && p3cm.IsDead && p4cm.IsDead)
        {
            pauseText.text = "PLAYER 1 WINS!";
            WinGame();
        }
        else if(p1cm.IsDead && p3cm.IsDead && p4cm.IsDead)
        {
            pauseText.text = "PLAYER 2 WINS!";
            WinGame();
        }
        else if (p1cm.IsDead && p2cm.IsDead && p4cm.IsDead)
        {
            pauseText.text = "PLAYER 3 WINS!";
            WinGame();
        }
        else if (p1cm.IsDead && p2cm.IsDead && p3cm.IsDead)
        {
            pauseText.text = "PLAYER 4 WINS!";
            WinGame();
        }
    }

    void WinGame()
    {
        gameOver = true;
        pauseMenu.SetActive(true);
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ResumeGame();
    }
}
