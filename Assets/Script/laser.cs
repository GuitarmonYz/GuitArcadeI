using UnityEngine;
using MidiJack;
using System.Collections;

public class laser : MonoBehaviour {

	// Use this for initialization
	private LineRenderer line;
	playerController player;
	void Start () {
		line = gameObject.GetComponent<LineRenderer>();
		line.sortingLayerName = "Player";
		line.enabled = false;
		player = (playerController)this.transform.parent.GetComponent(typeof(playerController));
	}
	
	// Update is called once per frame
	void Update () { 
		
		if(MidiMaster.GetMute()!=64){
			StopCoroutine("FireLaser");
			StartCoroutine("FireLaser");
		}
		
	}

	IEnumerator FireLaser(){
		line.enabled = true;
		Vector2 face;
		//Debug.Log("fired");
		while(MidiMaster.GetMute()!=64){
			if(player.GetFace()){
				face = transform.right;
			}else{
				face = -transform.right;
			}
			Ray2D ray = new Ray2D(transform.position, face);
			RaycastHit2D hit = Physics2D.Raycast(transform.position, face,10);
			line.SetPosition(0,ray.origin);
			if(hit.collider==null){
				line.SetPosition(1,ray.GetPoint(10));
			}else {
				animyController animy = (animyController)hit.rigidbody.GetComponentInParent(typeof(animyController));
				animy.reduceHealth(0.2f);
				line.SetPosition(1,hit.point);
				player.IncreaseEnergy(0.25f);
			}
			
			
			yield return null;
		}
		line.enabled = false;
	}
	
}
