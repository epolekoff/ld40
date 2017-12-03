using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : AbsState
{
    public override void Enter(IStateMachineEntity entity)
    {
        GameManager.Instance.GameActive = true;
        GameManager.Instance.GameTimerActive = false;

        GameManager.Instance.GameCanvas.SetActive(true);
        GameManager.Instance.ControlsGroup.SetActive(true);
    }

    public override void Update(IStateMachineEntity entity)
    {
        GameManager.Instance.PlayerCamera.FollowSnowball();

        if(GameManager.Instance.GameTimerActive)
        {
            GameManager.Instance.GameTimer += Time.deltaTime;
        }
    }

    public override void Exit(IStateMachineEntity entity)
    {

    }
}
