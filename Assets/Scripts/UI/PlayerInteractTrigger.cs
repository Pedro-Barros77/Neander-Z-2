using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractTrigger : MonoBehaviour
{
    public Action OnPlayerEnter { get; set; }
    public Action OnPlayerExit { get; set; }
    public Action OnPlayerInteracted { get; set; }

    public List<InputActions> InteractActions = new() { InputActions.Interact };
    public float InteractHoldTimeMs;
    public bool RepeatedlyInteractOnHold;
    public ProgressBar InteractProgressBar;
    public GameObject ObjectToShowWhenInRange;

    public bool IsPlayerInRange => playersInRange > 0;

    float interactTimeoutMs;
    int playersInRange;
    bool releasedInteract = true;

    void Start()
    {
        interactTimeoutMs = InteractHoldTimeMs;
        if (ObjectToShowWhenInRange != null)
            ObjectToShowWhenInRange.SetActive(false);
        if (InteractProgressBar != null)
        {
            InteractProgressBar.SetMaxValue(1, 1);
            InteractProgressBar.UseShadows = false;
            InteractProgressBar.UseOutline = false;
            InteractProgressBar.UseAnimation = false;
        }
    }

    void Update()
    {
        bool interacted = CheckInteraction();
        if (interacted)
            OnPlayerInteracted?.Invoke();

        UpdateUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !IsPlayerInRange)
            PlayerEntered();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playersInRange == 1)
            PlayerExited();
    }

    void PlayerEntered()
    {
        playersInRange++;
        if (ObjectToShowWhenInRange != null)
            ObjectToShowWhenInRange.SetActive(true);
        OnPlayerEnter?.Invoke();
    }

    void PlayerExited()
    {
        playersInRange--;
        if (ObjectToShowWhenInRange != null)
            ObjectToShowWhenInRange.SetActive(false);
        interactTimeoutMs = InteractHoldTimeMs;
        OnPlayerExit?.Invoke();
    }

    bool CheckInteraction()
    {
        if (!IsPlayerInRange)
            return false;

        bool interacted = false;
        if (InteractHoldTimeMs == 0)
        {
            interacted = InteractActions.Any(x => RepeatedlyInteractOnHold ? Constants.GetAction(x) : Constants.GetActionDown(x));
            return interacted;
        }

        if (InteractActions.Any(x => Constants.GetAction(x)))
        {
            if (releasedInteract || RepeatedlyInteractOnHold)
            {
                interactTimeoutMs -= Time.deltaTime * 1000;

                if (interactTimeoutMs <= 0)
                {
                    interacted = true;
                    if (!RepeatedlyInteractOnHold)
                    {
                        interactTimeoutMs = 0;
                        releasedInteract = false;
                    }
                }
            }
        }
        else
        {
            interactTimeoutMs = InteractHoldTimeMs;
            releasedInteract = true;
        }

        return interacted;
    }

    void UpdateUI()
    {
        if (InteractHoldTimeMs > 0 && InteractProgressBar != null)
        {
            float interactProgress = (InteractHoldTimeMs - interactTimeoutMs) / InteractHoldTimeMs;
            InteractProgressBar.RemoveValue(1);
            InteractProgressBar.AddValue(interactProgress);
        }
    }
}
