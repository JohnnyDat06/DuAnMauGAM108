using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private Animator anim;
    [SerializeField] private float climbSpeed = 3f;

    private float vertical;
    private bool isClimbing = false;
    private bool isOnLadder = false;
 
    void Update()
    {
        vertical = Input.GetAxis("Vertical");

        if (isOnLadder && Mathf.Abs(vertical) > .1f)
        {
            isClimbing = true;
            anim.SetBool("IsClimb", true);
        }
        else
        {
            anim.SetBool("IsClimb", false);
        }
    }

    private void FixedUpdate()
    {
        if(isClimbing)
        {
            playerRigidbody.gravityScale = 0f;
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, vertical * climbSpeed);
        }
        else
        {
            playerRigidbody.gravityScale = 2f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Ladder"))
        {
            isOnLadder = true;           
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Ladder"))
        {
            isOnLadder = false;
            isClimbing = false;
            anim.SetBool("IsClimb", false);
        }
    }
}
