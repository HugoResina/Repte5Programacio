using UnityEngine;

public class ItemMoveAnim : MonoBehaviour
{
   
   
    void Update()
    {
        Vector3 itemRotation = new Vector3(0, 1, 0);
        Vector3 itemHeight = new Vector3(0, Mathf.Sin(Time.time) * 0.008f, 0);

        transform.localEulerAngles += itemRotation;
        transform.position += itemHeight;
    }
}
