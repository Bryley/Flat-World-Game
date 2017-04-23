using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : MonoBehaviour {

	public GameObject player;

	Rigidbody2D rb;
	BoxCollider2D collider;
	Animator animator;
	public float walkSpeed = 1f;
	public float runSpeed = 2.5f;
	public float viewDistance = 5f;

	// 1 = left, 2 = top, 3 = right, 4 = down
	int lastFacing = 0;

	bool isMoving;

	//AI stuff
	int timeTillNextStep;
	Vector2 direction;

	bool isAttacking;
	bool underAttack;
	bool dying;

	public int minTimeWait = 5;
	public int maxTimeWait = 100;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		collider = GetComponent<BoxCollider2D> ();
		animator = GetComponent<Animator> ();

		lastFacing = 1;

		isAttacking = false;
		underAttack = false;

		timeTillNextStep = Random.Range (minTimeWait, maxTimeWait);
		direction = Vector2.zero;

		dying = false;

	}

	void Update(){

		if(isMoving){
			if(getFacing() == 1 || getFacing() == 2){
				animator.Play ("EnemyWalkLeft");
			}else if(getFacing() == 3 || getFacing() == 4){
				animator.Play ("EnemyWalkRight");
			}
		}else{
			if (underAttack == false) {
				if (getFacing () == 1 || getFacing () == 2) {
					animator.Play ("EnemyIdleLeft");
				} else if (getFacing () == 3 || getFacing () == 4) {
					animator.Play ("EnemyIdleRight");
				}
			}
		}

	}

	void FixedUpdate () {

		//Define collision boxes around player.
		Vector2 leftArea1 = new Vector2(collider.bounds.min.x, collider.bounds.min.y);
		Vector2 leftArea2 = new Vector2(collider.bounds.min.x-collider.bounds.extents.x, collider.bounds.min.y+collider.bounds.extents.y*2);

		Vector2 rightArea1 = new Vector2(collider.bounds.max.x, collider.bounds.max.y);
		Vector2 rightArea2 = new Vector2(collider.bounds.max.x+collider.bounds.extents.x, collider.bounds.max.y-collider.bounds.extents.y*2);

		Vector2 topArea1 = new Vector2(collider.bounds.max.x, collider.bounds.max.y);
		Vector2 topArea2 = new Vector2(collider.bounds.max.x-collider.bounds.extents.x*2, collider.bounds.max.y+collider.bounds.extents.y);

		Vector2 bottomArea1 = new Vector2(collider.bounds.min.x, collider.bounds.min.y);
		Vector2 bottomArea2 = new Vector2(collider.bounds.min.x+collider.bounds.extents.x*2, collider.bounds.min.y-collider.bounds.extents.y);
		// ===

		float xVelocity = direction.x;
		float yVelocity = direction.y;

		if (isAttacking == false) {
			timeTillNextStep--;

			if (timeTillNextStep <= 0) {
				if (direction.Equals (Vector2.zero)) {
					timeTillNextStep = Random.Range (minTimeWait, maxTimeWait);
					xVelocity = Random.Range (-1, 2) * walkSpeed;
					yVelocity = Random.Range (-1, 2) * walkSpeed;
					direction = new Vector2 (xVelocity, yVelocity);
				} else {
					timeTillNextStep = Random.Range (minTimeWait, maxTimeWait);
					direction = Vector2.zero;
				}
			}

			Vector2 playerPos = player.transform.position;
			Vector2 aiPos = transform.position;

			Vector2 difference = playerPos - aiPos;

			if(difference.magnitude <= viewDistance){
				isAttacking = true;
			}

		}else{
			//Follow and attack player;

			Vector2 playerPos = player.transform.position;
			Vector2 aiPos = transform.position;

			Vector2 difference = playerPos - aiPos;

			int x = 0;
			int y = 0;

			if(difference.x > 0.11){
				x = 1;
			}else if(difference.x < -0.11){
				x = -1;
			}
			if(difference.y > 0.11){
				y = 1;
			}else if(difference.y < -0.11){
				y = -1;
			}

			if(difference.magnitude <= 1.5){
				x = 0;
				y = 0;
				if (underAttack == false) {
					attack ();
				}
			}

			if(underAttack){
				x = 0;
				y = 0;
				timeTillNextStep--;
				if(timeTillNextStep <= 0){
					underAttack = false;
					if(difference.magnitude <= 1.5f){
						if (dying == false) {
							Player script = (Player)player.GetComponent (typeof(Player));

							script.health--;
						}
					}
				}
			}

			direction = new Vector2 (x*runSpeed, y*runSpeed);
		}

		if(dying){
			direction = Vector2.zero;
			timeTillNextStep--;

			if(timeTillNextStep <= 0){
				dying = false;
				Destroy (gameObject);
			}
		}

		// ======
		//Restricts player's movement:
		// ======

		//Is one because it finds the players collider.
		if (Physics2D.OverlapAreaAll (leftArea1, leftArea2).Length <= 1){
			if(xVelocity < 0){
				xVelocity = 0;
				timeTillNextStep = 0;
			}
		}
		if (Physics2D.OverlapAreaAll (rightArea1, rightArea2).Length <= 1){
			if(xVelocity > 0){
				xVelocity = 0;
				timeTillNextStep = 0;
			}
		}
		if (Physics2D.OverlapAreaAll (topArea1, topArea2).Length <= 1){
			if(yVelocity > 0){
				yVelocity = 0;
				timeTillNextStep = 0;
			}
		}
		if (Physics2D.OverlapAreaAll (bottomArea1, bottomArea2).Length <= 1){
			if(yVelocity < 0){
				yVelocity = 0;
				timeTillNextStep = 0;
			}
		}

		Vector2 vel = new Vector2 (xVelocity, yVelocity);


		rb.velocity = vel;

		if(vel.x == 0 && vel.y == 0){
			isMoving = false;
		} else {
			isMoving = true;
		}

		bool xDom = Mathf.Abs(vel.x) >= Mathf.Abs(vel.y);

		if (vel.x < 0 && xDom) {
			lastFacing = 1;
		} else if (vel.y > 0 && xDom == false) {
			lastFacing = 2;
		} else if (vel.x > 0 && xDom) {
			lastFacing = 3;
		} else if (vel.y < 0 && xDom == false) {
			lastFacing = 4;
		}

	}

	public int getFacing(){
		return lastFacing;
	}


	void attack(){
		underAttack = true;
		isMoving = false;
		timeTillNextStep = 25;
		if(getFacing() == 1 || getFacing() == 2){
			animator.Play ("EnemyAttackLeft");
		}else if(getFacing() == 3 || getFacing() == 4){
			animator.Play ("EnemyAttackRight");
		}

	}

	public void die(){
		dying = true;
		timeTillNextStep = 30;
		if(getFacing() == 1 || getFacing() == 2){
			animator.Play ("EnemyDeathLeft");
		}else if(getFacing() == 3 || getFacing() == 4){
			animator.Play ("EnemyDeathRight");
		}
	}
}
