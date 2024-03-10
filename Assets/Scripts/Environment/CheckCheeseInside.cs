using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCheeseInside : MonoBehaviour
{

    public bool isCheeseInside;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            isCheeseInside = true;
            Debug.Log("cheese inside");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            isCheeseInside = false;
            Debug.Log("cheese not inside");
        }
    }


}
