using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour {
	
	Rigidbody2D rb;
	BoxCollider2D collider;
	Animator animator;
	public float speed = 3f;

	// 1 = left, 2 = top, 3 = right, 4 = down
	int lastFacing = 0;

	bool isMoving;

	//AI stuff
	int timeTillNextStep;
	Vector2 direction;

	public int minTimeWait = 500;
	public int maxTimeWait = 3000;

	void Start () {
		Time.timeScale = 1;
		rb = GetComponent<Rigidbody2D>();
		collider = GetComponent<BoxCollider2D> ();
		animator = GetComponent<Animator> ();

		lastFacing = 1;

		timeTillNextStep = Random.Range (minTimeWait, maxTimeWait);
		direction = Vector2.zero;

	}

	void Update(){

		if(isMoving){
			if(getFacing() == 1 || getFacing() == 2){
				animator.Play ("PlayerWalkLeft");
			}else if(getFacing() == 3 || getFacing() == 4){
				animator.Play ("PlayerWalkRight");
			}
		}else{
			if(getFacing() == 1 || getFacing() == 2){
				animator.Play ("PlayerIdleLeft");
			}else if(getFacing() == 3 || getFacing() == 4){
				animator.Play ("PlayerIdleRight");
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

		timeTillNextStep--;

		if(timeTillNextStep <= 0){
			if (direction.Equals (Vector2.zero)) {
				timeTillNextStep = Random.Range (minTimeWait, maxTimeWait);
				xVelocity = Random.Range(-1, 2) * speed;
				yVelocity = Random.Range (-1, 2) * speed;
				direction = new Vector2 (xVelocity, yVelocity);
			} else{
				timeTillNextStep = Random.Range (minTimeWait, maxTimeWait);
				direction = Vector2.zero;
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
}
