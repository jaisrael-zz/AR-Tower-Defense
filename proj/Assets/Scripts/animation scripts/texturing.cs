using UnityEngine;
using System.Collections;

public class texturing : MonoBehaviour {

	public float xOffset;
	public float yOffset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;
		Vector2 currentOffset = this.renderer.material.GetTextureOffset("_MainTex");
		this.gameObject.renderer.material.SetTextureOffset("_MainTex",currentOffset+(new Vector2(xOffset,yOffset)*dt));
	}
}
