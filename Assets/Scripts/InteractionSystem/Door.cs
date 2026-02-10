using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        if (GayManager.GayManagerInstance.hasItemEquiped)
        {
            Debug.Log("m'obro");
            return true;
        }
        else
        {
            Debug.Log("falta l'item clau");
            return true;
        }
    }
}
