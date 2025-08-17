using System;
using Unity.Behavior;

[BlackboardEnum]
public enum State
{
    Idle,
    Approaching,
    CloseRange,
    MidRange,
    LongRange,
    SpecialAttacks,
    Running,
    FutureSight,
    Testing
}
