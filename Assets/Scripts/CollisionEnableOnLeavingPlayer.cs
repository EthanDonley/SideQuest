using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEnableOnLeavingPlayer : MonoBehaviour
{
    public Collider2D target;

    private void OnTriggerExit2D ( Collider2D collision )
    {
        target.enabled = true;
    }
}
