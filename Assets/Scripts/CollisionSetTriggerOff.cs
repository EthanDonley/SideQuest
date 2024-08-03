using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSetTriggerOff : MonoBehaviour
{
    // Reenable the physics with the player once the collision ends
    private void OnTriggerExit2D ( Collider2D collision )
    {
        GetComponent<Collider2D> ().isTrigger = false;
    }
}
