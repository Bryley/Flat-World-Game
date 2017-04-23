using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour {

	Rigidbody2D rb;
	Animator animator;
	public float speed = 3f;

	// 1 = left, 2 = top, 3 = right, 4 = down
	int lastFacing = 0;

	bool gettingWood;
	bool isMoving;

	int count = 0;
	public int countTo = 50;

	public int health = 3;

	bool underAttack;

	int timeTilNextStep;


	BoxCollider2D collider;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		collider = GetComponent<BoxCollider2D> ();
		animator = GetComponent<Animator> ();

		lastFacing = 1;
		gettingWood = false;

		underAttack = false;

		timeTilNextStep = 40;

	}

	void Update(){

		count++;
		if(count >= countTo){
			count = 0;
			if (gettingWood) {
				GameManager.instance.addWood (1);
			}
		}

		if (underAttack == false) {
			if (isMoving) {
				if (getFacing () == 1 || getFacing () == 2) {
					animator.Play ("PlayerWalkLeft");
				} else if (getFacing () == 3 || getFacing () == 4) {
					animator.Play ("PlayerWalkRight");
				}
			} else {
				if (getFacing () == 1 || getFacing () == 2) {
					animator.Play ("PlayerIdleLeft");
				} else if (getFacing () == 3 || getFacing () == 4) {
					animator.Play ("PlayerIdleRight");
				}
			}
		}

		if(health <= 0){
			StartCoroutine(GameManager.instance.playerDeath ());
		}

		if(timeTilNextStep > 0){
			timeTilNextStep--;
			if(timeTilNextStep <= 0){
				underAttack = false;
				timeTilNextStep = 0;
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

		float xVelocity = Input.GetAxis ("Horizontal") * speed;
		float yVelocity = Input.GetAxis ("Vertical") * speed;

		//Check facing value.
		bool xDom = Mathf.Abs(xVelocity) >= Mathf.Abs(yVelocity);

		if(xVelocity < 0 && xDom){
			lastFacing = 1;
		}else if(yVelocity > 0 && xDom == false){
			lastFacing = 2;
		}else if(xVelocity > 0 && xDom){
			lastFacing = 3;
		}else if(xVelocity < 0 && xDom == false){
			lastFacing = 4;
		}

		// ======
		//Restricts player's movement:
		// ======

		//Is one because it finds the players collider.
		if (Physics2D.OverlapAreaAll (leftArea1, leftArea2).Length <= 1){
			if(xVelocity < 0){
				xVelocity = 0;
			}
		}
		if (Physics2D.OverlapAreaAll (rightArea1, rightArea2).Length <= 1){
			if(xVelocity > 0){
				xVelocity = 0;
			}
		}
		if (Physics2D.OverlapAreaAll (topArea1, topArea2).Length <= 1){
			if(yVelocity > 0){
				yVelocity = 0;
			}
		}
		if (Physics2D.OverlapAreaAll (bottomArea1, bottomArea2).Length <= 1){
			if(yVelocity < 0){
				yVelocity = 0;
			}
		}

		Vector2 vel = new Vector2 (xVelocity, yVelocity);
		

		rb.velocity = vel;

		if(vel.x == 0 && vel.y == 0){
			isMoving = false;
		} else {
			isMoving = true;
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Tree")) {
			gettingWood = true;
		}else if(other.CompareTag("Sign")){
			Sign script = (Sign) other.gameObject.GetComponent(typeof(Sign));
			GameManager.instance.foundIsland(script.getIslandName ());

			GameManager.instance.openSignMenu (script.getIslandName ());

		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (other.CompareTag ("Tree")) {
			gettingWood = false;
		}else if(other.CompareTag("Sign")){
			GameManager.instance.closeSignMenu ();
		}
	}

	public Vector2 getHoloPos(){
		Vector2 hologramPos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

		if (lastFacing == 1) {
			hologramPos += Vector2.left;
		}
		else if(lastFacing == 2){
			hologramPos += Vector2.up;
		}
		else if(lastFacing == 3){
			hologramPos += Vector2.right;
		}
		else if(lastFacing == 4){
			hologramPos += Vector2.down;
		}
		if (lastFacing == 0) {
			return new Vector2 (5000f, 5000f);
		}

		return hologramPos;
	}

	public int getFacing(){
		return lastFacing;
	}

	public void attack(){
		underAttack = true;
		timeTilNextStep = 40;
		if (getFacing () == 1 || getFacing () == 2) {
			animator.Play ("PlayerAttackLeft");
		}else if(getFacing() == 3 || getFacing() == 4){
			animator.Play ("PlayerAttackRight");
		}

		Collider2D[] colliders = Physics2D.OverlapCircleAll (transform.position, 1.5f);

		foreach(Collider2D coll in colliders){
			GameObject obj = coll.gameObject;

			if(obj.CompareTag("Enemy")){
				AIEnemy script = (AIEnemy)obj.GetComponent (typeof(AIEnemy));
				script.die ();
				continue;
			}
		}

	}
}
