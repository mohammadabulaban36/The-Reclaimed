using UnityEngine;
using TMPro;
using ArabicSupport;

public class Key : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private GameObject playerObject;

    [Header("UI Prompt")]
    public TextMeshProUGUI promptUI;

    [Header("Audio Settings")]
    public AudioClip pickupSound; // خانة لملف الصوت

    void Start()
    {
        if (promptUI != null)
        {
            promptUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (playerObject != null)
            {
                playerObject.GetComponent<PlayerMovement>().hasKey = true;
            }

            if (promptUI != null)
            {
                promptUI.gameObject.SetActive(false);
            }

            // تشغيل الصوت في مكان المفتاح قبل حذفه
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // حذف المفتاح من اللعبة
            Destroy(gameObject);
            Debug.Log("تم التقاط المفتاح!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerObject = other.gameObject;

            if (promptUI != null)
            {
                promptUI.text = ArabicFixer.Fix("اضغط E لالتقاط المفتاح");
                promptUI.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (promptUI != null)
            {
                promptUI.gameObject.SetActive(false);
            }
        }
    }
}