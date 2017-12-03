using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : AbsState
{
    public override void Enter(IStateMachineEntity entity)
    {
        GameManager.Instance.GameActive = false;

        GameManager.Instance.MenuWorldSpaceCanvas.SetActive(true);
        GameManager.Instance.MenuWorldSpaceCamera.SetActive(true);
        GameManager.Instance.MenuScreenSpaceCanvas.SetActive(true);

        GameManager.Instance.GameCanvas.SetActive(false);
    }

    public override void Update(IStateMachineEntity entity)
    {
        GameManager.Instance.PlayerCamera.ShowStartingPlatform();

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.StartGame();
        }

    }

    public override void Exit(IStateMachineEntity entity)
    {
        GameManager.Instance.MenuWorldSpaceCanvas.SetActive(false);
        GameManager.Instance.MenuWorldSpaceCamera.SetActive(false);
        GameManager.Instance.MenuScreenSpaceCanvas.SetActive(false);
    }
}
