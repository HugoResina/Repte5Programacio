using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

[System.Serializable]
public class SaveData
{
    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;

    public bool hasItemEquiped;
}

public class GayManager : MonoBehaviour
{
    public bool hasItemEquiped = false;
    public static GayManager GayManagerInstance;
    public GameObject HatModel;
    public GameObject Player;

    private CharacterController cc;

    private void Awake()
    {
        
        if (GayManagerInstance == null)
        {
            GayManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

       
        cc = Player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(LoadGameDelayed());

        if (cc != null) cc.enabled = true;
    }
    private System.Collections.IEnumerator LoadGameDelayed()
    {

        yield return new WaitForEndOfFrame();

        var tpc = Player.GetComponent<ThirdPersonController>();
        var cc = Player.GetComponent<CharacterController>();

        if (tpc != null) tpc.enabled = false;
        if (cc != null) cc.enabled = false;

        string path = Application.dataPath + "/savegame.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Vector3 loadedPosition = new Vector3(data.posX, data.posY, data.posZ);
            Quaternion loadedRotation = Quaternion.Euler(data.rotX, data.rotY, data.rotZ);

            
            Player.transform.position = loadedPosition;
            Player.transform.rotation = loadedRotation;



            hasItemEquiped = data.hasItemEquiped;
            if (HatModel != null)
                HatModel.SetActive(hasItemEquiped);
        }

        if (cc != null) cc.enabled = true;
        if (tpc != null) tpc.enabled = true;

        Debug.Log("Game Loaded!");
    }
    private void OnEnable()
    {
        ThirdPersonController.OnSaveGame += SaveGame;
    }

    private void OnDisable()
    {
        ThirdPersonController.OnSaveGame -= SaveGame;
    }

    void SaveGame()
    {
        Debug.Log("GayManager saving game...");

        SaveData data = new SaveData();

        data.posX = Player.transform.position.x;
        data.posY = Player.transform.position.y;
        data.posZ = Player.transform.position.z;

        data.rotX = Player.transform.eulerAngles.x;
        data.rotY = Player.transform.eulerAngles.y;
        data.rotZ = Player.transform.eulerAngles.z;

        data.hasItemEquiped = hasItemEquiped;

        string json = JsonUtility.ToJson(data, true);
        string path = Application.dataPath + "/savegame.json";

        File.WriteAllText(path, json);
        Debug.Log("Game saved at: " + path);
    }

    void LoadGame()
    {
        string path = Application.dataPath + "/savegame.json";

        if (!File.Exists(path))
        {
            Debug.Log("No save file found.");
            return;
        }

        Debug.Log("Loading save file...");
        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Vector3 loadedPosition = new Vector3(data.posX, data.posY, data.posZ);
        Quaternion loadedRotation = Quaternion.Euler(data.rotX, data.rotY, data.rotZ);

        Teleport(loadedPosition, loadedRotation);

        hasItemEquiped = data.hasItemEquiped;

        if (HatModel != null)
            HatModel.SetActive(hasItemEquiped);

        Debug.Log("Game Loaded!");
    }
    public void Teleport(Vector3 position, Quaternion rotation)
    {
        CharacterController cc = Player.GetComponent<CharacterController>();
        ThirdPersonController tpc = Player.GetComponent<ThirdPersonController>();

        if (cc != null) cc.enabled = false;
        if (tpc != null) tpc.enabled = false;

        Player.transform.position = position;
        Player.transform.rotation = rotation;

        if (cc != null) cc.enabled = true;
        if (tpc != null) tpc.enabled = true;
    }
}