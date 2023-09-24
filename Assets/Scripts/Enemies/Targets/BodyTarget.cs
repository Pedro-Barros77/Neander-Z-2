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

    public void OnPointHit(Vector3 hitPoint, Vector3 pointToDirection, string bodyPartName)
    {
        if (SparksPrefab == null)
            return;

        if (lastSparkTime + sparksDelay > Time.time)
            return;

        var sparks = Instantiate(SparksPrefab, hitPoint, Quaternion.identity, EffectsContainer);
        sparks.transform.up = pointToDirection;
        lastSparkTime = Time.time;
    }

    public void TakeDamage(float value, string bodyPartName, IEnemyTarget attacker, Vector3? hitPosition = null)
    {
        if (value < 0) return;

        Color32 color;

        switch (bodyPartName)
        {
            case "Head":
                //value *= HeadshotDamageMultiplier;
                color = Color.red;
                break;

            case "Plate":
                color = Color.white;
                value = 0;
                break;

            default:
                color = Color.yellow;
                break;
        }

        ShowPopup(value.ToString("N1"), color, hitPosition ?? transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 2)); ;

        if (HitSounds.Any())
        {
            var randomHitSound = HitSounds[Random.Range(0, HitSounds.Count)];
            AudioSource.PlayOneShot(randomHitSound.Audio, randomHitSound.Volume);
        }
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
}
