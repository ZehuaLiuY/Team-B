using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAffect : MonoBehaviour
{
    private CheeseSmellController _cheeseSmellController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            // Debug.Log("cheese in trigger");
            _cheeseSmellController = other.GetComponent<CheeseSmellController>();
            _cheeseSmellController.setWind(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            // Debug.Log("cheese leave trigger");
            _cheeseSmellController = other.GetComponent<CheeseSmellController>();
            _cheeseSmellController.setWind(false);
        }
    }
}
