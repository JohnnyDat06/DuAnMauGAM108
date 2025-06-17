using System;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private Rigidbody2D bulletRigidBody;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SerDirection(Vector2 direction)
    {
        bulletRigidBody.velocity = direction.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {       
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
