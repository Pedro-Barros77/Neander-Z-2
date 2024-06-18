using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BodyTarget : MonoBehaviour, IPlayerTarget
{
    public bool IsAlive => true;
    public GameObject SparksPrefab;
    public List<CustomAudio> HitSounds;


    AudioSource AudioSource;
    SpriteRenderer SpriteRenderer;
    Transform EffectsContainer;
    Canvas WorldPosCanvas;
    GameObject PopupPrefab;
    float lastSparkTime;
    float sparksDelay = 0.03f;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        EffectsContainer = GameObject.Find("EffectsContainer").transform;
        PopupPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup");
        WorldPosCanvas = GameObject.Find("WorldPositionCanvas").GetComponent<Canvas>();
    }

    void Update()
    {

    }

    public void OnPointHit(TakeDamageProps props)
    {
        if (SparksPrefab == null)
            return;

        if (lastSparkTime + sparksDelay > Time.time)
            return;

        var sparks = Instantiate(SparksPrefab, props.HitPosition.Value, Quaternion.identity, EffectsContainer);
        sparks.transform.up = props.HitEffectDirection.Value;
        lastSparkTime = Time.time;
    }

    public void TakeDamage(TakeDamageProps props)
    {
        if (props.Damage < 0) return;

        Color32 color;
        float damage = props.Damage;

        switch (props.BodyPartName)
        {
            case "Head":
                damage *= props.HeadshotMultiplier;
                color = Color.red;
                break;

            case "Plate":
                color = Color.white;
                damage = 0;
                break;

            default:
                color = Color.yellow;
                break;
        }

        ShowPopup(damage.ToString("N1"), color, props.HitPosition ?? transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 2)); ;

        HitSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);

        if(props.HitEffectDirection != null)
            OnPointHit(props);
    }

    public virtual void HandleSpriteColorChange(Color32 color)
    {
        SpriteRenderer.material.SetColor("_Color", color);
    }

    /// <summary>
    /// Função para exibir o popup com devidos parâmetros.
    /// </summary>
    /// <param name="text">Texto a ser exibido</param>
    /// <param name="textColor">A cor que o popup vai ser exibido</param>
    /// <param name="hitPosition">A posição que o popup vai aparecer</param>
    private void ShowPopup(string text, Color32 textColor, Vector3 hitPosition)
    {
        var popup = Instantiate(PopupPrefab, hitPosition, Quaternion.identity, WorldPosCanvas.transform);
        var popupSystem = popup.GetComponent<PopupSystem>();
        if (popupSystem != null)
        {
            popupSystem.Init(text, hitPosition, 2000f, textColor);
        }
    }

    public void GetHealth(float health) => throw new System.NotImplementedException();
}
