using UnityEngine;
public interface IInteractable
{
    void OnInteract(GameObject interactorPlayer);

    void OnFocus();

    void OnLoseFocus();

    string GetInteractPrompt();
}