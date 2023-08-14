using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    static MenuController _instance;

    public static MenuController Instance
    {
        get
        {
            if (_instance == null) _instance = GameObject.Find("MenuController").GetComponent<MenuController>();
            return _instance;
        }
    }

    public bool IsGamePaused { get; set; }

    [SerializeField]
    private GameObject PausePanel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (IsGamePaused)
                ContinueGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        IsGamePaused = true;
        PausePanel.SetActive(true);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        IsGamePaused = false;
        PausePanel.SetActive(false);
    }

    public void RestartGame()
    {
        ContinueGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        ContinueGame();
    }

    public void ResetButton(Animator btnAnimator)
    {
        btnAnimator.ResetTrigger("Highlighted");
        btnAnimator.ResetTrigger("Pressed");
        btnAnimator.ResetTrigger("Selected");
        btnAnimator.SetTrigger("Normal");
        btnAnimator.transform.localScale = new Vector3(1, 1, 1);
    }
}
