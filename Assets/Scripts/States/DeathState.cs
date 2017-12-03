using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : AbsState
{
    public override void Enter(IStateMachineEntity entity)
    {
        GameManager.Instance.GameActive = false;
        GameManager.Instance.GameTimerActive = false;

        GameManager.Instance.GameCanvas.GetComponent<GameCanvas>().DeathGroup.SetActive(true);
    }

    public override void Update(IStateMachineEntity entity)
    {
        GameManager.Instance.PlayerCamera.LookAtSnowball();

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.ResetGame();
        }
    }

    public override void Exit(IStateMachineEntity entity)
    {

    }
}
