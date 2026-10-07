using UnityEngine;
using TMPro;
using ArabicSupport;

public class NoteInteraction : MonoBehaviour
{
    private bool isPlayerInRange = false;

    [Header("إعدادات واجهة الورقة")]
    public GameObject notePanel; // لوحة عرض الورقة (Panel)
    public TextMeshProUGUI promptText; // نص الإرشاد "اضغط R لقراءة الورقة"
    public TextMeshProUGUI noteText; // النص المكتوب داخل الورقة
    public TextMeshProUGUI backButtonText; // نص زر الإغلاق

    [Header("صوت (اختياري)")]
    public AudioClip paperSound;

    void Start()
    {
        if (notePanel != null) notePanel.SetActive(false);
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.R))
        {
            ToggleNotePanel();
        }
    }

    void ToggleNotePanel()
    {
        bool isActive = notePanel.activeSelf;

        if (!isActive)
        {
            notePanel.SetActive(true);
            Time.timeScale = 0f; // إيقاف الزمن لقراءة الورقة براحة
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // نص السؤال داخل الورقة باللغة الفصحى الصحيحة
            if (noteText != null)
            {
                noteText.text = ArabicFixer.Fix("أيّ من هذه الخيارات خطير؟\nابحث عن الإجابة الصحيحة وتوجه نحو الصندوق لفتحها!");
            }
            if (backButtonText != null)
            {
                backButtonText.text = ArabicFixer.Fix("إغلاق");
            }
            if (promptText != null) promptText.gameObject.SetActive(false);

            if (paperSound != null)
            {
                AudioSource.PlayClipAtPoint(paperSound, transform.position);
            }
        }
        else
        {
            CloseNote();
        }
    }

    public void CloseNote()
    {
        Time.timeScale = 1f; // استعادة الزمن
        if (notePanel != null) notePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (isPlayerInRange && promptText != null)
        {
            promptText.gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (promptText != null)
            {
                promptText.text = ArabicFixer.Fix("اضغط R لقراءة الورقة");
                promptText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
            if (notePanel != null && notePanel.activeSelf)
            {
                CloseNote();
            }
        }
    }
}