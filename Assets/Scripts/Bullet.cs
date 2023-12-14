using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    public bool isRotate;

    void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * 10);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Delete bullets when they leave the screen
        if ( collision.gameObject.tag == "BorderBullet" )
        {
            gameObject.SetActive(false);
        }
    }
}
