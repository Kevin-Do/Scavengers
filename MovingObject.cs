using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;


	private BoxCollider2D boxCollider;

	private Rigidbody2D rb2D;

	private float inverseMoveTime;
	
	// Use this for initialization
	protected virtual void Start ()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
		
	}

	protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
	{
		Vector2 start = transform.position; //discards z data
		Vector2 end = start + new Vector2(xDir, yDir);

		boxCollider.enabled = false; //dont wanna hit our own collider
		hit = Physics2D.Linecast(start, end, blockingLayer); //check collision on the blocking layer
		boxCollider.enabled = true;

		if (hit.transform == null) //line is open!
		{
			StartCoroutine(SmoothMovement(end));
			return true;
		}
		return false; //unsecessful
	}

	protected virtual void AttemptMove<T>(int xDir, int yDir)
		where T : Component //specify T is component

	{
		RaycastHit2D hit;
		bool canMove = Move(xDir, yDir, out hit);
		if (hit.transform == null)
			return;
		//free to go lets move!

		T hitComponent = hit.transform.GetComponent<T>();
		//Get the component you can into!

		if (!canMove && hitComponent != null)
		{
			//You ran into something (generic component)
			OnCantMove(hitComponent);
		}
	}
	
	protected IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		//sqrMagnitude is cheaper than magintude
		//Get how much more distance we need to go basically do we need to keep going?
		while (sqrRemainingDistance > float.Epsilon) //epsilon is almost 0
		{
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			//MoveTowards returns point in between rb2D position and end
			//inverseMoveTime * Time.deltaTime is like a current progress
			//Then move the rb
			
			
			//Recalculate remaining distance
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
			//wait for a frame before revaluating
		}
	}

	protected abstract void OnCantMove<T>(T component)
		where T : Component;
	//implement getting stuck for derived classes!
} 
