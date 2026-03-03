using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private GameObject interactPanel;

    [Header("Hold Settings")]
    [SerializeField] private Image fillImage;        // YENİ: Dolacak olan resim
    [SerializeField] private float holdTime = 0.4f;  // YENİ: Input Action'daki Hold süresiyle AYNI olmalı

    private Coroutine fillCoroutine;

    private void OnEnable()
    {
        if (playerInteractor != null)
        {
            playerInteractor.OnInteractableHit += UpdateUI;
            playerInteractor.OnInteractStart += StartFillProgress;
            playerInteractor.OnInteractCancel += CancelFillProgress;
        }

        if (playerEquipment != null)
        {
            playerEquipment.OnDropStart += HandleDropStart;
            playerEquipment.OnDropCancel += HandleDropCancel;
        }
    }

    private void OnDisable()
    {
        if (playerInteractor != null)
        {
            playerInteractor.OnInteractableHit -= UpdateUI;
            playerInteractor.OnInteractStart -= StartFillProgress;
            playerInteractor.OnInteractCancel -= CancelFillProgress;
        }

        if (playerEquipment != null)
        {
            playerEquipment.OnDropStart -= HandleDropStart;
            playerEquipment.OnDropCancel -= HandleDropCancel;
        }
    }

    private void UpdateUI(string promptMessage)
    {
        if (string.IsNullOrEmpty(promptMessage))
        {
            interactPanel.SetActive(false);
            promptText.text = "";
            CancelFillProgress(); // UI kapanırsa dolumu da iptal et
        }
        else
        {
            interactPanel.SetActive(true);
            promptText.text = promptMessage;

            // UI açıldığında barın sıfır olduğundan emin ol
            if (fillImage != null) fillImage.fillAmount = 0f;
        }
    }

    private void HandleDropStart(string promptMsg)
    {
        UpdateUI(promptMsg);
        StartFillProgress();
    }

    private void HandleDropCancel()
    {
        UpdateUI(null);
        CancelFillProgress();
    }

    // --- YENİ UI ANİMASYONLARI ---

    private void StartFillProgress()
    {
        if (fillImage == null) return;

        if (fillCoroutine != null) StopCoroutine(fillCoroutine);
        fillCoroutine = StartCoroutine(FillRoutine());
    }

    private void CancelFillProgress()
    {
        if (fillImage == null) return;

        if (fillCoroutine != null) StopCoroutine(fillCoroutine);
        fillImage.fillAmount = 0f; // Barı anında sıfırla
    }

    private IEnumerator FillRoutine()
    {
        float timer = 0f;
        fillImage.fillAmount = 0f;

        // holdTime (0.4s) boyunca barı 0'dan 1'e doğru pürüzsüzce doldur
        while (timer < holdTime)
        {
            timer += Time.deltaTime;
            fillImage.fillAmount = timer / holdTime;
            yield return null; // Bir sonraki frame'i bekle (Update gibi çalışır ama işi bitince ölür)
        }

        fillImage.fillAmount = 1f;
    }
}