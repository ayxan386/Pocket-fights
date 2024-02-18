using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseTriggerEffect : MonoBehaviour
{
    [SerializeField] private float nextEffectDelay;
    [SerializeField] private UnityEvent<StatEffect, StatController> nextTriggerEffect;

    protected abstract void MainEffect(StatEffect baseEffect, StatController effectHolder);

    public void ApplyEffect(StatEffect baseEffect, StatController effectHolder)
    {
        StartCoroutine(MainEffectDelayThenNext(baseEffect, effectHolder));
    }

    private IEnumerator MainEffectDelayThenNext(StatEffect baseEffect, StatController effectHolder)
    {
        MainEffect(baseEffect, effectHolder);

        yield return new WaitForSeconds(nextEffectDelay);

        nextTriggerEffect?.Invoke(baseEffect, effectHolder);
    }
}