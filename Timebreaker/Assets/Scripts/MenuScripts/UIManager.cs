using UnityEngine;
using UnityEngine.Playables;


/*/////////////////////////////////////////////////////////////////
    Intro --> Title <--> Menu <--> ArenaMenu <--> Game
                        /
                       RaidMenu <--> Game

/////////////////////////////////////////////////////////////////*/
public class UIManager : MonoBehaviour {
    
    // Menu animations
    public PlayableDirector Intro;
    public PlayableDirector Title;
    public PlayableDirector Menu;
    public PlayableDirector Selection;
    public PlayableDirector Raid;

    // Panels controlled
    public GameObject playPanel;
    public GameObject titlePanel;
    public GameObject introPanel;
    public GameObject menuPanel;

    private static GameObject thisCanvas;

    // INIT
    void Awake ()
    {

        thisCanvas = gameObject;
        // Since this is the first expected scene, we set the set the initial state here
        if (GM.manager.state == GM.GameState.UNASSIGNED)
        {
            GM.manager.state = GM.GameState.ONINTRO;
        }
        // TODO: see the effect of disabling all these child elements of Canvas
        for (int i=0; i< thisCanvas.transform.childCount; i++)
        {
            var childObj = thisCanvas.transform.GetChild(i).gameObject;
            if (childObj != null)
            {
                childObj.SetActive(false);
            }
        }
    }

    // Start Menu Scene
    void Start ()
    {
        if (GM.manager.state == GM.GameState.ONINTRO)
            showIntroPanel();
    }

    private void EndOfTimelineListener(PlayableDirector obj)
    {
        // Different callbacks after timelines are finished
        switch (GM.manager.state)
        {
            case GM.GameState.ONINTRO:
            {
                GM.manager.state = GM.GameState.ONTITLE;
                introPanel.SetActive(false);
                playPanel.SetActive(true);
                break;
            }
            case GM.GameState.ONTITLE:
            {
                GM.manager.state = GM.GameState.ONMAINMENU;
                playPanel.SetActive(false);
                break;
            }
        }
    }


    private void showIntroPanel()
    {
        if (introPanel && Intro)
        {
            introPanel.SetActive(true);
            titlePanel.SetActive(true);
            Intro.Play();
            Intro.stopped += EndOfTimelineListener;
        }
    }


    public void showMenu()
    {
        if (menuPanel && Menu)
        {
            menuPanel.SetActive(true);
            Menu.Play();
            Menu.stopped += EndOfTimelineListener;
        }
    }

    
}
