using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationRandomizer : MonoBehaviour
{
    [SerializeField]
    List<TriggerItem> TriggerItems;
    [SerializeField]
    float RandomPickIntervalMS;
    [SerializeField]
    int MaxRepeatCount = 2;

    Animator Animator;
    TriggerItem CurrentState;
    float LastDrawTime;
    int RepeatCounter;

    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        TriggerItems = TriggerItems.SelectMany(x => Enumerable.Repeat(x, x.Count)).ToList();
        CurrentState = TriggerItems.FirstOrDefault();
    }

    void Update()
    {
        if (!TriggerItems.Any())
            return;

        if (CurrentState == null)
            return;

        if (LastDrawTime != 0 && Time.time - LastDrawTime < (RandomPickIntervalMS / 1000))
            return;

        NextRandomTrigger();
    }

    private void NextRandomTrigger()
    {
        LastDrawTime = Time.time;

        var trigger = TriggerItems.Where(x => (MaxRepeatCount == 0 && x.TriggerName != CurrentState.TriggerName) || MaxRepeatCount > 0).RandomElement();
        if (trigger == null)
            return;

        if (CurrentState.TriggerName == trigger.TriggerName)
        {
            RepeatCounter++;
            if (RepeatCounter >= MaxRepeatCount)
            {
                trigger = TriggerItems.Where(x => x.TriggerName != CurrentState.TriggerName).RandomElement();
                RepeatCounter = 0;
            }
        }
        else
            RepeatCounter = 0;

        CurrentState = trigger;

        foreach (var item in TriggerItems.DistinctBy(x => x.TriggerName))
            Animator.ResetTrigger(item.TriggerName);

        Animator.SetTrigger(CurrentState.TriggerName);
    }

    [Serializable]
    public class TriggerItem
    {
        public int Count;
        public string TriggerName;
    }
}
