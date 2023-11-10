using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSaveScreen : MonoBehaviour
{
    [SerializeField]
    Transform SavesContent;
    [SerializeField]
    GameObject SaveFilePrefab, NewSaveModal, PreviewContent, EmptyPreview;
    [SerializeField]
    InputField InputNewSaveName;

    [SerializeField]
    TextMeshProUGUI SaveTitleText, CurrentWaveText, TotalScoreText, MoneyText, PrimaryWeaponText, SecondaryWeaponText, HealthText,
        TotalTimeText, TotalInStoreTimeText, WavesRestartedText, TotalDeathsText, TotalEnemiesKilledText, TotalHeadshotKillsText, TotalPrecisionText;

    List<Animator> SavesAnimators = new();
    NZSave[] Saves;
    NZSave SelectedSave;

    MainMenuScreen MainMenu;

    void Start()
    {
        MainMenu = GetComponent<MainMenuScreen>();
        MainMenu.OnScreenChangedEvent += OnScreenOpened;
    }

    void Update()
    {
        if (MainMenu.ActiveScreen != MenuScreens.SelectSave)
            return;

        PreviewContent.SetActive(SelectedSave != null);
        EmptyPreview.SetActive(SelectedSave == null);

        if (SelectedSave != null)
            SavesAnimators[Array.IndexOf(Saves, SelectedSave)].SetTrigger("Selected");
    }

    /// <summary>
    /// Seleciona ou desseleciona um save.
    /// </summary>
    /// <param name="saveName">O nome do arquivo do save.</param>
    /// <param name="selected">Se está sendo selecionado ou não.</param>
    public void SelectSave(string saveName, bool selected)
    {
        if (!selected)
        {
            if (SelectedSave != null && Saves.Any() && SavesAnimators.Any())
            {
                var prevSelectedSaveAnimator = SavesAnimators[Array.IndexOf(Saves, SelectedSave)];
                prevSelectedSaveAnimator.ResetTrigger("Selected");
                prevSelectedSaveAnimator.SetTrigger("Unselect");
                SelectedSave = null;
            }
            return;
        }

        NZSave save = Saves.FirstOrDefault(s => s.FileName == saveName);
        SelectedSave = save;

        if (save == null)
            return;

        SaveTitleText.text = save.FileName;
        CurrentWaveText.text = save.CurrentWave.ToString("D2");
        TotalScoreText.text = save.WavesStats.Sum(x => x.Score).ToString("N0");
        MoneyText.text = $"$ {save.PlayerTotalMoney:N2}";

        var weaponSelection = save.PrimaryWeaponsSelection.Concat(save.SecondaryWeaponsSelection);
        var primaryType = weaponSelection.FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Primary)?.Type ?? WeaponTypes.None;
        var secondaryType = weaponSelection.FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Secondary)?.Type ?? WeaponTypes.None;

        PrimaryWeaponText.text = primaryType.ToString();
        SecondaryWeaponText.text = secondaryType.ToString();
        HealthText.text = save.PlayerHealth.ToString("N0");

        TimeSpan timeSpan = TimeSpan.FromSeconds(save.WavesStats.Sum(x => x.TimeTaken));
        TotalTimeText.text = $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s {timeSpan.Milliseconds:00}ms";

        TotalInStoreTimeText.text = "soon...";
        WavesRestartedText.text = save.WavesStats.Sum(x => x.RestartCount).ToString();
        TotalDeathsText.text = save.WavesStats.Sum(x => x.DeathCount).ToString();
        int enemiesKilled = save.WavesStats.Sum(x => x.EnemiesKilled);
        int headshotKills = save.WavesStats.Sum(x => x.HeadshotKills);
        TotalEnemiesKilledText.text = enemiesKilled.ToString();
        TotalHeadshotKillsText.text = $"{headshotKills} ({(headshotKills + enemiesKilled == 0 ? 0 : headshotKills / enemiesKilled * 100):N0}%)";
        TotalPrecisionText.text = $"{save.WavesStats.Sum(x => x.Precision) / save.WavesStats.Count:N1}%";
    }

    /// <summary>
    /// Volta para a tela anterior.
    /// </summary>
    public void GoBack()
    {
        MainMenu.OpenScreen(MenuScreens.SelectGameMode);
    }

    /// <summary>
    /// Continua o jogo com o progresso do save selecionado.
    /// </summary>
    public void ContinueGame()
    {
        if (SelectedSave == null)
            return;

        SavesManager.SelectedSaveName = SelectedSave.FileName;
        SavesManager.LoadSavedGame(GameModes.WaveMastery, SavesManager.SelectedSaveName);
        MenuController.Instance.IsTutorialActive = false;
        MenuController.Instance.ChangeScene(SceneNames.Store, LoadSceneMode.Single);
    }

    /// <summary>
    /// Exibe o modal para preencher o nome do novo save.
    /// </summary>
    public void OpenNewSaveModal()
    {
        NewSaveModal.SetActive(true);
        InputNewSaveName.text = $"Save {Saves.Length + 1:D2}";
    }

    /// <summary>
    /// Fecha o modal de preenchimento do nome do novo save.
    /// </summary>
    public void CloseNewSaveModal()
    {
        NewSaveModal.SetActive(false);
    }

    /// <summary>
    /// Cria um novo arquivo de save.
    /// </summary>
    public void CreateNewSave()
    {
        string saveName = InputNewSaveName.text;
        if (string.IsNullOrWhiteSpace(saveName) || saveName.Length > 40)
            return;

        SavesManager.SelectedSaveName = saveName;
        SavesManager.SaveGame(GameModes.WaveMastery, SavesManager.SelectedSaveName);
        if (MenuController.Instance.IsTutorialActive)
            MenuController.Instance.ChangeScene(SceneNames.Graveyard, LoadSceneMode.Single);
        else
            MenuController.Instance.ChangeScene(SceneNames.Store, LoadSceneMode.Single);
    }

    /// <summary>
    /// Exclui o arquivo de save selecionado.
    /// </summary>
    public void DeleteSave()
    {
        SavesManager.DeleteSave(SelectedSave);
        SelectedSave = null;
        LoadSaves();
    }

    /// <summary>
    /// Função chamada quando a tela é trocada.
    /// </summary>
    /// <param name="newScreen">A nova tela sendo aberta.</param>
    /// <param name="previousScreen">A tela anterior sendo fechada.</param>
    void OnScreenOpened(MenuScreens newScreen, MenuScreens previousScreen)
    {
        if (newScreen != MenuScreens.SelectSave)
            return;

        LoadSaves();
    }

    /// <summary>
    /// Carrega todos os saves do modo de jogo selecionado e os adiciona na tela.
    /// </summary>
    void LoadSaves()
    {
        foreach (Transform child in SavesContent)
            Destroy(child.gameObject);

        SavesAnimators.Clear();
        SelectedSave = null;

        Saves = SavesManager.ListSaves(GameModes.WaveMastery).ToArray();
        foreach (NZSave save in Saves)
        {
            GameObject saveObject = Instantiate(SaveFilePrefab, SavesContent);
            TextMeshProUGUI saveNameText = saveObject.transform.Find("LabelContainer").GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI waveNumberText = saveObject.transform.Find("Image/NumberContainer").GetComponentInChildren<TextMeshProUGUI>();
            Toggle toggle = saveObject.GetComponentInChildren<Toggle>();
            toggle.group = SavesContent.GetComponent<ToggleGroup>();
            toggle.onValueChanged.AddListener((selected) => SelectSave(save.FileName, selected));

            saveObject.name = save.FileName;
            saveNameText.text = save.FileName;
            waveNumberText.text = save.CurrentWave.ToString("D2");
            SavesAnimators.Add(saveObject.GetComponentInChildren<Animator>());
        }
    }
}
