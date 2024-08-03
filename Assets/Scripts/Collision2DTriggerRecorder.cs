using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision2DTriggerRecorder : MonoBehaviour
{
    public List<Collider2D> collisions = new List<Collider2D> ();

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        collisions.Add ( collision );
    }
    private void OnTriggerExit2D ( Collider2D collision )
    {
        collisions.Remove ( collision );
    }
}
