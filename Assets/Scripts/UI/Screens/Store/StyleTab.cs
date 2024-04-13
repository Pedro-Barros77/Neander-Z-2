using HSVPicker;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StyleTab : MonoBehaviour
{
    StoreScreen storeScreen;

    public bool IsSkinDirty { get; private set; }

    [SerializeField]
    public SkinManager SkinManager;
    [SerializeField]
    TMP_Dropdown CharactersDropdown, AnimationDropdown;
    [SerializeField]
    GameObject NewCharacterModal;
    [SerializeField]
    InputField InputNewCharacterName;
    [SerializeField]
    Slider SkinColorSlider;
    [SerializeField]
    Sprite SkinGradientSprite;

    [SerializeField]
    ColorPicker ColorPicker;

    [SerializeField]
    Button HatPrevBtn, HatNextBtn, HairPrevBtn, HairNextBtn, HeadPrevBtn, HeadNextBtn, TorsoPrevBtn, TorsoNextBtn, ShirtPrevBtn, ShirtNextBtn, LegsPrevBtn, LegsNextBtn, PantsPrevBtn, PantsNextBtn, ShoesPrevBtn, ShoesNextBtn;
    [SerializeField]
    Button BtnDeleteCharacter;

    [SerializeField]
    Image HairColorPreview, EyeColorPreview;

    int currentHatIndex = 0, currentHairIndex = 0, currentHeadIndex = 0, currentTorsoIndex = 0, currentShirtIndex = 0, currentLegsIndex = 0, currentPantsIndex = 0, currentShoesIndex = 0;
    Color32 CurrentSkinColor, CurrentHairColor, CurrentEyeColor, ColorBeforeEdit;

    List<SkinData.Data> CharactersOptions;

    SkinColoringTypes CurrentSettingColor;

    void Start()
    {
        storeScreen = GetComponent<StoreScreen>();

        SkinManager.LoadSkinData(storeScreen.PlayerData.SkinData);
        LoadSkin();

        CharactersOptions = Resources.LoadAll<SkinData>($"ScriptableObjects/Player/Characters").Select(x => x.Encode()).ToList();
        var globalSave = SavesManager.GetGlobalSave();
        if (globalSave.SavedCharacters.Any())
            CharactersOptions.AddRange(globalSave.SavedCharacters);
        CharactersDropdown.options.Clear();
        CharactersDropdown.options = CharactersOptions.Select(c => new TMP_Dropdown.OptionData(c.CharacterName)).ToList();
        CharactersDropdown.value = 0;
        CharactersDropdown.onValueChanged.AddListener(SelectCharacter);
        var selectedCharacter = CharactersOptions.FirstOrDefault(x => x.CharacterName == storeScreen.PlayerData.SkinData.CharacterName);
        if (selectedCharacter != null)
            CharactersDropdown.value = CharactersOptions.IndexOf(selectedCharacter);

        AnimationDropdown.options.Clear();
        AnimationDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(AnimationTypes))));
        AnimationDropdown.value = (int)AnimationTypes.Idle;
        AnimationDropdown.onValueChanged.AddListener(SetAnimationPreview);



        GenerateSkinColorSliderGradient();
        SkinColorSlider.onValueChanged.AddListener(SetSkinColor);

        SkinManager.IsPreviewAnimation = true;
    }

    void Update()
    {
        HatPrevBtn.interactable = currentHatIndex > 0;
        HatNextBtn.interactable = currentHatIndex < Enum.GetNames(typeof(SkinHatOptions)).Length - 1;
        HairPrevBtn.interactable = currentHairIndex > 0;
        HairNextBtn.interactable = currentHairIndex < Enum.GetNames(typeof(SkinHairOptions)).Length - 1;
        HeadPrevBtn.interactable = currentHeadIndex > 1;
        HeadNextBtn.interactable = currentHeadIndex < Enum.GetNames(typeof(SkinHeadOptions)).Length - 1;
        TorsoPrevBtn.interactable = currentTorsoIndex > 1;
        TorsoNextBtn.interactable = currentTorsoIndex < Enum.GetNames(typeof(SkinTorsoOptions)).Length - 1;
        ShirtPrevBtn.interactable = currentShirtIndex > 0;
        ShirtNextBtn.interactable = currentShirtIndex < Enum.GetNames(typeof(SkinShirtOptions)).Length - 1;
        LegsPrevBtn.interactable = currentLegsIndex > 1;
        LegsNextBtn.interactable = currentLegsIndex < Enum.GetNames(typeof(SkinLegOptions)).Length - 1;
        PantsPrevBtn.interactable = currentPantsIndex > 0;
        PantsNextBtn.interactable = currentPantsIndex < Enum.GetNames(typeof(SkinPantsOptions)).Length - 1;
        ShoesPrevBtn.interactable = currentShoesIndex > 0;
        ShoesNextBtn.interactable = currentShoesIndex < Enum.GetNames(typeof(SkinShoesOptions)).Length - 1;

        HairColorPreview.color = CurrentHairColor;
        EyeColorPreview.color = CurrentEyeColor;

        BtnDeleteCharacter.interactable = CharactersOptions.ElementAtOrDefault(CharactersDropdown.value)?.Character == CharacterTypes.None;
    }

    /// <summary>
    /// Abre o modal para criar um novo personagem.
    /// </summary>
    public void OpenNewCharacterModal()
    {
        NewCharacterModal.SetActive(true);
        InputNewCharacterName.text = $"Character {CharactersOptions.Where(x => x.Character != CharacterTypes.Custom).Count() + 1:D2}";
    }

    /// <summary>
    /// Fecha o modal de criar um novo personagem.
    /// </summary>
    public void CloseNewCharacterModal()
    {
        NewCharacterModal.SetActive(false);
    }

    /// <summary>
    /// Cria um novo personagem com o nome definido pelo input do modal.
    /// </summary>
    public void CreateNewCharacter()
    {
        string characterName = InputNewCharacterName.text;
        if (string.IsNullOrWhiteSpace(characterName) || characterName.Length > 20)
        {
            ShowPopupMessage(characterName.Length > 20 ? "Character name is too long! (Max 20)" : "Character name cannot be empty!", Constants.Colors.YellowAmmo);
            return;
        }
        if (characterName == "Custom")
        {
            ShowPopupMessage("Character name cannot be 'Custom'!", Constants.Colors.YellowAmmo);
            return;
        }
        if (CharactersOptions.Any(x => x.CharacterName == characterName))
        {
            ShowPopupMessage("Character name already exists! Delete it or choose another name.", Constants.Colors.YellowAmmo);
            return;
        }

        SaveCurrentSkinData();

        var newSkinData = storeScreen.PlayerData.SkinData.Encode();
        newSkinData.CharacterName = characterName;
        newSkinData.Character = CharacterTypes.None;

        if (SavesManager.SaveCharacter(newSkinData))
        {
            ShowPopupMessage("New Character Created!", Constants.Colors.GreenMoney);
            RemoveCustomSkin();

            var newSkin = ScriptableObject.CreateInstance<SkinData>();
            newSkinData.Seed(newSkin);
            storeScreen.PlayerData.SkinData = newSkin;

            CharactersOptions.Add(newSkinData);
            CharactersDropdown.options.Add(new TMP_Dropdown.OptionData(newSkinData.CharacterName));
            CharactersDropdown.value = CharactersDropdown.options.Count - 1;
            CharactersDropdown.RefreshShownValue();
            CloseNewCharacterModal();
            IsSkinDirty = true;
        }
        else
        {
            ShowPopupMessage("Failed to create new character!", Color.red);
        }
    }

    /// <summary>
    /// Exclui o personagem selecionado do save global.
    /// </summary>
    public void DeleteCharacter()
    {
        var skinData = CharactersOptions[CharactersDropdown.value];
        if (SavesManager.DeleteCharacter(skinData.CharacterName))
        {
            ShowPopupMessage("Character Deleted!", Constants.Colors.GreenMoney);
            CharactersOptions.Remove(skinData);
            CharactersDropdown.options.RemoveAt(CharactersDropdown.value);
            CharactersDropdown.value = 0;
        }
        else
        {
            ShowPopupMessage("Failed to delete character!", Color.red);
        }
    }

    /// <summary>
    /// Avança um item da skin para o próximo disponível.
    /// </summary>
    /// <param name="itemTypeIndex">O index do item a ser trocado (enum SkinItemTypes)</param>
    public void NextSkinItem(int itemTypeIndex) => ChangeSkinItem((SkinItemTypes)itemTypeIndex, true);
    /// <summary>
    /// Retrocede um item da skin para o anterior disponível.
    /// </summary>
    /// <param name="itemTypeIndex">O index do item a ser trocado (enum SkinItemTypes)</param>
    public void PreviousSkinItem(int itemTypeIndex) => ChangeSkinItem((SkinItemTypes)itemTypeIndex, false);

    /// <summary>
    /// Troca um item da skin para o próximo/anterior disponível.
    /// </summary>
    /// <param name="itemType">O tipo de item da skin a ser trocado.</param>
    /// <param name="incrementIndex">True para avançar um item, false para retroceder.</param>
    public void ChangeSkinItem(SkinItemTypes itemType, bool incrementIndex = true)
    {
        CreateCustomSkinOnEdit();
        int indexDelta = incrementIndex ? 1 : -1;
        switch (itemType)
        {
            case SkinItemTypes.Hat:
                if (!Enum.IsDefined(typeof(SkinHatOptions), currentHatIndex + indexDelta))
                    return;
                currentHatIndex += indexDelta;
                SkinManager.CurrentHat = (SkinHatOptions)currentHatIndex;
                break;

            case SkinItemTypes.Hair:
                if (!Enum.IsDefined(typeof(SkinHairOptions), currentHairIndex + indexDelta))
                    return;
                currentHairIndex += indexDelta;
                SkinManager.CurrentHair = (SkinHairOptions)currentHairIndex;
                break;

            case SkinItemTypes.Head:
                if (!Enum.IsDefined(typeof(SkinHeadOptions), currentHeadIndex + indexDelta))
                    return;
                currentHeadIndex += indexDelta;
                SkinManager.CurrentHead = (SkinHeadOptions)currentHeadIndex;
                break;

            case SkinItemTypes.Shirt:
                if (!Enum.IsDefined(typeof(SkinShirtOptions), currentShirtIndex + indexDelta))
                    return;
                currentShirtIndex += indexDelta;
                SkinManager.CurrentShirt = (SkinShirtOptions)currentShirtIndex;
                break;

            case SkinItemTypes.Torso:
                if (!Enum.IsDefined(typeof(SkinTorsoOptions), currentTorsoIndex + indexDelta))
                    return;
                currentTorsoIndex += indexDelta;
                SkinManager.CurrentTorso = (SkinTorsoOptions)currentTorsoIndex;
                break;

            case SkinItemTypes.Pants:
                if (!Enum.IsDefined(typeof(SkinPantsOptions), currentPantsIndex + indexDelta))
                    return;
                currentPantsIndex += indexDelta;
                SkinManager.CurrentPants = (SkinPantsOptions)currentPantsIndex;
                break;

            case SkinItemTypes.Legs:
                if (!Enum.IsDefined(typeof(SkinLegOptions), currentLegsIndex + indexDelta))
                    return;
                currentLegsIndex += indexDelta;
                SkinManager.CurrentLegs = (SkinLegOptions)currentLegsIndex;
                break;

            case SkinItemTypes.Shoes:
                if (!Enum.IsDefined(typeof(SkinShoesOptions), currentShoesIndex + indexDelta))
                    return;
                currentShoesIndex += indexDelta;
                SkinManager.CurrentShoes = (SkinShoesOptions)currentShoesIndex;
                break;
        }

        SkinManager.UpdateSkin();
        IsSkinDirty = true;
    }

    /// <summary>
    /// Seleciona um personagem da lista de personagens disponíveis.
    /// </summary>
    /// <param name="characterIndex">O index do personagem na lista para selecionar.</param>
    public void SelectCharacter(int characterIndex)
    {
        var newSkin = ScriptableObject.CreateInstance<SkinData>();
        var skinData = CharactersOptions[characterIndex];
        skinData.Seed(newSkin);
        storeScreen.PlayerData.SkinData = newSkin;
        SkinManager.LoadSkinData(storeScreen.PlayerData.SkinData);
        SkinManager.UpdateSkin();
        LoadSkin();
        RemoveCustomSkin();
        IsSkinDirty = true;
    }

    /// <summary>
    /// Define a pré-visualização da animação do personagem.
    /// </summary>
    /// <param name="animationTypeIndex">O index da animação a ser reproduzida (enum AnimationTypes).</param>
    public void SetAnimationPreview(int animationTypeIndex)
    {
        SkinManager.SetAnimation((AnimationTypes)animationTypeIndex);
    }

    /// <summary>
    /// Salva os dados da skin atual no ScriptableObject SkinData.
    /// </summary>
    public void SaveCurrentSkinData()
    {
        if (!IsSkinDirty)
            return;

        storeScreen.PlayerData.SkinData.SkinColor = CurrentSkinColor;
        storeScreen.PlayerData.SkinData.HairColor = CurrentHairColor;
        storeScreen.PlayerData.SkinData.EyeColor = CurrentEyeColor;

        storeScreen.PlayerData.SkinData.Hair = SkinManager.CurrentHair;
        storeScreen.PlayerData.SkinData.Hat = SkinManager.CurrentHat;
        storeScreen.PlayerData.SkinData.Head = SkinManager.CurrentHead;
        storeScreen.PlayerData.SkinData.Shirt = SkinManager.CurrentShirt;
        storeScreen.PlayerData.SkinData.Torso = SkinManager.CurrentTorso;
        storeScreen.PlayerData.SkinData.Pants = SkinManager.CurrentPants;
        storeScreen.PlayerData.SkinData.Legs = SkinManager.CurrentLegs;
        storeScreen.PlayerData.SkinData.Shoes = SkinManager.CurrentShoes;

        IsSkinDirty = false;
    }

    /// <summary>
    /// Cancela a edição de cor e reverte para o valor anterior.
    /// </summary>
    public void CancelColorSet()
    {
        switch (CurrentSettingColor)
        {
            case SkinColoringTypes.Hair:
                SetHairColor(ColorBeforeEdit);
                break;
            case SkinColoringTypes.Eyes:
                SetEyesColor(ColorBeforeEdit);
                break;
        }

        SetColorPickerOpened(SkinColoringTypes.None);
    }

    /// <summary>
    /// Abre o seletor de cor para a cor alvo especificada.
    /// </summary>
    /// <param name="colorTarget">O index do tipo de cor a ser modificado (enum SkinColoringTypes).</param>
    public void SetColorPickerOpened(int colorTarget) => SetColorPickerOpened((SkinColoringTypes)colorTarget);
    /// <summary>
    /// Abre o seletor de cor para a cor alvo especificada.
    /// </summary>
    /// <param name="colorTarget">A cor a ser modificada (enum SkinColoringTypes).</param>
    void SetColorPickerOpened(SkinColoringTypes colorTarget)
    {
        CurrentSettingColor = colorTarget;

        ColorPicker.onValueChanged.RemoveListener(HandleColorPickerChange);
        if (CurrentSettingColor == SkinColoringTypes.None)
        {
            ColorPicker.gameObject.SetActive(false);
            return;
        }

        ColorBeforeEdit = CurrentSettingColor switch
        {
            SkinColoringTypes.Hair => CurrentHairColor,
            SkinColoringTypes.Eyes => CurrentEyeColor,
            _ => Color.white
        };

        ColorPicker.gameObject.SetActive(true);
        ColorPicker.CurrentColor = ColorBeforeEdit;
        ColorPicker.onValueChanged.AddListener(HandleColorPickerChange);
    }

    /// <summary>
    /// Função chamada quando o valor do seletor de cor é alterado.
    /// </summary>
    /// <param name="color">A nova cor selecionada.</param>
    void HandleColorPickerChange(Color color)
    {
        CreateCustomSkinOnEdit();
        switch (CurrentSettingColor)
        {
            case SkinColoringTypes.Hair:
                SetHairColor(color);
                break;
            case SkinColoringTypes.Eyes:
                SetEyesColor(color);
                break;
        }
        ColorPicker.CurrentColor = color;
    }

    /// <summary>
    /// Define o gradiente do seletor de cor de pele, com base nas cores SkinLightestColor e SkinDarkestColor do Constants.
    /// </summary>
    void GenerateSkinColorSliderGradient()
    {
        const int WIDTH = 256;
        Texture2D texture = new(WIDTH, 1);

        Color32[] gradientColors = Enumerable.Range(0, WIDTH)
           .Select(x => (Color32)Color.Lerp(Constants.Colors.SkinLightestColor, Constants.Colors.SkinDarkestColor, x / (float)WIDTH))
           .ToArray();

        texture.SetPixels32(gradientColors);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        Image imageComponent = SkinColorSlider.transform.Find("Background").GetComponent<Image>();
        Sprite gradientSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        imageComponent.sprite = gradientSprite;
    }

    /// <summary>
    /// Define a cor da pele com base no valor do slider.
    /// </summary>
    /// <param name="value">O valor do slider do seletor de cor da pele.</param>
    void SetSkinColor(float value)
    {
        CreateCustomSkinOnEdit();
        CurrentSkinColor = Color.Lerp(Constants.Colors.SkinLightestColor, Constants.Colors.SkinDarkestColor, value);
        SkinManager.UpdateSkinColor(CurrentSkinColor);
        IsSkinDirty = true;
    }

    /// <summary>
    /// Define a cor do cabelo.
    /// </summary>
    /// <param name="color">A cor a ser definida para o cabelo.</param>
    void SetHairColor(Color color)
    {
        CreateCustomSkinOnEdit();
        CurrentHairColor = color;
        SkinManager.UpdateHairColor(CurrentHairColor);
        IsSkinDirty = true;
    }

    /// <summary>
    /// Define a cor dos olhos.
    /// </summary>
    /// <param name="color">A cor a ser definida para os olhos.</param>
    void SetEyesColor(Color color)
    {
        CreateCustomSkinOnEdit();
        CurrentEyeColor = color;
        SkinManager.UpdateEyesColor(CurrentEyeColor);
        IsSkinDirty = true;
    }

    /// <summary>
    /// Carrega a skin atual do jogador para a aba de estilo.
    /// </summary>
    void LoadSkin()
    {
        SkinColorSlider.SetValueWithoutNotify(SkinManager.CurrentSkinColor.GetRatioFromRange(Constants.Colors.SkinLightestColor, Constants.Colors.SkinDarkestColor));
        HairColorPreview.color = SkinManager.CurrentHairColor;
        EyeColorPreview.color = SkinManager.CurrentEyeColor;

        CurrentSkinColor = SkinManager.CurrentSkinColor;
        CurrentHairColor = SkinManager.CurrentHairColor;
        CurrentEyeColor = SkinManager.CurrentEyeColor;

        currentHatIndex = (int)SkinManager.CurrentHat;
        currentHairIndex = (int)SkinManager.CurrentHair;
        currentHeadIndex = (int)SkinManager.CurrentHead;
        currentTorsoIndex = (int)SkinManager.CurrentTorso;
        currentShirtIndex = (int)SkinManager.CurrentShirt;
        currentLegsIndex = (int)SkinManager.CurrentLegs;
        currentPantsIndex = (int)SkinManager.CurrentPants;
        currentShoesIndex = (int)SkinManager.CurrentShoes;
    }

    /// <summary>
    /// Cria uma skin temporária para armazenar as alterações feitas na aba de estilo.
    /// </summary>
    void CreateCustomSkinOnEdit()
    {
        if (CharactersOptions.Any(c => c.CharacterName == "Custom"))
        {
            CharactersDropdown.value = CharactersOptions.FindIndex(c => c.CharacterName == "Custom");
            return;
        }

        var newSkinData = storeScreen.PlayerData.SkinData.Encode();
        newSkinData.CharacterName = "Custom";
        newSkinData.Character = CharacterTypes.Custom;

        var newSkin = ScriptableObject.CreateInstance<SkinData>();
        newSkinData.Seed(newSkin);
        storeScreen.PlayerData.SkinData = newSkin;

        CharactersOptions.Add(newSkinData);
        CharactersDropdown.options.Add(new TMP_Dropdown.OptionData(newSkinData.CharacterName));
        CharactersDropdown.value = CharactersOptions.Count - 1;
    }

    /// <summary>
    /// Remove a skin temporária criada na aba de estilo.
    /// </summary>
    void RemoveCustomSkin()
    {
        var customSkin = CharactersOptions.FirstOrDefault(x => x.Character == CharacterTypes.Custom);
        if (customSkin != null)
        {
            CharactersDropdown.options.RemoveAt(CharactersDropdown.options.FindIndex(x => x.text == customSkin.CharacterName));
            CharactersOptions.Remove(customSkin);
        }
    }

    /// <summary>
    /// Exibe uma mensagem pop-up na tela.
    /// </summary>
    /// <param name="message">O texto da mensagem.</param>
    /// <param name="color">A cor do texto.</param>
    void ShowPopupMessage(string message, Color32 color)
    {
        storeScreen.ShowPopup(message, color, CharactersDropdown.transform.position + new Vector3(4, 0), 2000f, 30);
    }
}
