using UnityEngine;
//using System.Collections;

public class bullet_listener : MonoBehaviour {
	
	// Update is called once per frame
	private Animator animator;
	private bool exploded;
	private float explode_time;
	private Rigidbody2D rb2d;
	void Awake()
	{
		animator = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		exploded = false;
		explode_time = -1;
	}
	void Update () {
		Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
		if (screenPosition.x > Screen.width || screenPosition.x < 0)
		{
			Debug.Log("bullet destroyed");
			this.gameObject.SetActive (false);
		}
		if(exploded&&Time.time > (explode_time+1f)){
			this.gameObject.SetActive(false);
		}
		// if(Time.time>(explode_time+0.2f)&&exploded){
		// 	this.transform.localScale = new Vector2(3,3);
		// }

	}
	void OnTriggerEnter2D (Collider2D other){
		if (other.gameObject.CompareTag ("enemy")) {
			animator.SetTrigger("explode");
			rb2d.velocity = new Vector2(0,0);
			explode_time = Time.time;
			other.gameObject.SetActive(false);
			
			this.transform.localScale = new Vector2(3,3);
			exploded = true;
			this.GetComponent<CircleCollider2D>().enabled = false;
			
		}

	}
}
