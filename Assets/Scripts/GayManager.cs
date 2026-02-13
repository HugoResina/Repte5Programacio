using UnityEngine;

public class GayManager : MonoBehaviour
{
    public bool hasItemEquiped = false;

    public static GayManager GayManagerInstance;
    public GameObject HatModel;

    //save game
    
    private void Awake()
    {
        /*if (hasItemEquiped)
        {
            activar modelo item
           
        }*/
       
        if (GayManagerInstance == null)
        {
            GayManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
