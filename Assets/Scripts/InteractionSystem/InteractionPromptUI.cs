using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private TextMeshProUGUI _promptText;
    private void Start()
    {
        _camera = Camera.main;
        _uiPanel.SetActive(false);
    }

    private void Update()
    {
        var rotation = _camera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public bool isDisplayed = false;
    public void SetUp(string promptText)
    {
        _promptText.text = promptText;
        _uiPanel.SetActive(true);
        isDisplayed = true;
    }
    public void Close()
    {
        isDisplayed = false;
        _uiPanel?.SetActive(false);
    }
}
