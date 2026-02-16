using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private string _sceneToLoad; 

    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        if (!string.IsNullOrEmpty(_sceneToLoad))
        {
            SceneManager.LoadScene(_sceneToLoad);
            return true;
        }

        Debug.LogWarning("No scene assigned ");
        return false;
    }
}