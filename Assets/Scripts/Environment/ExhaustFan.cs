using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustFan : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            CheeseSmellController cheeseSmellController = other.GetComponent<CheeseSmellController>();
            cheeseSmellController.setFan(true, transform);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            CheeseSmellController cheeseSmellController = other.GetComponent<CheeseSmellController>();
            cheeseSmellController.setFan(false, null);
        }
    }
}
