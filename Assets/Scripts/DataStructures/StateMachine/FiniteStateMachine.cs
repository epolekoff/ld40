using UnityEngine;
using System.Collections;

public class FiniteStateMachine {

    protected IState currentState;
    protected IState previousState;
    protected IState globalState;

    protected IStateMachineEntity owner;
	
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="initialState">The first state.</param>
    public FiniteStateMachine(IState initialState, IStateMachineEntity owner)
    {
        currentState = initialState;
        this.owner = owner;

        currentState.Enter(owner);
    }

    /// <summary>
    /// Update the entire state machine each cycle.
    /// </summary>
	public void Update () {

        //Update the current state each cycle.
        currentState.Update(owner);

        if(globalState != null)
        {
            globalState.Update(owner);
        }
	}


    /// <summary>
    /// Change to a new state.
    /// </summary>
    /// <param name="newState">The new state.</param>
    public void ChangeState(IState newState)
    {
        //Exit the old state.
        currentState.Exit(owner);

        //Swap states
        previousState = currentState;
        currentState = newState;

        //Enter the new state.
        currentState.Enter(owner);
    }

    /// <summary>
    /// Setter for the global state.
    /// </summary>
    /// <param name="globalState">The new global state.</param>
    public void SetGlobalState(IState globalState)
    {
        this.globalState = globalState;
    }
}
