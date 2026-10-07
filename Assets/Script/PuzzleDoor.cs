using UnityEngine;
using TMPro;
using ArabicSupport;

public class PuzzleDoor : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private bool isDoorOpened = false;

    [Header("إعدادات واجهة المستخدم")]
    public GameObject numberPuzzlePanel; // لوحة الألغاز
    public TextMeshProUGUI promptText; // نص "اضغط E لحل لغز الباب"
    public TextMeshProUGUI feedbackText; // نص رسائل الخطأ والنجاح
    public TextMeshProUGUI questionText; // نص السؤال داخل اللوحة
    public TMP_InputField inputField; // خانة كتابة الرقم
    public TextMeshProUGUI submitButtonText; // نص زر التأكيد
    public TextMeshProUGUI backButtonText; // نص زر الرجوع

    [Header("إعدادات اللغز والباب")]
    public int correctLegsCount = 8; // العدد الصحيح لأرجل الكراسي
    public Animator doorAnimator;
    public AudioClip doorOpenSound;
    public AudioClip successSound;

    void Start()
    {
        if (numberPuzzlePanel != null) numberPuzzlePanel.SetActive(false);
        if (promptText != null) promptText.gameObject.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";

        if (inputField != null && inputField.placeholder != null)
        {
            TextMeshProUGUI placeholderText = inputField.placeholder.GetComponent<TextMeshProUGUI>();
            if (placeholderText != null)
            {
                placeholderText.text = ArabicFixer.Fix("اكتب الرقم هنا");
            }
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDoorOpened)
        {
            TogglePuzzleUI();
        }
    }

    void TogglePuzzleUI()
    {
        bool isActive = numberPuzzlePanel.activeSelf;

        if (!isActive)
        {
            numberPuzzlePanel.SetActive(true);
            Time.timeScale = 0f; // إيقاف الزمن بالكامل
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (feedbackText != null) feedbackText.text = "";
            if (inputField != null) inputField.text = "";

            if (questionText != null)
            {
                questionText.text = ArabicFixer.Fix("كم عدد أرجل الكراسي؟");
            }
            if (submitButtonText != null)
            {
                submitButtonText.text = ArabicFixer.Fix("تأكيد");
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

    public void CheckAnswer()
    {
        if (isDoorOpened || inputField == null) return;

        string cleanInput = inputField.text.Trim();

        if (int.TryParse(cleanInput, out int enteredNumber))
        {
            if (enteredNumber == correctLegsCount)
            {
                isDoorOpened = true;
                if (feedbackText != null)
                {
                    feedbackText.text = ArabicFixer.Fix("إجابة صحيحة! فتح الباب.");
                }

                if (successSound != null)
                {
                    AudioSource.PlayClipAtPoint(successSound, transform.position);
                }

                if (doorAnimator != null)
                {
                    doorAnimator.SetTrigger("Open");
                }

                if (doorOpenSound != null)
                {
                    AudioSource.PlayClipAtPoint(doorOpenSound, transform.position);
                }

                // إعادة الزمن للوضع الطبيعي قبل إغلاق اللوحة
                Time.timeScale = 1f;

                Invoke("ClosePuzzle", 2f);
                Invoke("ClearFeedback", 3f);
            }
            else
            {
                if (feedbackText != null)
                {
                    feedbackText.text = ArabicFixer.Fix("رقم خاطئ، حاول عدّ أرجل الكراسي مجدداً!");
                }
                Invoke("ClearFeedback", 3f);
            }
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text = ArabicFixer.Fix("الرجاء إدخال رقم صحيح!");
            }
            Invoke("ClearFeedback", 3f);
        }
    }

    // دالة إغلاق اللوحة وإعادة الزمن (تُربط بزر الرجوع)
    public void ClosePuzzle()
    {
        Time.timeScale = 1f; // استعادة حركة الزمن
        if (numberPuzzlePanel != null) numberPuzzlePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (promptText != null && !isDoorOpened)
        {
            promptText.text = ArabicFixer.Fix("اضغط E لحل لغز الباب");
            promptText.gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDoorOpened)
        {
            isPlayerInRange = true;
            UpdatePromptText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptText != null) promptText.gameObject.SetActive(false);
            if (numberPuzzlePanel != null) numberPuzzlePanel.SetActive(false);
            ClearFeedback();

            // إضافة هذا الشرط: إذا كانت اللعبة ليست في حالة إيقاف مؤقت، اضبط الماوس
            if (!PauseMenu.GameIsPaused)
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}