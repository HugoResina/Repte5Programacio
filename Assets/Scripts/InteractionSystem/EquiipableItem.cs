using UnityEngine;

public class EquiipableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt; 
    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("interacted");
        return true;
        GayManager.GayManagerInstance.hasItemEquiped = true;
    }
}
