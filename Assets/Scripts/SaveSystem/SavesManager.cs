using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SavesManager
{
    /// <summary>
    /// O nome do arquivo de save selecionado, que o jogador está jogando atualmente.
    /// </summary>
    public static string SelectedSaveName { get; set; }

    /// <summary>
    /// Salva o progresso do jogo atual.
    /// </summary>
    /// <param name="gameMode">O modo de jogo a ser salvo.</param>
    /// <param name="fileName">O nome do arquivo a ser salvo.</param>
    /// <param name="encrypted">Se o arquivo deve ser criptografado ou não.</param>
    /// <returns>True se o arquivo foi salvo com sucesso.</returns>
    public static bool SaveGame(GameModes gameMode, string fileName, bool encrypted = false)
    {
        JsonSaveService jsonService = new();
        PlayerData player = GetPlayerSO();
        InventoryData inventory = GetInventorySO();
        NZSave save = Seed(new(), player, inventory);
        save.FileName = fileName;
        save.FolderPath = jsonService.CombinePaths(jsonService.ROOT_FOLDER, gameMode.ToString());

        if (jsonService.SaveData(gameMode.ToString(), fileName, save, encrypted))
            return true;
        return false;
    }

    /// <summary>
    /// Salva as preferências do jogador.
    /// </summary>
    /// <returns>True se o arquivo foi salvo com sucesso.</returns>
    public static bool SavePrefs()
    {
        JsonSaveService jsonService = new();
        PrefsSave prefs = new()
        {
            //Options
            MusicVolume = MenuController.Instance.MusicVolume,
            InterfaceVolume = MenuController.Instance.UIVolume,
            PlayerVolume = MenuController.Instance.PlayerVolume,
            EnemiesVolume = MenuController.Instance.EnemiesVolume,
            InputMode = MenuController.Instance.IsMobileInput ? 1 : 0,
            FileName = "Preferences",
            FolderPath = jsonService.ROOT_FOLDER
        };

        if (jsonService.SaveData("", "Preferences", prefs, false, "json"))
            return true;
        return false;
    }

    /// <summary>
    /// Lista todos os saves de um modo de jogo.
    /// </summary>
    /// <param name="gameMode">O modo de jogo dos saves a serem carregados.</param>
    /// <param name="encrypted">Se os arquivos foram criptografados.</param>
    /// <returns>Uma lista contendo todos os saves carregados.</returns>
    public static IEnumerable<NZSave> ListSaves(GameModes gameMode, bool encrypted = false)
    {
        JsonSaveService jsonService = new();
        string folderPath = jsonService.CombinePaths(jsonService.ROOT_FOLDER, gameMode.ToString());
        var filePaths = Directory.GetFiles(folderPath);

        NZSave LoadSaveWithFileName(string fullFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullFilePath);
            var save = jsonService.LoadData<NZSave>(folderPath, fileName, encrypted);
            save.FileName = fileName;
            save.FolderPath = folderPath;
            return save;
        }

        var saves = filePaths.Select(LoadSaveWithFileName);
        return saves;
    }

    /// <summary>
    /// Carrega o progresso de um save para dentro do jogo.
    /// </summary>
    /// <param name="gameMode">O modo de jogo do save a ser carregado.</param>
    /// <param name="fileName">O nome do arquivo do save.</param>
    /// <param name="encrypted">Se o arquivo foi criptografado.</param>
    public static void LoadSavedGame(GameModes gameMode, string fileName, bool encrypted = false)
    {
        JsonSaveService jsonService = new();
        string folderPath = jsonService.CombinePaths(jsonService.ROOT_FOLDER, gameMode.ToString());
        var save = jsonService.LoadData<NZSave>(folderPath, fileName, encrypted);
        if(save == null)
        {
            Debug.LogError($"Save file {fileName} not found!");
            return;
        }

        save.FileName = fileName;
        save.FolderPath = folderPath;
        UnSeed(save, GetPlayerSO(), GetInventorySO());
    }

    /// <summary>
    /// Carrega as preferências do jogador para dentro do jogo.
    /// </summary>
    public static void LoadSavedPrefs()
    {
        JsonSaveService jsonService = new();
        string folderPath = jsonService.ROOT_FOLDER;
        var prefs = jsonService.LoadData<PrefsSave>(folderPath, "Preferences", false, "json");
        if (prefs == null)
        {
            Debug.LogError($"Preferences Save file not found!");
            return;
        }

        prefs.FileName = "Preferences";
        prefs.FolderPath = folderPath;

        //Options
        MenuController.Instance.MusicVolume = prefs.MusicVolume;
        MenuController.Instance.UIVolume = prefs.InterfaceVolume;
        MenuController.Instance.PlayerVolume = prefs.PlayerVolume;
        MenuController.Instance.EnemiesVolume = prefs.EnemiesVolume;
        MenuController.Instance.IsMobileInput = prefs.InputMode == 1;
    }

    /// <summary>
    /// Exclui um arquivo de save.
    /// </summary>
    /// <param name="folderRelativePath">O caminho relativo da pasta do save.</param>
    /// <param name="fileName">O nome do save.</param>
    public static void DeleteSave(string folderRelativePath, string fileName)
    {
        JsonSaveService jsonService = new();
        string path = jsonService.CombinePaths(jsonService.ROOT_FOLDER, folderRelativePath, $"{fileName}.{jsonService.SAVE_EXTENSION}");
        File.Delete(path);
    }

    /// <summary>
    /// Exclui um arquivo de save.
    /// </summary>
    /// <param name="save">O save a ser excluído.</param>
    public static void DeleteSave(NZSave save)
    {
        JsonSaveService jsonService = new();
        string path = jsonService.CombinePaths(save.FolderPath, $"{save.FileName}.{jsonService.SAVE_EXTENSION}");
        File.Delete(path);
    }

    /// <summary>
    /// Preenche as informações de um save de acordo com o estado do jogo atual.
    /// </summary>
    /// <param name="save">O save a ser populado.</param>
    /// <param name="player">O ScriptableObject do jogador.</param>
    /// <param name="inventory">O ScriptableObject do inventário.</param>
    /// <returns></returns>
    private static NZSave Seed(NZSave save, PlayerData player, InventoryData inventory)
    {
        //Player
        save.CurrentWave = player.CurrentWaveIndex;
        save.PlayerTotalMoney = player.Money;
        save.PlayerHealth = player.Health;
        save.TotalScore = player.Score;
        save.MaxHealthUpgradeIndex = player.MaxHealthUpgradeIndex;
        save.MovementSpeedUpgradeIndex = player.MovementSpeedUpgradeIndex;
        save.SprintSpeedUpgradeIndex = player.SprintSpeedUpgradeIndex;
        save.JumpForceUpgradeIndex = player.JumpForceUpgradeIndex;
        save.MaxStaminaUpgradeIndex = player.MaxStaminaUpgradeIndex;
        save.StaminaRegenUpgradeIndex = player.StaminaRegenUpgradeIndex;
        save.StaminaHasteUpgradeIndex = player.StaminaHasteUpgradeIndex;
        save.JumpStaminaUpgradeIndex = player.JumpStaminaUpgradeIndex;
        save.SprintStaminaUpgradeIndex = player.SprintStaminaUpgradeIndex;
        save.AttackStaminaUpgradeIndex = player.AttackStaminaUpgradeIndex;

        //Inventory
        save.BackpackUpgradeIndex = inventory.UpgradeIndex;
        save.PistolAmmo = inventory.PistolAmmo;
        save.ShotgunAmmo = inventory.ShotgunAmmo;
        save.RifleAmmo = inventory.RifleAmmo;
        save.SniperAmmo = inventory.SniperAmmo;
        save.RocketAmmo = inventory.RocketAmmo;
        save.PrimaryWeaponsSelection = inventory.PrimaryWeaponsSelection;
        save.SecondaryWeaponsSelection = inventory.SecondaryWeaponsSelection;
        save.ThrowableItemsSelection = inventory.ThrowableItemsSelection;
        save.TacticalAbilitiesSelection = inventory.TacticalAbilitiesSelection;

        //Missing:
        //save.TotalGameTime;
        //save.TotalInStoreTime;
        //save.WavesRestarted;
        //save.TotalKills;
        //save.TotalHeadshotKills;
        //save.TotalPrecision;
        //save.InputMode;

        return save;
    }

    /// <summary>
    /// Descarrega as informações de um save para o estado atual do jogo.
    /// </summary>
    /// <param name="save">O save a ser lido.</param>
    /// <param name="player">O ScriptableObject do jogador.</param>
    /// <param name="inventory">O ScriptableObject do inventário;</param>
    private static void UnSeed(NZSave save, PlayerData player, InventoryData inventory)
    {
        //Player
        player.CurrentWaveIndex = save.CurrentWave;
        player.Money = save.PlayerTotalMoney;
        player.Health = save.PlayerHealth;
        player.Score = save.TotalScore;
        player.MaxHealthUpgradeIndex = save.MaxHealthUpgradeIndex;
        player.MovementSpeedUpgradeIndex = save.MovementSpeedUpgradeIndex;
        player.SprintSpeedUpgradeIndex = save.SprintSpeedUpgradeIndex;
        player.JumpForceUpgradeIndex = save.JumpForceUpgradeIndex;
        player.MaxStaminaUpgradeIndex = save.MaxStaminaUpgradeIndex;
        player.StaminaRegenUpgradeIndex = save.StaminaRegenUpgradeIndex;
        player.StaminaHasteUpgradeIndex = save.StaminaHasteUpgradeIndex;
        player.JumpStaminaUpgradeIndex = save.JumpStaminaUpgradeIndex;
        player.SprintStaminaUpgradeIndex = save.SprintStaminaUpgradeIndex;
        player.AttackStaminaUpgradeIndex = save.AttackStaminaUpgradeIndex;

        //Inventory
        inventory.UpgradeIndex = save.BackpackUpgradeIndex;
        inventory.PistolAmmo = save.PistolAmmo;
        inventory.ShotgunAmmo = save.ShotgunAmmo;
        inventory.RifleAmmo = save.RifleAmmo;
        inventory.SniperAmmo = save.SniperAmmo;
        inventory.RocketAmmo = save.RocketAmmo;
        inventory.PrimaryWeaponsSelection = save.PrimaryWeaponsSelection;
        inventory.SecondaryWeaponsSelection = save.SecondaryWeaponsSelection;
        inventory.ThrowableItemsSelection = save.ThrowableItemsSelection;
        inventory.TacticalAbilitiesSelection = save.TacticalAbilitiesSelection;
    }

    /// <summary>
    /// Obtém o ScriptableObject do jogador.
    /// </summary>
    /// <returns>O SO do jogador.</returns>
    private static PlayerData GetPlayerSO() => Resources.Load<PlayerData>("ScriptableObjects/Player/Player");

    /// <summary>
    /// Obtém o ScriptableObject do inventário.
    /// </summary>
    /// <returns>O SO do inventário.</returns>
    private static InventoryData GetInventorySO() => Resources.Load<InventoryData>("ScriptableObjects/Player/PlayerInventory");
}
