using UnityEngine;
using TMPro;
using ArabicSupport;

public class IntroManager : MonoBehaviour
{
    [Header("إعدادات لوحة البداية")]
    public GameObject introPanel; // اللوحة (Panel)
    public TextMeshProUGUI introText; // النص داخل اللوحة
    public float displayDuration = 2f; // مدة ظهور الرسالة بالثواني (يمكنك تعديلها)

    void Start()
    {
        // 1. إظهار اللوحة
        if (introPanel != null) introPanel.SetActive(true);

        // 2. كتابة النص وتصحيحه
        if (introText != null)
        {
            string message = "ماذا حدث لي؟ لقد كنت نائماً...\nإذا نجوت، سوف أستعيد جسدي!";
            introText.text = ArabicFixer.Fix(message);
        }

        // 3. إخفاء الماوس وقفل حركة اللاعب والكاميرا مؤقتاً
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // تعطيل سكربت حركة اللاعب والكاميرا إن وجدوا
        TogglePlayerControls(false);

        // 4. استخدام Coroutine بدل Invoke لأن Invoke لا يعمل إذا توقف الزمن (ولكن هنا الزمن شغال طبيعي)
        Invoke("HideIntro", displayDuration);
    }

    void HideIntro()
    {
        // 5. إخفاء اللوحة وإعادة التحكم للاعب
        if (introPanel != null) introPanel.SetActive(false);
        TogglePlayerControls(true);
    }

    void TogglePlayerControls(bool enable)
    {
        // تعطيل أو تفعيل حركة الكاميرا واللاعب مؤقتاً
        MouseCamLook camLook = FindObjectOfType<MouseCamLook>();
        if (camLook != null) camLook.enabled = enable;

        PlayerMovement movement = FindObjectOfType<PlayerMovement>();
        if (movement != null) movement.enabled = enable;
    }
}