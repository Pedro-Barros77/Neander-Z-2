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
        PlayerData player = GetPlayerSO();
        InventoryData inventory = GetInventorySO();
        NZSave save = Seed(new(), player, inventory);

        return SaveNzSave(save, gameMode, fileName, encrypted);
    }

    /// <summary>
    /// Dado um save, salva-o no arquivo.
    /// </summary>
    /// <param name="save">O save a ser salvo em disco.</param>
    /// <param name="gameMode">O modo de jogo a ser salvo.</param>
    /// <param name="fileName">O nome do arquivo a ser salvo.</param>
    /// <param name="encrypted">Se o arquivo deve ser criptografado ou não.</param>
    /// <returns>True se o arquivo foi salvo com sucesso.</returns>
    public static bool SaveNzSave(NZSave save, GameModes gameMode, string fileName, bool encrypted = false)
    {
        JsonSaveService jsonService = new();

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
    /// Atualiza informações da wave que devem ser imediatas (antes do ponto de save padrão) e as salva.
    /// </summary>
    /// <param name="waveNumber">O número da wave a ser atualizada.</param>
    /// <param name="started">Se a wave está sendo iniciada agora.</param>
    /// <param name="completed">Se a wave está sendo completada agora.</param>
    /// <returns>Retorna as informações atualizadas da wave.</returns>
    public static WaveStats.Data UpdateWaveStats(int waveNumber, bool started = false, bool died = false)
    {
        var save = GetSave(GameModes.WaveMastery, SelectedSaveName);
        if (save.WavesStats.Count < waveNumber)
            save.WavesStats.Add(new WaveStats.Data()
            {
                WaveNumber = waveNumber
            });

        var stat = save.WavesStats.First(x => x.WaveNumber == waveNumber);

        if (started)
        {
            if (stat.Started)
                stat.RestartCount++;
            stat.Started = true;
        }

        if (died)
            stat.DeathCount++;

        SaveNzSave(save, GameModes.WaveMastery, SelectedSaveName, true);

        return stat;
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
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            return new List<NZSave>();
        }

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
    /// Carrega um arquivo de save.
    /// </summary>
    /// <param name="gameMode">O modo de jogo do save a ser carregado.</param>
    /// <param name="fileName">O nome do arquivo do save.</param>
    /// <param name="encrypted">Se o arquivo foi criptografado.</param>
    /// <returns>O save carregado.</returns>
    public static NZSave GetSave(GameModes gameMode, string fileName, bool encrypted = false)
    {
        JsonSaveService jsonService = new();
        string folderPath = jsonService.CombinePaths(jsonService.ROOT_FOLDER, gameMode.ToString());
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            return null;
        }
        var save = jsonService.LoadData<NZSave>(folderPath, fileName, encrypted);
        if (save == null)
        {
            Debug.LogError($"Save file {fileName} not found!");
            return null;
        }

        save.FileName = fileName;
        save.FolderPath = folderPath;

        return save;
    }

    /// <summary>
    /// Carrega o progresso de um save para dentro do jogo.
    /// </summary>
    /// <param name="gameMode">O modo de jogo do save a ser carregado.</param>
    /// <param name="fileName">O nome do arquivo do save.</param>
    /// <param name="encrypted">Se o arquivo foi criptografado.</param>
    public static void LoadSavedGame(GameModes gameMode, string fileName, bool encrypted = false)
    {
        NZSave save = GetSave(gameMode, fileName, encrypted);

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
        if (!File.Exists(path))
        {
            Debug.LogError($"Save file {fileName} not found to delete!");
            return;
        }
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
        if (!File.Exists(path))
        {
            Debug.LogError($"Save file {save.FileName} not found to delete!");
            return;
        }
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
        InventoryData.WeaponSelection UpdateWeaponMagazine(InventoryData.WeaponSelection weaponSelection)
        {
            var weapon = GetWeaponDataSO(weaponSelection.Type, weaponSelection.WeaponClass);
            if (weapon == null) return null;
            weaponSelection.MagazineBullets = weapon.MagazineBullets;
            return weaponSelection;
        }

        //Player
        save.CurrentWave = player.CurrentWaveIndex;
        save.PlayerTotalMoney = player.Money;
        save.PlayerHealth = player.Health;
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
        save.PrimaryWeaponsSelection = inventory.PrimaryWeaponsSelection.Select(UpdateWeaponMagazine).Where(x => x != null).ToList();
        save.SecondaryWeaponsSelection = inventory.SecondaryWeaponsSelection.Select(UpdateWeaponMagazine).Where(x => x != null).ToList();
        save.ThrowableItemsSelection = inventory.ThrowableItemsSelection;
        save.TacticalAbilitiesSelection = inventory.TacticalAbilitiesSelection;


        //Waves
        save.WavesStats = GetWaveStats().Where(x => x.Started).Select(x => x.GetData()).ToList();

        //Missing:
        //save.TotalInStoreTime;

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
        player.Score = save.WavesStats.Sum(w => w.Score);
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

        //Waves
        WaveStats[] waveStats = GetWaveStats().OrderBy(x => x.WaveNumber).ToArray();
        for (int i = 0; i < save.WavesStats.OrderBy(x => x.WaveNumber).Count(); i++)
        {
            waveStats[i].WaveNumber = save.WavesStats[i].WaveNumber;
            waveStats[i].Score = save.WavesStats[i].Score;
            waveStats[i].MoneyEarned = save.WavesStats[i].MoneyEarned;
            waveStats[i].EnemiesKilled = save.WavesStats[i].EnemiesKilled;
            waveStats[i].HeadshotKills = save.WavesStats[i].HeadshotKills;
            waveStats[i].Precision = save.WavesStats[i].Precision;
            waveStats[i].RestartCount = save.WavesStats[i].RestartCount;
            waveStats[i].TimeTaken = save.WavesStats[i].TimeTaken;
            waveStats[i].DamageTaken = save.WavesStats[i].DamageTaken;
            waveStats[i].Completed = save.WavesStats[i].Completed;
            waveStats[i].Started = save.WavesStats[i].Started;
            //Pendente salvar tempo na loja
        }

        //Player Upgrades
        var storePlayerSO = GetStorePlayerSO();
        LoadPlayerUpgrades(player, storePlayerSO);

        //Backpack Upgrades
        var storeBackpackSO = GetStoreBackpackSO();
        LoadBackpackUpgrades(save.BackpackUpgradeIndex, storeBackpackSO, inventory);

        //Weapons Upgrades
        foreach (var weaponSelection in save.PrimaryWeaponsSelection.Concat(save.SecondaryWeaponsSelection))
        {
            var weaponData = GetWeaponDataSO(weaponSelection.Type, weaponSelection.WeaponClass);
            LoadWeaponUpgrades(weaponData, weaponSelection);
        }
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

    /// <summary>
    /// Obtém o ScriptableObject do Player na Loja.
    /// </summary>
    /// <returns>O SO do player na loja.</returns>
    private static StorePlayerData GetStorePlayerSO() => Resources.Load<StorePlayerData>("ScriptableObjects/Store/Natives/Player");

    /// <summary>
    /// Obtém o ScriptableObject da Mochila na Loja.
    /// </summary>
    /// <returns>O SO da Mochila na Loja.</returns>
    private static StoreBackpackData GetStoreBackpackSO() => Resources.Load<StoreBackpackData>("ScriptableObjects/Store/Natives/Backpack");

    /// <summary>
    /// Obtém o ScriptableObject da Arma especificada.
    /// </summary>
    /// <param name="weaponType">O tipo da arma.</param>
    /// <param name="weaponClass">A classe da arma.</param>
    /// <returns>O SO da arma especificada.</returns>
    private static BaseWeaponData GetWeaponDataSO(WeaponTypes weaponType, WeaponClasses weaponClass) => Resources.Load<BaseWeaponData>($"ScriptableObjects/Weapons/{weaponClass}/{weaponType}");
    /// <summary>
    /// Carrega todas as estatísticas de waves.
    /// </summary>
    /// <returns>Uma lista contendo todos os SO de WaveStat.</returns>
    private static IEnumerable<WaveStats> GetWaveStats() => Resources.LoadAll<WaveStats>($"ScriptableObjects/Waves/Stats");

    /// <summary>
    /// Carrega todos os upgrades comprados da arma.
    /// </summary>
    /// <param name="weaponData">O SO da arma contendo os dados de upgrade.</param>
    /// <param name="weaponSelection">A seleção da arma a receber os valores dos upgrades.</param>
    private static void LoadWeaponUpgrades(BaseWeaponData weaponData, InventoryData.WeaponSelection weaponSelection)
    {
        if (weaponData == null || weaponSelection == null) return;

        float GetAttributeUpgradedValue(WeaponAttributes attribute)
        {
            var upgradeMap = weaponSelection.UpgradesMap.FirstOrDefault(x => x.Attribute == attribute);

            if (upgradeMap == null)
                return 0;

            if (!weaponData.Upgrades.Any(x => x.Attribute == attribute))
                return 0;

            var upgradeSteps = weaponData.Upgrades.FirstOrDefault(x => x.Attribute == attribute)
                .UpgradeSteps
                .Where((x, i) => i < upgradeMap.UpgradeStep);

            if (upgradeSteps == null || !upgradeSteps.Any())
                return 0;

            return upgradeSteps.Sum(x => x.Value);
        }

        //Damage
        float damageUpgradeValue = GetAttributeUpgradedValue(WeaponAttributes.Damage);
        weaponData.Damage += weaponData.BulletType switch
        {
            BulletTypes.Shotgun => damageUpgradeValue / (weaponData as ShotgunData).ShellPelletsCount,
            _ => damageUpgradeValue
        };

        //FireRate
        float fireRateUpgradeValue = GetAttributeUpgradedValue(WeaponAttributes.FireRate);
        if (weaponData is BurstFireData burstWeapon)
        {
            float fireRatesSum = burstWeapon.FireRate + burstWeapon.BurstFireRate;
            float fireRateAverage = fireRatesSum / 2;
            float targetFireRateAverage = fireRateAverage + fireRateUpgradeValue;

            float fireRateWeight = burstWeapon.FireRate / fireRatesSum;
            float burstFireRateWeight = burstWeapon.BurstFireRate / fireRatesSum;

            float fireRateAverageDifference = (targetFireRateAverage * 2) - fireRatesSum;

            burstWeapon.FireRate += fireRateAverageDifference * fireRateWeight;
            burstWeapon.BurstFireRate += fireRateAverageDifference * burstFireRateWeight;
        }
        else
            weaponData.FireRate += fireRateUpgradeValue;

        //ReloadSpeed
        float reloadSpeedUpgradeValue = GetAttributeUpgradedValue(WeaponAttributes.ReloadSpeed);
        float barReloadSpeed = Constants.CalculateReloadSpeed(weaponData);
        weaponData.ReloadTimeMs = weaponData.ReloadType switch
        {
            ReloadTypes.SingleBullet => 5000 / ((barReloadSpeed + reloadSpeedUpgradeValue) * weaponData.MagazineSize),
            _ => 5000 / (barReloadSpeed + reloadSpeedUpgradeValue)
        };

        //Range
        float rangeUpgradeValue = GetAttributeUpgradedValue(WeaponAttributes.Range);
        float currentValuesSum = weaponData.MinDamageRange + weaponData.MaxDamageRange + weaponData.BulletMaxRange;
        float currentAverage = currentValuesSum / 3;
        float targetAverage = currentAverage + rangeUpgradeValue;
        float minDamageRangeWeight = weaponData.MinDamageRange / currentValuesSum;
        float maxDamageRangeWeight = weaponData.MaxDamageRange / currentValuesSum;
        float bulletMaxRangeWeight = weaponData.BulletMaxRange / currentValuesSum;
        float averageDifference = (targetAverage * 3) - currentValuesSum;
        weaponData.MinDamageRange += averageDifference * minDamageRangeWeight;
        weaponData.MaxDamageRange += averageDifference * maxDamageRangeWeight;
        weaponData.BulletMaxRange += averageDifference * bulletMaxRangeWeight;

        //BulletSpeed
        float bulletSpeedUpgradeValue = GetAttributeUpgradedValue(WeaponAttributes.BulletSpeed);
        weaponData.BulletSpeed += bulletSpeedUpgradeValue;

        //HeadshotMultiplier
        float headshotMultiplierUpgradeValue = GetAttributeUpgradedValue(WeaponAttributes.HeadshotMultiplier);
        weaponData.HeadshotMultiplier += headshotMultiplierUpgradeValue;

        //MagazineSize
        float magazineSizeUpgradeValue = GetAttributeUpgradedValue(WeaponAttributes.MagazineSize);
        weaponData.MagazineSize += (int)magazineSizeUpgradeValue;

        //Dispersion
        float dispersionUpgradeValue = GetAttributeUpgradedValue(WeaponAttributes.Dirspersion);
        if (weaponData is ShotgunData shotgunData)
            shotgunData.PelletsDispersion -= dispersionUpgradeValue;
        else if (weaponData is LauncherData launcherData)
        {
            float radiusSum = launcherData.ExplosionMinDamageRadius + launcherData.ExplosionMaxDamageRadius;
            float radiusAverage = radiusSum / 2;
            float targetRadiusAverage = radiusAverage + dispersionUpgradeValue;

            float minRadiusWeight = launcherData.ExplosionMinDamageRadius / radiusSum;
            float maxRadiusWeight = launcherData.ExplosionMaxDamageRadius / radiusSum;

            float radiusAverageDifference = (targetRadiusAverage * 2) - radiusSum;

            launcherData.ExplosionMinDamageRadius += radiusAverageDifference * minRadiusWeight;
            launcherData.ExplosionMaxDamageRadius += radiusAverageDifference * maxRadiusWeight;
        }

        weaponData.MagazineBullets = weaponSelection.MagazineBullets;
    }

    /// <summary>
    /// Carrega todos os upgrades do player comprados.
    /// </summary>
    /// <param name="playerData">O SO do player a receber os valores de upgrade.</param>
    /// <param name="storePlayerData">O SO do player na loja contendo os dados de upgrades.</param>
    private static void LoadPlayerUpgrades(PlayerData playerData, StorePlayerData storePlayerData)
    {
        if (playerData == null || storePlayerData == null) return;

        float GetAttributeUpgradedValue(PlayerAttributes attribute)
        {
            int upgradeIndex = playerData.GetUpgradeIndex(attribute);

            if (upgradeIndex == 0)
                return 0;

            var upgradeSteps = storePlayerData.GetPlayerUpgrades(attribute)
                .Where((x, i) => i < upgradeIndex);

            if (upgradeSteps == null || !upgradeSteps.Any())
                return 0;

            return upgradeSteps.Sum(x => x.Value);
        }

        playerData.MaxHealth += GetAttributeUpgradedValue(PlayerAttributes.MaxHealth);
        playerData.MaxMovementSpeed += GetAttributeUpgradedValue(PlayerAttributes.MovementSpeed);
        playerData.SprintSpeedMultiplier += GetAttributeUpgradedValue(PlayerAttributes.SprintSpeed);
        playerData.JumpForce += GetAttributeUpgradedValue(PlayerAttributes.JumpForce);
        playerData.MaxStamina += GetAttributeUpgradedValue(PlayerAttributes.MaxStamina);
        playerData.StaminaRegenRate += GetAttributeUpgradedValue(PlayerAttributes.StaminaRegen);

        float staminaHasteUpgradeValue = GetAttributeUpgradedValue(PlayerAttributes.StaminaHaste);
        if (staminaHasteUpgradeValue > 0)
            playerData.StaminaRegenDelayMs = 5000 / (5000 / playerData.StaminaRegenDelayMs + staminaHasteUpgradeValue);

        float jumpStaminaUpgradeValue = GetAttributeUpgradedValue(PlayerAttributes.JumpStamina);
        if (jumpStaminaUpgradeValue > 0)
            playerData.JumpStaminaDrain = 100 / (100 / playerData.JumpStaminaDrain + jumpStaminaUpgradeValue);

        float sprintStaminaUpgradeValue = GetAttributeUpgradedValue(PlayerAttributes.SprintStamina);
        if (sprintStaminaUpgradeValue > 0)
            playerData.SprintStaminaDrain = 100 / (100 / playerData.SprintStaminaDrain + sprintStaminaUpgradeValue);

        float attackStaminaUpgrade = GetAttributeUpgradedValue(PlayerAttributes.AttackStamina);
        if (jumpStaminaUpgradeValue > 0)
            playerData.AttackStaminaDrain = 100 / (100 / playerData.AttackStaminaDrain + attackStaminaUpgrade);
    }

    /// <summary>
    /// Carrega todos os upgrades da mochila comprados.
    /// </summary>
    /// <param name="backpackUpgradeIndex">O índice de upgrade atual da mochila.</param>
    /// <param name="storeBackpackData">O SO da mochila na loja contendo os dados de upgrades.</param>
    /// <param name="inventoryData">O SO do inventário para atualizar os valores.</param>
    private static void LoadBackpackUpgrades(int backpackUpgradeIndex, StoreBackpackData storeBackpackData, InventoryData inventoryData)
    {
        if (backpackUpgradeIndex == 0 || storeBackpackData == null) return;

        var backpackUpgrades = storeBackpackData.AmmoUpgrades
            .Where((x, i) => i < backpackUpgradeIndex);

        if (backpackUpgrades == null || !backpackUpgrades.Any())
            return;

        inventoryData.MaxPistolAmmo += backpackUpgrades.Sum(x => x.PistolAmmo);
        inventoryData.MaxShotgunAmmo += backpackUpgrades.Sum(x => x.ShotgunAmmo);
        inventoryData.MaxRifleAmmo += backpackUpgrades.Sum(x => x.RifleAmmo);
        inventoryData.MaxSniperAmmo += backpackUpgrades.Sum(x => x.SniperAmmo);
        inventoryData.MaxRocketAmmo += backpackUpgrades.Sum(x => x.RocketAmmo);
    }
}
