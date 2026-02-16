using UnityEngine;
using System;

public class DanceOnAwake : MonoBehaviour
{
    public static Action DanceInHouse;

    void Start()
    {
        DanceInHouse?.Invoke();
    }

   
}
