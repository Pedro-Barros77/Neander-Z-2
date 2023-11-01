using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    public OnGoBack GoBackFunction { get; set; }
    public delegate void OnGoBack();

    [SerializeField]
    TextMeshProUGUI TitleText;
    GameObject MainOptionsContent, AudioContent;
    Slider MusicVolumeSlider, UIVolumeSlider, PlayerVolumeSlider, EnemiesVolumeSlider;
    GameObject MusicVolumeCutLine, UIVolumeCutLine, PlayerVolumeCutLine, EnemiesVolumeCutLine;
    bool initialized = false;

    ActiveOptionsContent ActiveTab = ActiveOptionsContent.MainOptions;

    private enum ActiveOptionsContent
    {
        MainOptions,
        AudioOptions
    }

    void Start()
    {
        MainOptionsContent = transform.Find("MainOptions").gameObject;
        AudioContent = transform.Find("AudioOptions").gameObject;

        var slidersContainer = AudioContent.transform.Find("SlidersContainer");

        MusicVolumeSlider = slidersContainer.Find("MusicAudio").GetComponentInChildren<Slider>();
        UIVolumeSlider = slidersContainer.Find("UIAudio").GetComponentInChildren<Slider>();
        PlayerVolumeSlider = slidersContainer.Find("PlayerAudio").GetComponentInChildren<Slider>();
        EnemiesVolumeSlider = slidersContainer.Find("EnemiesAudio").GetComponentInChildren<Slider>();

        MusicVolumeCutLine = slidersContainer.Find("MusicAudio").Find("Icon").GetChild(0).gameObject;
        UIVolumeCutLine = slidersContainer.Find("UIAudio").Find("Icon").GetChild(0).gameObject;
        PlayerVolumeCutLine = slidersContainer.Find("PlayerAudio").Find("Icon").GetChild(0).gameObject;
        EnemiesVolumeCutLine = slidersContainer.Find("EnemiesAudio").Find("Icon").GetChild(0).gameObject;

        initialized = true;
        Open();
    }

    void Update()
    {
        MenuController.Instance.MusicVolume = MusicVolumeSlider.value;
        MenuController.Instance.UIVolume = UIVolumeSlider.value;
        MenuController.Instance.PlayerVolume = PlayerVolumeSlider.value;
        MenuController.Instance.EnemiesVolume = EnemiesVolumeSlider.value;

        MusicVolumeCutLine.SetActive(MusicVolumeSlider.value == 0);
        UIVolumeCutLine.SetActive(UIVolumeSlider.value == 0);
        PlayerVolumeCutLine.SetActive(PlayerVolumeSlider.value == 0);
        EnemiesVolumeCutLine.SetActive(EnemiesVolumeSlider.value == 0);
    }

    public void Open()
    {
        if (!initialized)
            return;

        SetTab(ActiveOptionsContent.MainOptions);
        SetSliders();
    }

    public void OpenAudioOptions()
    {
        SetTab(ActiveOptionsContent.AudioOptions);
    }

    public void GoBack()
    {
        if (!initialized)
            return;

        switch (ActiveTab)
        {
            case ActiveOptionsContent.MainOptions:
                SetTab(ActiveOptionsContent.MainOptions);
                GoBackFunction?.Invoke();
                break;

            case ActiveOptionsContent.AudioOptions:
                SetTab(ActiveOptionsContent.MainOptions);
                break;
        }
    }

    void SetTab(ActiveOptionsContent newTab)
    {
        ActiveTab = newTab;
        MenuController.Instance.SetCursor(Cursors.Arrow);
        MainOptionsContent.SetActive(ActiveTab == ActiveOptionsContent.MainOptions);
        AudioContent.SetActive(ActiveTab == ActiveOptionsContent.AudioOptions);

        if (TitleText != null)
            TitleText.text = ActiveTab switch
            {
                ActiveOptionsContent.MainOptions => "Options",
                ActiveOptionsContent.AudioOptions => "Audio",
                _ => "Options"
            };
    }

    void SetSliders()
    {
        MusicVolumeSlider.value = MenuController.Instance.MusicVolume;
        UIVolumeSlider.value = MenuController.Instance.UIVolume;
        PlayerVolumeSlider.value = MenuController.Instance.PlayerVolume;
        EnemiesVolumeSlider.value = MenuController.Instance.EnemiesVolume;
    }
}
