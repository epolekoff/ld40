using UnityEngine;
using System.Collections;

public interface IStateMachineEntity {

    /// <summary>
    /// Get the state machine from this object so we can change states.
    /// </summary>
    /// <returns></returns>
    FiniteStateMachine GetStateMachine(int number = 0);
}
