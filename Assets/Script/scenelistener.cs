using UnityEngine;
// using System.Collections;

public class scenelistener : MonoBehaviour {

	// Use this for initialization
	
	// Update is called once per frame
	public GameObject leftbound;
	private GameObject new_leftbound;
	public GameObject rightbound;
	private GameObject new_rightbound;
	GameObject player;
	private GameObject screen_camera;
	private bool total_act;
	public GameObject enemy;
	private bool begin;
	private bool gen_begin;
	private bool gen_finish;
	private int genCount;
	private System.Random random;
	private float nextupdate;
	private int finalCount;
	private PlayerHealth playerhealth;
	void Awake()
	{

		player = GameObject.FindGameObjectWithTag("Player");
		screen_camera = GameObject.FindGameObjectWithTag("MainCamera");
		playerhealth = player.GetComponent<PlayerHealth>();
		random = new System.Random();
		begin = false;
		gen_begin = false;
		gen_finish = false;
		genCount = 0;
		finalCount = 4;
		nextupdate = 0;
		// InvokeRepeating("invokeTest",1f,2f);
	}
	void Update () {
		if(begin&&(finalCount == 0)&&gen_finish){
			leftbound.SetActive(false);
			rightbound.SetActive(false);
			screen_camera.GetComponent<cameraController>().enabled = true;
			begin = false;
		}
		if(genCount < 4 && Time.time > nextupdate && begin){
			genEnemy();
			// Debug.Log("Starts generate enemy");
			nextupdate = Time.time + 1f;
		}
		if(genCount>=4){
			gen_finish = true;
		}
	}

	void genEnemy(){
		GameObject new_enemy = Instantiate(enemy);
		int rnd = random.Next(2);
		if (rnd == 1){
			new_enemy.transform.position = new Vector2(leftbound.transform.position.x+1,player.transform.position.y);
		}else{
			new_enemy.transform.position = new Vector2(rightbound.transform.position.x-1,player.transform.position.y);
		}
		genCount ++;
	}

	public void deCount(){
		if(begin){
			finalCount--;
		}
	}
	void OnTriggerEnter2D(Collider2D other)
	{   if(other.CompareTag("Player")){
			begin =true;
			//Debug.Log(begin);
			leftbound = Instantiate(leftbound);
			rightbound = Instantiate(rightbound);
			screen_camera.GetComponent<cameraController>().enabled = false;
			EdgeCollider2D edgec = GetComponent<EdgeCollider2D>();
			edgec.enabled = false;
			playerhealth.takeDamage(2);
		}
	}
}
