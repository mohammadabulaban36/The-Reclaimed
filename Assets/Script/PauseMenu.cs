using UnityEngine;
using TMPro;
using ArabicSupport;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("إعدادات الواجهة")]
    public GameObject pauseMenuUI; // لوحة الإيقاف المؤقت (Panel)
    public TextMeshProUGUI pauseTitleText, resumeButtonText, restartButtonText, quitButtonText;

    void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        FixTexts();
    }

    void Update()
    {
        // الضغط على زر ESC لإيقاف اللعبة أو استئنافها
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    void FixTexts()
    {
        if (pauseTitleText != null) pauseTitleText.text = ArabicFixer.Fix("إيقاف مؤقت");
        if (resumeButtonText != null) resumeButtonText.text = ArabicFixer.Fix("إكمال اللعب");
        if (restartButtonText != null) restartButtonText.text = ArabicFixer.Fix("إعادة اللعب");
        if (quitButtonText != null) quitButtonText.text = ArabicFixer.Fix("خروج من اللعبة");
    }
}