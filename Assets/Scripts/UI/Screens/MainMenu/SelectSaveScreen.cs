using System;
using System.Collections.Generic;
using System.IO;
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
    GameObject SaveFilePrefab, PreviewContent, EmptyPreview, PopupPrefab;
    [SerializeField]
    Canvas Canvas;

    [SerializeField]
    TextMeshProUGUI SaveTitleText, CurrentWaveText, TotalScoreText, MoneyText, PrimaryWeaponText, SecondaryWeaponText, HealthText,
        TotalTimeText, TotalInStoreTimeText, WavesRestartedText, TotalDeathsText, TotalEnemiesKilledText, TotalHeadshotKillsText, TotalPrecisionText;

    [SerializeField]
    BaseButton BtnDeleteAll, BtnImportSave, BtnExportSave;

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
        if (BtnDeleteAll?.Button != null)
            BtnDeleteAll.Button.interactable = Saves.Any();

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

        var weaponSelection = save.WeaponsSelection;
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
        int headshotPrecision = 0;
        if (headshotKills + enemiesKilled > 0)
            headshotPrecision = (int)((float)headshotKills / enemiesKilled * 100);
        TotalHeadshotKillsText.text = $"{headshotKills} ({headshotPrecision:N0}%)";
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
        CustomDialog.Open(new()
        {
            BtnConfirmText = "Create",
            UseCancelButton = false,
            PromptText = "Enter a save name.",
            UseInputField = true,
            InputFieldPlaceholderText = "Save name",
            InputFieldText = $"Save {Saves.Length + 1:D2}",
            OnConfirm = (text) =>
            {
                CreateNewSave(text);
            },
        });
    }

    /// <summary>
    /// Cria um novo arquivo de save.
    /// </summary>
    public void CreateNewSave(string saveName)
    {
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
        CustomDialog.Open(new()
        {
            BtnConfirmText = "Yes",
            PromptText = "Are you sure you want to delete this game save?",
            OnConfirm = (_) =>
            {
                SavesManager.DeleteSave(SelectedSave);
                SelectedSave = null;
                LoadSaves();
            },
        });

    }

    /// <summary>
    /// Exclui todos os arquivos de save.
    /// </summary>
    public void DeleteAllSaves()
    {
        CustomDialog.Open(new()
        {
            BtnConfirmText = "Yes",
            PromptText = "Are you sure you want to delete all game saves?",
            OnConfirm = (_) =>
            {
                foreach (NZSave save in Saves)
                    SavesManager.DeleteSave(save);

                SelectedSave = null;
                LoadSaves();
            },
        });
    }

    /// <summary>
    /// Importa um arquivo de save a partir do caminho especificado.
    /// </summary>
    public void ImportSave()
    {
        CustomDialog.Open(new()
        {
            UseCancelButton = false,
            DialogSize = new Vector2(400, 150),
            BtnConfirmText = "Import",
            BtnConfirmTooltipText = "Import a save file from your computer to the game.",
            PromptText = "Copy the file path and paste it here:",
            UseInputField = true,
            InputFieldPlaceholderText = "Ex: C:\\Users\\YourName\\Downloads\\Save 01.nzsave",
            OnConfirm = (text) =>
            {
                if (!text.EndsWith(".nzsave"))
                {
                    ShowPopup("Invalid file format!", Constants.Colors.RedMoney, BtnImportSave.transform.position + new Vector3(-70, 60));
                    return;
                }

                NZSave importedSave = SavesManager.ImportNzSave(text);

                if (importedSave == null)
                {
                    ShowPopup("Failed to import the file!", Constants.Colors.RedMoney, BtnImportSave.transform.position + new Vector3(-70, 60));
                    return;
                }

                bool saved = SavesManager.SaveNzSave(importedSave, GameModes.WaveMastery, Path.GetFileNameWithoutExtension(Path.GetFileName(text)));

                if (!saved)
                {
                    ShowPopup("Failed to save the file!", Constants.Colors.RedMoney, BtnImportSave.transform.position + new Vector3(-70, 60));
                    return;
                }

                SelectedSave = null;
                LoadSaves();
                ShowPopup("Save file successfully imported!", Constants.Colors.GreenMoney, BtnImportSave.transform.position + new Vector3(-70, 60));
            },
        });
    }

    /// <summary>
    /// Exporta o arquivo de save selecionado para a pasta downloads.
    /// </summary>
    public void ExportSave()
    {
        bool exported = SavesManager.ExportNzSave(SelectedSave);
        if (exported)
            ShowPopup("Save file successfully saved in Downloads folder!", Constants.Colors.GreenMoney, BtnExportSave.transform.position + new Vector3(-70, 50));
        else
            ShowPopup("Failed to save the file!", Constants.Colors.RedMoney, BtnExportSave.transform.position + new Vector3(-70, 50));
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

    /// <summary>
    /// Exibe um Popup na tela.
    /// </summary>
    /// <param name="text">O texto a ser exibido.</param>
    /// <param name="textColor">A cor do texto a ser exibido.</param>
    /// <param name="position">A posição do texto a ser exibido.</param>
    private void ShowPopup(string text, Color32 textColor, Vector3 position, float durationMs = 4000, float scale = 40)
    {
        var popup = Instantiate(PopupPrefab, position, Quaternion.identity, Canvas.transform);
        var popupSystem = popup.GetComponent<PopupSystem>();
        if (popupSystem != null)
        {
            popupSystem.Init(text, position, durationMs, textColor, scale);
        }
    }
}
