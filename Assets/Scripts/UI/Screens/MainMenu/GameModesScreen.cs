using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameModesScreen : MonoBehaviour
{
    public GameModes SelectedGamemode { get; private set; } = GameModes.WaveMastery;

    [SerializeField]
    Toggle BtnWaveMastery, BtnEndlessSurvival, BtnSiegeDefense, BtnCampaign;
    [SerializeField]
    TextMeshProUGUI TxtGameModeTitle, TxtGameModeDescription;
    [SerializeField]
    GameObject SelectGameModeContent, MainMenuContent;
    [SerializeField]
    Button BtnPlay;

    TextMeshProUGUI BtnPlayText;

    const string WaveMasteryDescription = "   This game mode challenges you with a 50 waves progression, where the difficulty is always increasing and you'll encounter a powerful boss each 10 waves.\r\n   \r\n   You start with nothing, choose your first weapon and try to survive, carefully spending your earned resources into weapons, ammo, upgrades, skills and more!";
    const string EndlessSurvivalDescription = "   In 'Endless Survival,' there is no end in sight as you face an infinite and ever-escalating challenge. Your goal is simple: survive for as long as you can! \r\n   There are no set-waves, no scripted progression, just a continuous battle for survival.\r\n   Manage your resources wisely to resist the attacks and be the owner of the highest score!";
    const string SiegeDefenseDescription = "   In 'Siege Defense,' you must defend your base from the hordes of enemies that will try to break in and kill you.\r\n  You must strategically allocate your resources to reinforce defenses, purchase powerful weapons, and maintain the integrity of your barricades.\r\n   This game mode provides a structured and intense gameplay experience, challenging your tactical thinking and resource management skills.";
    const string CampaignDescription = "   In the 'Campaign' mode, you embark on an immersive journey through a zombie-infested world, where a gripping storyline unfolds. \r\n   You'll face a series of carefully crafted missions and objectives, each contributing to the overarching narrative.\r\n   This game mode offers a more guided and meaningful experience. Your actions impact the story, and you'll encounter a variety of challenges and scenarios that demand your unique skills and decision-making.";

    void Start()
    {
        BtnPlayText = BtnPlay.GetComponentInChildren<TextMeshProUGUI>();
    }


    void Update()
    {
        switch (SelectedGamemode)
        {
            case GameModes.WaveMastery:
                BtnWaveMastery.Select();
                TxtGameModeTitle.text = "Wave Mastery";
                TxtGameModeDescription.text = WaveMasteryDescription;

                BtnPlay.interactable = true;
                BtnPlayText.text = "Play";
                break;
            case GameModes.EndlessSurvival:
                BtnEndlessSurvival.Select();
                TxtGameModeTitle.text = "Endless Survival";
                TxtGameModeDescription.text = EndlessSurvivalDescription;

                BtnPlay.interactable = false;
                BtnPlayText.text = "Soon...";
                break;
            case GameModes.SiegeDefense:
                BtnSiegeDefense.Select();
                TxtGameModeTitle.text = "Siege Defense";
                TxtGameModeDescription.text = SiegeDefenseDescription;

                BtnPlay.interactable = false;
                BtnPlayText.text = "Soon...";
                break;
            case GameModes.Campaign:
                BtnCampaign.Select();
                TxtGameModeTitle.text = "Campaign";
                TxtGameModeDescription.text = CampaignDescription;

                BtnPlay.interactable = false;
                BtnPlayText.text = "Soon...";
                break;
        }
    }

    /// <summary>
    /// Seleciona o modo de jogo a ser iniciado.
    /// </summary>
    /// <param name="modeIndex">O índice do tipo de modo de jogo.</param>
    public void SelectGameMode(int modeIndex)
    {
        SelectedGamemode = (GameModes)modeIndex;
    }

    /// <summary>
    /// Volta para a tela anterior.
    /// </summary>
    public void GoBack()
    {
        SelectGameModeContent.SetActive(false);
        MainMenuContent.SetActive(true);
    }

    /// <summary>
    /// Inicia o modo de jogo selecionado.
    /// </summary>
    public void Play()
    {
        switch (SelectedGamemode)
        {
            case GameModes.WaveMastery:
                MenuController.Instance.ChangeScene(SceneNames.Graveyard, LoadSceneMode.Single);
                break;
        }
    }
}
