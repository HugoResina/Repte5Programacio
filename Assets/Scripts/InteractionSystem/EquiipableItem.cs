using UnityEngine;

public class EquiipableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt; 
    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("interacted");
        GayManager.GayManagerInstance.hasItemEquiped = true;
        GayManager.GayManagerInstance.HatModel.SetActive(true);
        this.gameObject.SetActive(false);
        return true;
    }
}
