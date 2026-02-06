using UnityEngine;

public class AimController : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject AimCamera;
    //public GameObject Site;
    public static AimController AimContInstance;
    public bool isAiming= false;
    private void Awake()
    {
        if (AimContInstance == null)
        {
            AimContInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        if(isAiming && !AimCamera.activeInHierarchy)
        {
            MainCamera.SetActive(false);
            AimCamera.SetActive(true);
        }
        else if (!isAiming && !MainCamera.activeInHierarchy)
        {
            MainCamera.SetActive(true);
            AimCamera.SetActive(false);
        }
    }

}
