using UnityEngine;
using TMPro;
using ArabicSupport;

public class OpenDoor : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private GameObject playerObject;
    private bool isDoorOpened = false; // هل تم فتح الباب من قبل؟

    [Header("References")]
    public Animator doorAnimator;
    public TextMeshProUGUI promptText;

    [Header("Audio Settings")]
    public AudioClip doorOpenSound;

    void Start()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // إذا كان اللاعب قريب وضغط E ولم يفتح الباب من قبل
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDoorOpened)
        {
            PlayerMovement player = playerObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                if (player.hasKey)
                {
                    if (doorAnimator != null)
                    {
                        doorAnimator.SetTrigger("Open");
                    }

                    if (doorOpenSound != null)
                    {
                        AudioSource.PlayClipAtPoint(doorOpenSound, transform.position);
                    }

                    player.hasKey = false; // استهلاك المفتاح
                    isDoorOpened = true;   // تثبيت أن الباب أصبح مفتوحاً

                    if (promptText != null)
                    {
                        promptText.gameObject.SetActive(false); // إخفاء النص نهائياً
                    }
                }
                else
                {
                    if (promptText != null)
                    {
                        promptText.text = ArabicFixer.Fix("ابحث عن المفتاح");
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // إذا دخل اللاعب وكان الباب أساساً مفتوحاً، لا تفعل شيئاً ولا تظهر أي نص
        if (other.CompareTag("Player") && !isDoorOpened)
        {
            isPlayerInRange = true;
            playerObject = other.gameObject;

            if (promptText != null)
            {
                PlayerMovement player = other.GetComponent<PlayerMovement>();
                if (player != null && player.hasKey)
                {
                    promptText.text = ArabicFixer.Fix("اضغط E لفتح الباب");
                }
                else
                {
                    promptText.text = ArabicFixer.Fix("ابحث عن المفتاح");
                }
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
        }
    }
}