using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using MidiJack;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour {

	private Rigidbody2D rb2d; 
	public Rigidbody2D bullet;
	public Rigidbody2D hp_fruit;
	public Rigidbody2D punch;
	private Animator animator;
	private bool faceRight = true;
	private bool grounded = true;
	private float ceilingRadius = .01f;
	private float groundedRadius = .3f;
	private float crouchSpeed = .36f;
	private bool airControl = true;
    float moveH = 0;
	float moveV = 0; 
	float nextupdate=0;
	float deltatime = 1f;
	[SerializeField] private LayerMask whatIsGround;
	public Image hp_bar;
	float max_health = 100;
	float cur_health = 100;
	public Image mp_bar;
	float max_energy = 100;
	float cur_energy = 0;
	bool punch_lock = false;
	float punch_update = 0;
	float pre_entropy = 0;
	int inc_count = 0;
	private bool dead;
	private bool finished;
	public Text finish;
	private float finish_time;
	private Transform groundCheck;
	private Transform ceilingCheck;

	private PlayerHealth player_health;


	void Start(){
		rb2d = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		player_health = GetComponent<PlayerHealth>();
		groundCheck = transform.Find ("groundcheck");
		ceilingCheck = transform.Find ("ceilcheck");
		punch.gameObject.SetActive(false);
		rb2d.freezeRotation = true;
		dead = false;
		mp_bar.transform.localScale = new Vector3(0,mp_bar.transform.localScale.y,mp_bar.transform.localScale.z);
		finish_time = -1;
		finished = false;
	}
	
	
	// Update is called once per frame
	void FixedUpdate () {
		
		// Debug.Log(Screen.width);


		if (MidiMaster.GetEntropyBuff().Count >= 12){
			float cur_entropy = entropy(MidiMaster.GetEntropyBuff());
			// Debug.Log(cur_entropy);
			float diff_entropy = cur_entropy - pre_entropy;
			
			if ((cur_entropy-pre_entropy)>0.3){
					Debug.Log("bingo!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
					Rigidbody2D hp_fruit_ins = Instantiate(hp_fruit);
					hp_fruit_ins.transform.position = new Vector3(rb2d.transform.position.x+10,rb2d.transform.position.y+20,rb2d.transform.position.z);
					inc_count = 0;
				
			}
			pre_entropy = cur_entropy;
			
		}

		if (cur_energy>=20&&checkMoon(MidiMaster.GetTriBuff())){
			moon_attack();
			MidiMaster.ClearTriBuff();
			cur_energy -= 20;
			mp_bar.transform.localScale = new Vector3(cur_energy/max_energy,mp_bar.transform.localScale.y,mp_bar.transform.localScale.z);
		}
		

		float[] input = MidiMaster.GetInput ();

		Queue<float[]> punch_buff = MidiMaster.GetPunch();
		
		
		if (CheckPunch(punch_buff)){	
			if(Time.time>punch_update){
				punch_update = Time.time + deltatime;
			}
			continue_punch();	
			MidiMaster.ClearPunch();
			//punch_lock = true;
		}else{
			if(Time.time>punch_update){
				punch.gameObject.SetActive(false);
				//punch_lock = false;
			}
			
		}

		if (Input.GetKeyDown ("space")&&grounded==true) {
			animator.SetTrigger ("NotGround");
			moveV = 58;
		}

		grounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundedRadius, whatIsGround);

		animator.SetBool("Ground", grounded);
		

		if(input[0]>-1){
			nextupdate = input[2] + deltatime;
		}
		
			if (input[0] >= 46 && Time.time < nextupdate) {
			moveH = 4;
			rb2d.velocity = new Vector2(moveH,rb2d.velocity.y);
			} else if (input[0] < 46 && input[0] > -1 && Time.time < nextupdate) {
				moveH = -4;
				rb2d.velocity = new Vector2(moveH, rb2d.velocity.y);
			} else if (Time.time > nextupdate){
				moveH = 0;
				rb2d.velocity = new Vector2(moveH, rb2d.velocity.y);
			}
			
			if (input[0]>-1 && input[1] >= 1.6f && grounded == true){
				input[1] = -1;
				moveV = 580;
				rb2d.AddForce(new Vector2(0,moveV));
				// Debug.Log("Force added");
				animator.SetTrigger("NotGround");
			}

		animator.SetFloat ("Speed", Math.Abs(moveH));

		if(dead){
			
			finish.color = Color.Lerp(finish.color,Color.black,Time.deltaTime*5f);
			if (Time.time > finish_time + 2f){
				SceneManager.LoadScene("MainStage",LoadSceneMode.Single);
			}
		}

		if(this.transform.position.x > 143){
			if(!finished){
				finish.text = "Yeah!!!!";
				finish_time = Time.time;
			}
			finished = true;
		}
		if(finished){
			finish.color = Color.Lerp(finish.color,Color.black,Time.deltaTime*5f);
			if (Time.time > finish_time + 2f){
				SceneManager.LoadScene("MainStage",LoadSceneMode.Single);
			}
		}

		if (moveH > 0 && !faceRight)
			Flip();
		else if (moveH < 0 && faceRight)
			Flip();
	}

	public bool GetFace(){
		return faceRight;
	}

	private void continue_punch(){
		punch.gameObject.SetActive(true);
		//IncreaseEnergy(5);
	}

	private void moon_attack(){
		Rigidbody2D newbullet = Instantiate (bullet);
		newbullet.transform.position = rb2d.transform.position;
		if (faceRight) {
			newbullet.velocity = new Vector2 (15f, 0);
		} else {
			newbullet.velocity = new Vector2 (-15f,0);
			Vector3 theScale = newbullet.transform.localScale;
			theScale.x *= -1;
			newbullet.transform.transform.localScale = theScale;
		}
	}

	public void Move(float move, bool crouch, bool jump){
		// If crouching, check to see if the character can stand up
		// Set whether or not the character is crouching in the animator
		animator.SetBool("Crouch", crouch);
		//only control the player if grounded or airControl is turned on
		if (grounded || airControl)
		{
			// Reduce the speed if crouching by the crouchSpeed multiplier

			// The Speed animator parameter is set to the absolute value of the horizontal input.
			animator.SetFloat("Speed", Mathf.Abs(move));

			// Move the character
			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !faceRight)
				// ... flip the player.
				Flip();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && faceRight)
				// ... flip the player.
				Flip();
		}
		// If the player should jump...
		if (grounded && jump && animator.GetBool("Ground"))
		{
			// Add a vertical force to the player.
			grounded = false;
			animator.SetBool("Ground", false);
			//rigidbody2D.AddForce(new Vector2(0f, jumpForce));
		}
	}
	void OnCollisionEnter2D (Collision2D other){
		if (other.gameObject.CompareTag ("hp_fruit")) {
			Debug.Log("get hp fruit");
			Destroy(other.gameObject);
			this.IncreaseHealth(5);
		}
		if (other.gameObject.CompareTag("mp_fruit")){
			Debug.Log("get mp fruit");
			this.IncreaseEnergy(5);
			Debug.Log(other.gameObject.name);
		}
	}

	private void Flip(){
		// Switch the way the player is labelled as facing.
		faceRight = !faceRight;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private float variance(List<int> noteArray){
		float var_result = 0;
		int avg_result = 0;
		foreach (int note in noteArray){
			avg_result += note;
		}
		avg_result /= noteArray.Count;
		foreach(int note in noteArray){
			var_result = Mathf.Pow(note - avg_result,2);
		}
		var_result /= noteArray.Count;
		
		return var_result;
	}

	private float entropy(Queue<int> noteArray){
		int[] freqArray = new int[128];
		foreach(int note in noteArray){
			freqArray[note] += 1;
		}
		double result = 0;
		int count = noteArray.Count;
		var noteSet = new HashSet<int>(noteArray);

		foreach(int note in noteSet){
			float probability = (float)freqArray[note]/(float)count;
			result += probability * Math.Log(probability,2);
		}
		return -(float)result;
	}
	public void IncreaseEnergy(float energy){
		if(cur_energy+energy>max_energy){
			cur_energy = max_energy;
		}else{
			cur_energy += energy;
		}
		mp_bar.transform.localScale = new Vector3(cur_energy/max_energy,mp_bar.transform.localScale.y,mp_bar.transform.localScale.z);
	}

	public void IncreaseHealth(float health){
		if(cur_health+health>max_health) cur_health = max_health; else cur_health += health;
		hp_bar.transform.localScale = new Vector3(cur_health/max_health,hp_bar.transform.localScale.y,hp_bar.transform.localScale.z);
	}

	public void DecreaseHealth(float health){
		if(cur_health-health<=0){
			Debug.Log("Die");
			cur_health = 0;
			

			if (!dead){
				finish.text = "GameOver";
				animator.SetTrigger("Dead");
				finish_time = Time.time;
			}
			dead = true;
		}else{
			player_health.takeDamage(1);
			cur_health -= health;
		}
		hp_bar.transform.localScale = new Vector3(cur_health/max_health,hp_bar.transform.localScale.y,hp_bar.transform.localScale.z);
	}

	private bool CheckPunch(Queue<float[]> queue){
		float result = 0;
		HashSet<int> set = new HashSet<int>();
		if(queue.Count>2){
			float[] pre = queue.Peek();
			foreach(float[] note in queue){
				result += note[2] - pre[2];
				pre = note;
				set.Add((int)note[0]);
			}
			result /= (queue.Count-1);
		}
		if (set.Count == 2 && result > 0 && result < 0.1f ){
			return true;
		}else{
			return false;
		}
	}

	private bool checkMoon(Queue<int> queue){
		// foreach(var value in queue){
		// 	Debug.Log(value);
		// }
		// Debug.Log(".........................");
		int[] tri_array = queue.ToArray();
		for (int i=0;i<tri_array.Length;i++){
			for(int j=0; j < tri_array.Length;j++){
				if (Math.Abs(tri_array[j] - tri_array[i]) == 6){
					//Debug.Log(Math.Abs(tri_array[j] - tri_array[i]));
					return true;
				}
			}
		}
		return false;
	}
}
