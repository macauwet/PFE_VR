using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDetection : MonoBehaviour
{
    public bool col_left_hand = false;
    public bool col_right_hand = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Left"))
        {
            col_left_hand = true;
        }   
        
        if (other.gameObject.CompareTag("Right"))
        {
            col_right_hand = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Left"))
        {
            col_left_hand = false;
        }

        if (other.gameObject.CompareTag("Right"))
        {
            col_right_hand = false;
        }
    }
}
