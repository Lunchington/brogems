using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gem : MonoBehaviour {
	public int value;
	public int health;
	public int id;

	public int x;
	public int y;

	public static bool drag = false;
	public static Vector3  dragtransform;

	private Vector3 scaleDefault;

	private SpriteRenderer spriteRenderer;

	public static GameObject select;
	public static GameObject moveTo;

	public GameObject gemCollide;

	private float distX;
	private float distY;
	private Vector3 screenPoint;

	private Vector3 myScale;
	private Vector3 newScale;
	private float startTime;

	private float collisionRadius;
	private float newCollisionRadius;

	public bool readyToMove;
	public bool fallEffect;

	// Use this for initialization

	void Start () {
		myScale = transform.localScale;
		collisionRadius = this.gameObject.GetComponent<CircleCollider2D> ().radius;
		newCollisionRadius = collisionRadius + 0.10f;

		newScale = Vector3.zero;
		startTime = Time.time;
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		if(fallEffect){
			StartCoroutine("fallDown");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!fallEffect && Time.time-startTime<3){//After block spawn, the grow effect will last no more than 3 seconds
			if(!fallEffect)transform.localScale = Vector3.Lerp(newScale,myScale, (Time.time-startTime)/2);
			if(fallEffect)transform.localScale = Vector3.Lerp(newScale,myScale, (Time.time-startTime)*5); newScale = transform.localScale;
		}


	}

	void OnMouseOver(){
		if (!drag) {
			//Mouse over block effect

			spriteRenderer.transform.localScale = new Vector3 (myScale.x+0.2f,myScale.y+0.2f, 1);
		}

	}

	void OnMouseDown() {

		if (!select) {//If nothing selected, select this block
			this.gameObject.GetComponent<CircleCollider2D>().radius = newCollisionRadius;

			screenPoint =    Camera.main.WorldToScreenPoint(transform.position);
			distX = Input.mousePosition.x - screenPoint.x;
			distY = Input.mousePosition.y - screenPoint.y;

			select = this.gameObject;
			spriteRenderer.transform.localScale = new Vector3 (1.1f, 1.1f, 1);

			drag = true;
			dragtransform=select.transform.position;
		} 
	}

	void OnMouseUp() {
		this.gameObject.GetComponent<CircleCollider2D>().radius = collisionRadius;

		spriteRenderer.transform.localScale = scaleDefault;
	
		drag = false;
		gemCollide = null;
	}

	void OnTriggerStay2D (Collider2D coll) {
		if (select) {
						if ((coll.gameObject != select)) { 
								if (select.collider2D.OverlapPoint (coll.bounds.center)) {
										gemCollide = coll.gameObject;
						} else { 
							gemCollide = null; }
						}
				}
	}

	void OnTriggerExit2D (Collider2D coll) {
		if (coll.gameObject != select) {
			gemCollide = null;
		}	
	}

	void OnMouseDrag() {
		if (drag) {

			Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x - distX, Input.mousePosition.y - distY, 5f);
			Vector3 currentPos = Camera.main.ScreenToWorldPoint(currentScreenPoint);

			this.transform.position = currentPos;
				
		}
	}

	void OnMouseExit(){
		//Reset block scale
		spriteRenderer.transform.localScale = myScale;
	}

	public IEnumerator destroyBlock(){
		Vector3 lastPos = spriteRenderer.transform.localScale;
		float time = 0;
		
		while(time<1){
			time += Time.deltaTime;
			spriteRenderer.transform.localScale = Vector3.Lerp (lastPos, Vector3.zero, time);
			yield return null;
		}
		
		Destroy (gameObject);
		
		
	}

	public IEnumerator moveDown(int rows){
		Vector3 lastPos = transform.position;
		Vector3 newPos = new Vector3 (transform.position.x, transform.position.y - rows, transform.position.z);
		float time = 0;
		
		while(time<1){
			time += Time.deltaTime;
			transform.position = Vector3.Lerp (lastPos, newPos, time);
			yield return null;
		}
		
		
	}

	public IEnumerator fallDown(){
				Board match;
				Gem[] allb = FindObjectsOfType (typeof(Gem)) as Gem[];
				Vector3 newPos = transform.position;
				Vector3 lastPos = new Vector3 (transform.position.x, transform.position.y + 15, transform.position.z);
				float time = 0;
		
				while (time<1) {
						time += Time.deltaTime;
						transform.position = Vector3.Lerp (lastPos, newPos, time);
						yield return null;
				}
		}

}
