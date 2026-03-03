using System;
using UnityEngine;
using FishNet.Object;

public class PlayerInteractor : NetworkBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform cameraTransform;

    // UI Eventleri
    public event Action<string> OnInteractableHit;
    public event Action OnInteractStart;  // YENİ: Dolmaya başla
    public event Action OnInteractCancel; // YENİ: Dolmayı iptal et/sıfırla

    private IInteractable currentInteractable;
    private bool isInteracting; // YENİ: Şu an basılı tutuyor muyuz?

    private void Update()
    {
        if (!base.IsOwner) return;
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactableObj))
            {
                if (interactableObj != currentInteractable)
                {
                    ClearInteractable(); // Eski objeyi temizle (varsa)

                    currentInteractable = interactableObj;
                    currentInteractable.OnFocus();
                    OnInteractableHit?.Invoke(currentInteractable.GetInteractPrompt());
                }
                return; 
            }
        }
        ClearInteractable();
    }

    private void ClearInteractable()
    {
        if (currentInteractable != null)
        {
            // Eğer basılı tutarken kafamızı objeden çekersek, etkileşimi anında İPTAL ET
            if (isInteracting) CancelInteract();

            currentInteractable.OnLoseFocus();
            currentInteractable = null;
            OnInteractableHit?.Invoke(null);
        }
    }

    // --- YENİ ETKİLEŞİM METODLARI ---

    public void StartInteract()
    {
        if (!base.IsOwner || currentInteractable == null) return;
        
        isInteracting = true;
        OnInteractStart?.Invoke(); // UI'a dolmaya başla de
    }

    public void CompleteInteract()
    {
        if (!base.IsOwner || currentInteractable == null || !isInteracting) return;
        
        isInteracting = false;
        currentInteractable.OnInteract(this.gameObject); // Eşyayı al / Kapıyı aç
        OnInteractCancel?.Invoke(); // UI barını sıfırla
    }

    public void CancelInteract()
    {
        if (!isInteracting) return;

        isInteracting = false;
        OnInteractCancel?.Invoke(); // UI barını sıfırla
    }
}