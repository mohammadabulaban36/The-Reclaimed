using UnityEngine;
using TMPro;
using ArabicSupport;
using UnityEngine.SceneManagement;

public class WinButton : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private bool hasWon = false;

    [Header("إعدادات واجهة الفوز")]
    public GameObject winPanel; // لوحة شاشة الفوز (Panel)
    public TextMeshProUGUI promptText; // نص الإرشاد "اضغط E للتفعيل"
    public TextMeshProUGUI winMessageText; // نص التهنئة داخل اللوحة
    public TextMeshProUGUI restartButtonText; // نص زر إعادة اللعب

    [Header("أنيميشن وأصوات")]
    public Animator buttonAnimator; // مكوّن الأنيميشن الخاص بالزر أو المجسم
    public AudioClip winSound; // صوت الفوز

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        // إذا كان اللاعب قريب وضغط E ولم يفز بعد
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !hasWon)
        {
            StartWinSequence();
        }
    }

    void StartWinSequence()
    {
        hasWon = true;

        // 1. تشغيل الأنيميشن (تأكد أن Trigger اسمه "Win")
        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("Win");
        }

        // 2. تشغيل صوت الفوز
        if (winSound != null)
        {
            AudioSource.PlayClipAtPoint(winSound, transform.position);
        }

        // 3. إخفاء نص الإرشاد الخارجي
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }

        // 4. الانتظار قليلاً لكي تظهر بانل الفوز بعد ما يبدأ الأنيميشن
        Invoke("ShowWinPanel", 1.5f);
    }

    void ShowWinPanel()
    {
        // تفعيل لوحة الفوز
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // كتابة رسالة الفوز الجديدة مصححة باللغة العربية
        if (winMessageText != null)
        {
            winMessageText.text = ArabicFixer.Fix("لقد هربت و لقد استعدت جسدي");
        }

        if (restartButtonText != null)
        {
            restartButtonText.text = ArabicFixer.Fix("إعادة اللعبة");
        }

        // إيقاف الزمن بالكامل وإظهار الماوس
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // دالة لإعادة تشغيل اللعبة عند الضغط على زر
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasWon)
        {
            isPlayerInRange = true;
            if (promptText != null)
            {
                promptText.text = ArabicFixer.Fix("اضغط E للتفعيل");
                promptText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptText != null && !hasWon)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }
}