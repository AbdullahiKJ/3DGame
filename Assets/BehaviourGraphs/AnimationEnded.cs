using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AnimationEnded")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AnimationEnded", message: "Animation has ended or been interrupted", category: "Events", id: "d65fa2a4f6d96cacdc1e3f76956ae01e")]
public partial class AnimationEnded : EventChannelBase
{
    public delegate void AnimationEndedEventHandler();
    public event AnimationEndedEventHandler Event; 

    public void SendEventMessage()
    {
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        AnimationEndedEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as AnimationEndedEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as AnimationEndedEventHandler;
    }
}

