using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSupplyPackage : MonoBehaviour
{
    public Player Player { get; set; }
    protected Animator Animator { get; set; }
    protected Rigidbody2D Rigidbody { get; set; }
    [SerializeField]
    CustomAudio CargoPlaneAudio, ParachuteOpenAudio, ParachuteWindLoopAudio, BagDropImpactAudio, BagOpenZiperAudio;
    [SerializeField]
    GameObject Arrows;
    [SerializeField]
    PlayerInteractTrigger InteractTrigger;

    protected AudioSource AudioSource;
    protected SpriteRenderer SpriteRenderer;
    float CurrentSpriteAlpha = 1;

    protected virtual void Start()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody2D>();
        Rigidbody.isKinematic = true;
        AudioSource = GetComponent<AudioSource>();
        SpriteRenderer = transform.Find("CargoBag").GetComponent<SpriteRenderer>();

        if (InteractTrigger != null)
        {
            InteractTrigger.gameObject.SetActive(false);
            InteractTrigger.OnPlayerInteracted += OnInteract;
        }
        if (Arrows != null)
            Arrows.SetActive(false);

        CargoPlaneAudio.PlayIfNotNull(AudioSource, AudioTypes.Player);
        StartCoroutine(OpenParachute());
    }

    protected virtual void Update()
    {

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
            OnTouchDown();
    }

    /// <summary>
    /// Função chamada quando o pacote toca o chão.
    /// </summary>
    protected virtual void OnTouchDown()
    {
        AudioSource.loop = false;
        AudioSource.Stop();
        BagDropImpactAudio.PlayIfNotNull(AudioSource, AudioTypes.Player);
        Animator.SetTrigger("TouchDown");
        Rigidbody.mass = 8;
        Rigidbody.drag = 0;
        if (InteractTrigger != null)
            InteractTrigger.gameObject.SetActive(true);
        if (Arrows != null)
            Arrows.SetActive(true);
    }

    /// <summary>
    /// Abre o paraquedas um tempo após o efeito sonoro de avião.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator OpenParachute()
    {
        yield return new WaitForSeconds(8);
        ParachuteOpenAudio.PlayIfNotNull(AudioSource, AudioTypes.Player);
        Rigidbody.isKinematic = false;
        AudioSource.loop = true;
        ParachuteWindLoopAudio.PlayIfNotNull(AudioSource, AudioTypes.Player, false);
    }

    /// <summary>
    /// Função chamada quando o jogador interage com o pacote.
    /// </summary>
    protected virtual void OnInteract()
    {
        BagOpenZiperAudio.PlayIfNotNull(AudioSource, AudioTypes.Player);
        if (InteractTrigger != null)
            InteractTrigger.gameObject.SetActive(false);
        if (Arrows != null)
            Arrows.SetActive(false);

        StartCoroutine(KillSelfDelayed());
    }

    /// <summary>
    /// Destroi o objeto após um tempo.
    /// </summary>
    protected IEnumerator KillSelfDelayed()
    {
        yield return new WaitForSeconds(2);

        while (CurrentSpriteAlpha > 0)
        {
            CurrentSpriteAlpha -= 10f * Time.deltaTime;

            Color spriteColor = SpriteRenderer.color;
            spriteColor.a = CurrentSpriteAlpha;
            SpriteRenderer.color = spriteColor;

            yield return new WaitForSeconds(0.2f);
        }

        Destroy(gameObject);
    }
}
