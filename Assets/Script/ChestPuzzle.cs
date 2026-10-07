using UnityEngine;
using TMPro;
using ArabicSupport;

public class ChestPuzzle : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private GameObject playerObject;

    // حالات الصندوق
    private enum ChestState { Locked, OpenedReadyToTakeKey, Finished }
    private ChestState currentState = ChestState.Locked;

    [Header("إعدادات واجهة المستخدم")]
    public GameObject puzzlePanel;
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI questionText; // نص السؤال أو العنوان داخل اللوحة
    public TextMeshProUGUI backButtonText; // نص زر الرجوع

    [Header("إعدادات أزرار الخيارات")]
    public TextMeshProUGUI[] optionButtonTexts; // نصوص الأزرار الأربعة (ملعقة، شوكة، سكين، جزرة)

    [Header("Animation & Audio")]
    public Animator chestAnimator;
    public AudioClip chestOpenSound;
    public AudioClip keyPickupSound; // صوت أخذ المفتاح

    void Start()
    {
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        if (promptText != null) promptText.gameObject.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
        currentState = ChestState.Locked;

        // تعيين نصوص الأزرار تلقائياً مع الإصلاح العربي بالترتيب الصحيح
        if (optionButtonTexts != null && optionButtonTexts.Length >= 4)
        {
            optionButtonTexts[0].text = ArabicFixer.Fix("ملعقة");
            optionButtonTexts[1].text = ArabicFixer.Fix("شوكة");
            optionButtonTexts[2].text = ArabicFixer.Fix("سكين");    // الإجابة الصحيحة
            optionButtonTexts[3].text = ArabicFixer.Fix("جزرة");
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // الحالة الأولى: الصندوق مقفل واللاعب يريد حل اللغز
            if (currentState == ChestState.Locked && !puzzlePanel.activeSelf)
            {
                TogglePuzzleUI();
            }
            // الحالة الثانية: الصندوق فتح وجاهز لأخذ المفتاح بداخله
            else if (currentState == ChestState.OpenedReadyToTakeKey)
            {
                TakeKeyFromChest();
            }
        }
    }

    void TogglePuzzleUI()
    {
        bool isActive = puzzlePanel.activeSelf;

        if (!isActive)
        {
            puzzlePanel.SetActive(true);
            Time.timeScale = 0f; // إيقاف الزمن بالكامل
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (feedbackText != null) feedbackText.text = "";

            // تعيين السؤال باللغة العربية الفصحى
            if (questionText != null)
            {
                questionText.text = ArabicFixer.Fix("سوال موجود في الورقة ,ابحث عنها ");
            }
            if (backButtonText != null)
            {
                backButtonText.text = ArabicFixer.Fix("رجوع");
            }

            if (promptText != null) promptText.gameObject.SetActive(false);
        }
        else
        {
            ClosePuzzle();
        }
    }

    // تُستدعى عند الضغط على الإجابة الصحيحة في اللغز
    public void CorrectChoice()
    {
        if (currentState != ChestState.Locked) return;

        if (feedbackText != null)
        {
            feedbackText.text = ArabicFixer.Fix("إجابة صحيحة! فتح الصندوق.");
        }

        // 1. تشغيل أنيميشن فتح الصندوق
        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("Open");
        }

        // 2. تشغيل صوت فتح الصندوق
        if (chestOpenSound != null)
        {
            AudioSource.PlayClipAtPoint(chestOpenSound, transform.position);
        }

        // 3. تغيير حالة الصندوق ليصبح مفتوحاً وبانتظار أخذ المفتاح
        currentState = ChestState.OpenedReadyToTakeKey;

        // إعادة الزمن للوضع الطبيعي قبل إغلاق اللوحة
        Time.timeScale = 1f;

        // إغلاق واجهة اللغز بعد ثوانٍ ليرى الرسالة
        Invoke("CloseUIPanelAfterOpen", 1.5f);
        Invoke("ClearFeedback", 3f);
    }

    void CloseUIPanelAfterOpen()
    {
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        if (!PauseMenu.GameIsPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        // تحديث النص ليطلب من اللاعب أخذ المفتاح
        if (isPlayerInRange)
        {
            UpdatePromptText();
        }
    }

    // دالة إغلاق اللوحة عبر زر الرجوع واستعادة الزمن
    public void ClosePuzzle()
    {
        Time.timeScale = 1f; // استعادة حركة الزمن
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        if (!PauseMenu.GameIsPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        ClearFeedback();

        if (isPlayerInRange)
        {
            UpdatePromptText();
        }
    }

    // تُستدعى عند الضغط على E والمفتاح داخل الصندوق المفتوح
    void TakeKeyFromChest()
    {
        currentState = ChestState.Finished;

        // إعطاء المفتاح للاعب
        if (playerObject != null)
        {
            PlayerMovement player = playerObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.hasKey = true;
            }
        }

        // تشغيل صوت التقاط المفتاح
        if (keyPickupSound != null)
        {
            AudioSource.PlayClipAtPoint(keyPickupSound, transform.position);
        }

        // إخفاء النص نهائياً لأنه أخذ المفتاح وانتهى
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }

        Debug.Log("تم أخذ المفتاح من داخل الصندوق بنجاح!");
    }

    // تُستدعى عند الضغط على إجابة خاطئة
    public void WrongChoice()
    {
        if (feedbackText != null)
        {
            feedbackText.text = ArabicFixer.Fix("إجابة خاطئة، حاول مرة أخرى!");
        }
        Invoke("ClearFeedback", 3f);
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    void UpdatePromptText()
    {
        if (promptText == null) return;

        if (currentState == ChestState.Locked)
        {
            promptText.text = ArabicFixer.Fix("اضغط E لفحص الصندوق");
            promptText.gameObject.SetActive(true);
        }
        else if (currentState == ChestState.OpenedReadyToTakeKey)
        {
            promptText.text = ArabicFixer.Fix("اضغط E لأخذ المفتاح");
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentState != ChestState.Finished)
        {
            isPlayerInRange = true;
            playerObject = other.gameObject;
            UpdatePromptText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptText != null) promptText.gameObject.SetActive(false);
            if (puzzlePanel != null) puzzlePanel.SetActive(false);
            ClearFeedback();

            if (!PauseMenu.GameIsPaused)
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}