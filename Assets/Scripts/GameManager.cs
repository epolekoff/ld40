using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IStateMachineEntity
{
    private FiniteStateMachine m_stateMachine;

    public bool GameActive { get; set; }
    public bool GameTimerActive { get; set; }
    public float GameTimer { get; set; }

    public Snowball Snowball;
    public Transform SnowballSpawnPoint;
    public PlayerCamera PlayerCamera;

    public GameObject MenuWorldSpaceCanvas;
    public GameObject MenuWorldSpaceCamera;
    public GameObject MenuScreenSpaceCanvas;
    public GameObject GameCanvas;
    public GameObject ControlsGroup;


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        m_stateMachine = new FiniteStateMachine(new MenuState(), this);
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        m_stateMachine.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    /// <summary>
    /// Getter
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public FiniteStateMachine GetStateMachine(int number = 0)
    {
        return m_stateMachine;
    }

    /// <summary>
    /// Quit
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Start playing
    /// </summary>
    public void StartGame()
    {
        m_stateMachine.ChangeState(new GameState());
    }

    /// <summary>
    /// Reset the game.
    /// </summary>
    public void ResetGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Called when the snowball leaves the starting platform.
    /// </summary>
    public void LeavePlatform()
    {
        ControlsGroup.SetActive(false);
        GameTimer = 0;
        GameActive = true;
        GameTimerActive = true;

        AudioManager.Instance.PlayBell();
    }

    /// <summary>
    /// Lose
    /// </summary>
    public void KillPlayer()
    {
        if(GameManager.Instance.GameTimerActive)
        {
            m_stateMachine.ChangeState(new DeathState());
        }
    }

    /// <summary>
    /// Win
    /// </summary>
    public void Victory()
    {
        GameManager.Instance.GameTimerActive = false;

        GameCanvas.GetComponent<GameCanvas>().VictoryGroup.SetActive(true);
    }
}
