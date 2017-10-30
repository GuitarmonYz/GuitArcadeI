using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	// Use this for initialization
	private Color flash_color;
	private Color shine_color;
	public Image damage_image;
	private int sw;

	void Start () {
		flash_color = new Color(1,0,0,0.1f);
		shine_color = new Color(0,0,0,0.2f);
		sw = 1;
	}
	
	// Update is called once per frame
	void Update () {
		switch(sw){
			case 1:
			damage_image.color = Color.Lerp(damage_image.color,Color.clear,Time.deltaTime*5f);
			break;
			case 2:
			damage_image.color = Color.Lerp(damage_image.color,Color.clear,Time.deltaTime*10f);
			break;

		}
		
	}

	public void takeDamage(int fors){
		switch (fors){
			case 1:
				damage_image.color = flash_color;
				sw = 1;
				break;
			case 2:
				damage_image.color = shine_color;
				sw = 2;
				break;
		}
		
	}
	
	
}
