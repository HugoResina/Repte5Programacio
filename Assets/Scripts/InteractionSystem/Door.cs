using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private string _prompt;
    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        if (GayManager.GayManagerInstance.hasItemEquiped)
        {
            //abrirse
            return true;
        }
        else
        {
            Debug.Log("falta l'item clau");
            return true;
        }
    }
}
