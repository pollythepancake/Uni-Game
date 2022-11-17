using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerAttack : MonoBehaviour
{
    public Tilemap Tilemap;
    public ContactPoint2D[] contacts = new ContactPoint2D[10];
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Breakable")
        {
            Debug.Log("Hit tilemap!");
            int contactCount = collision.contactCount;
            if (contactCount > contacts.Length)
                contacts = new ContactPoint2D[contactCount];
            collision.GetContacts(contacts);

            Vector3 hitPosition = Vector3.zero;
            for (int i = 0; i != contactCount; ++i)
            {
                hitPosition.x = contacts[i].point.x;
                hitPosition.y = contacts[i].point.y;
                collision.gameObject.GetComponent<Tilemap>().SetTile(collision.gameObject.GetComponent<Tilemap>().WorldToCell(hitPosition), null);
            }
        }
    }
}