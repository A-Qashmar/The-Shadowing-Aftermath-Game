using UnityEngine;
using UnityEngine.Events;

public class animationHelper : MonoBehaviour
{
    public UnityEvent onAnimeationEventTrigger, onAttackPeformed;

    public void AnimationEventTrigger()
    {
        onAnimeationEventTrigger?.Invoke();
    }

    public void TriggerAttack()
    {
        onAttackPeformed?.Invoke();
    }
}
