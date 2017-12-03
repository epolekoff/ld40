using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsState : IState {

    public virtual void Enter(IStateMachineEntity entity)
    {
        
    }

    public virtual void Exit(IStateMachineEntity entity)
    {
        
    }

    public virtual void Update(IStateMachineEntity entity)
    {
        
    }
}
