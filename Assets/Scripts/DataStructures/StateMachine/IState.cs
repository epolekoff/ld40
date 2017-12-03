using UnityEngine;
using System.Collections;

public interface IState {

    /// <summary>
    /// Run once when entering the state for the first time.
    /// </summary>
    void Enter(IStateMachineEntity entity);

    /// <summary>
    /// Called once per update loop of the game when in this state.
    /// </summary>
    void Update(IStateMachineEntity entity);

    /// <summary>
    /// Run once when leaving the state.
    /// </summary>
    void Exit(IStateMachineEntity entity);
}
