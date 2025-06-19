using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private Rigidbody2D playerRigidbody;
	[SerializeField] private float moveSpeed = 6f;
	[SerializeField] private float jumpForce = 10f;
	[SerializeField] public Animator playerAnimator;
	[SerializeField] private BoxCollider2D playerCollider;
	[SerializeField] private LayerMask terrainLayer;
	[SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] private float climbSpeed = 3f;
    private float vertical;
	private int coinCount = 0;
    private bool isClimbing = false;
    private bool isOnLadder = false;
	private bool canMove = true;

    private bool jumpCheck = false;
	private bool facingRight = true;
    private GameObject currentPlatform = null;

	void Update()
	{
		if (canMove)
		{
			Movement();
			UpdateAnimator();
		}
		ClimbLadder();
    }

    private void FixedUpdate()
    {
        if (isClimbing == true)
        {
            playerRigidbody.gravityScale = 0f;
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, vertical * climbSpeed);
        }
        else
        {
            playerRigidbody.gravityScale = 2f;
        }
    }

    private void ClimbLadder()
	{
        vertical = Input.GetAxis("Vertical");

        if (isOnLadder == true && Mathf.Abs(vertical) > .1f)
        {
            isClimbing = true;
			playerAnimator.SetBool("IsClimb", true);
		}
		else
		{
			isClimbing = false;
        }
    }
	private void Movement()
	{
		if(!canMove) return;
        float horizontal = Input.GetAxis("Horizontal");
		playerRigidbody.velocity = new Vector2(horizontal * moveSpeed, playerRigidbody.velocity.y);
		if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
		{
			playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
		}
		if (IsGrounded() == true)
		{
			jumpCheck = true;
		}
		if (IsGrounded() == false && jumpCheck == true && Input.GetKeyDown(KeyCode.Space))
		{
			playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
			jumpCheck = false;
		}

		//Bấm S khi đứng trên platform sẽ rơi xuống
		if (Input.GetKeyDown(KeyCode.S))
		{
			if (currentPlatform != null)
			{
				PlatformController platformController = currentPlatform.GetComponent<PlatformController>();
				if (platformController != null)
				{
					platformController.DropPlayer();
					playerRigidbody.AddForce(Vector2.down * 2f, ForceMode2D.Impulse);
				}
			}
		}
	}

	private void UpdateAnimator()
	{
		if(!canMove) return;
        if (playerRigidbody.velocity.x < 0)
		{
            if (facingRight) Flip();
        }
		else if (playerRigidbody.velocity.x > 0)
		{
			if (!facingRight) Flip();
        }

		if (Math.Abs(playerRigidbody.velocity.x) > 0.1f)
		{
			playerAnimator.SetBool("IsMove", true);
		}
		else
		{
			playerAnimator.SetBool("IsMove", false);
		}
		if (playerRigidbody.velocity.y > .1f)
		{
			playerAnimator.SetInteger("State", 1);
		}
		else if (playerRigidbody.velocity.y < -.1f)
		{
			playerAnimator.SetInteger("State", -1);
		}
		else
		{
			playerAnimator.SetInteger("State", 0);
		}
	}

	private bool IsGrounded()
	{
		return Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f,
			Vector2.down, 0.1f, terrainLayer);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Platform"))
		{
			ContactPoint2D[] contacts = new ContactPoint2D[1];
			collision.GetContacts(contacts);
			if (contacts[0].normal.y > 0.5f)
			{
				currentPlatform = collision.gameObject;
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject == currentPlatform)
		{
			currentPlatform = null;
		}
	}
	private void Flip()
	{
		facingRight = !facingRight;
		transform.Rotate(0f, 180f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
        if (other.CompareTag("Coin"))
        {
			coinCount++;
			coinText.text = "" + coinCount;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
			playerAnimator.SetBool("IsClimb", false);
            isOnLadder = false;
            isClimbing = false;
        }
    }
    public void TriggerDeathAnimation()
    {
        canMove = false;
        playerAnimator.SetTrigger("IsDeath");
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.isKinematic = true;
    }
}