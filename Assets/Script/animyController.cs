using UnityEngine;
using UnityEngine.UI;
using System;


public class animyController : MonoBehaviour {
	private Rigidbody2D enemy_rigid_body;
	private Vector2 initPos;
	private Transform player_transform;
	private float health;
	private bool faceRight;
	private bool hit;
	// Use this for initialization
	private Animator animator;
	private GameObject player;
	private playerController pc;
	private float nextUpdate;
	
	
	
	private scenelistener scenelistener;
	void Awake () {
		faceRight = false;
		hit = false;
		nextUpdate = 0;
		
		initPos = new Vector2 (transform.position.x, transform.position.y);
		enemy_rigid_body = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag("Player");
		scenelistener = GameObject.FindGameObjectWithTag("scene").GetComponent<scenelistener>();
		pc = player.GetComponent<playerController>();
		
		enemy_rigid_body.velocity = new Vector2(-11f,0);
		enemy_rigid_body.freezeRotation = true;
		health = 20;
		player_transform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float diff = player_transform.position.x - transform.position.x;
		float diff_y = player_transform.position.y - transform.position.y;
		//Debug.Log(diff);
		if (Math.Abs(diff)>2.1f && Math.Abs(diff)< 8 && !hit){
			animator.SetBool("attack",false);
			if (player_transform.position.x-transform.position.x>0){
				enemy_rigid_body.velocity = new Vector2(3f,0);
			}else{
				enemy_rigid_body.velocity = new Vector2(-3f,0);
			}
		}else if((Math.Abs(diff)<2.1f || Math.Abs(diff)==2.1f) && Math.Abs(diff_y)<1){
			enemy_rigid_body.velocity = new Vector2(0,0);
			//Debug.Log("contact");
			if(Time.time > nextUpdate){
				//Debug.Log("attack");
				Attack();
				nextUpdate = Time.time + 1;
			}
			
		}else{
			if (initPos.x-enemy_rigid_body.position.x>7f) {
				enemy_rigid_body.velocity = new Vector2(11f,0);
			}
			if (initPos.x - enemy_rigid_body.position.x < -7) {
				enemy_rigid_body.velocity = new Vector2 (-11f, 0);
			}
		}

		if (enemy_rigid_body.velocity.x > 0 && !faceRight){
			Flip();
		}else if (enemy_rigid_body.velocity.x < 0 && faceRight){
			Flip();
		}

		// damage_image.color = Color.Lerp(damage_image.color,Color.clear,Time.deltaTime*5f);

		if(health<0){
			this.gameObject.SetActive(false);
			scenelistener.deCount();
		}
	}

	void Flip(){
		faceRight = !faceRight;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}	

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("punch")){
			//Debug.Log("Enemy get punched");
			pc.IncreaseEnergy(5);
			reduceHealth(5);
		}
	}
	
	void Attack(){
		animator.SetBool("attack",true);
		pc.DecreaseHealth(10);
		//damage_image.color = flash_color;
	}

	public void reduceHealth(float attack){
		health -= attack;
	}


}
