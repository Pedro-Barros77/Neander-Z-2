using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour
{
    [SerializeField]
    public AudioClip HoverSound, ClickSound;
    [SerializeField]
    public float HoverVolume = 1f, ClickVolume = 1f;

    private Animator animator;
    private Button button;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    /// <summary>
    /// Função chamada quando o botão é clicado.
    /// </summary>
    public void OnClick()
    {
        if (!button.interactable)
            return;

        if (HoverSound != null)
            audioSource.PlayOneShot(ClickSound, ClickVolume);
    }

    /// <summary>
    /// Função chamada quando o mouse passa por cima do botão.
    /// </summary>
    public void OnHover()
    {
        if (!button.interactable)
            return;

        if (HoverSound != null)
            audioSource.PlayOneShot(HoverSound, HoverVolume);
    }

    /// <summary>
    /// Reinicia o estado e animação do botão.
    /// </summary>
    /// <param name="btnAnimator">Botão a ser reiniciado.</param>
    public void ResetButton()
    {
        //animator.ResetTrigger("Highlighted");
        animator.ResetTrigger("Pressed");
        animator.ResetTrigger("Selected");
        animator.SetTrigger("Normal");
        animator.transform.localScale = new Vector3(1, 1, 1);
    }
}
