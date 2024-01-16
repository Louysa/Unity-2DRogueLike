using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
  public float moveTime = 0.1f;
  public LayerMask blockingLayer;
  

  private BoxCollider2D boxCollider2D;
  private Rigidbody2D rigidbody2D;
  private SpriteRenderer spriteRenderer;
  private float inverseMoveTime;
  private bool isMoving;
  
  

  protected virtual void Start()
  {
    boxCollider2D = GetComponent<BoxCollider2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    rigidbody2D = GetComponent<Rigidbody2D>();
    inverseMoveTime = 1f / moveTime;

  }

  protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
  {
    Vector2 start = transform.position;
    Vector2 end = start + new Vector2(xDir, yDir);

    boxCollider2D.enabled = false;
    hit = Physics2D.Linecast(start, end, blockingLayer);
    boxCollider2D.enabled = true;

    if (end.x - start.x < 0)
      spriteRenderer.flipX = true;
    else
      spriteRenderer.flipX = false;
    
    if (hit.transform == null)
    {
      StartCoroutine(SmoothMovement(end));
      return true;
    }

    return false;
  }

  protected virtual void AttemptMove<T>(int xDir, int yDir)
    where T : Component
  {
    RaycastHit2D hit;
    bool canMove = Move(xDir, yDir, out hit);
    if (hit.transform == null)
    {
      return;
    }

   
    T hitComponent = hit.transform.GetComponent<T>();
    
    if (!canMove && hitComponent != null & !isMoving )
    {
      OnCantMove(hitComponent);
    }
  }
  
  protected IEnumerator SmoothMovement(Vector3 end)
  {
    isMoving = true;
    float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    while (sqrRemainingDistance > float.Epsilon)
    {
      Vector3 newPosition = Vector3.MoveTowards(rigidbody2D.position, end, inverseMoveTime * Time.deltaTime);

      rigidbody2D.MovePosition(newPosition);
      sqrRemainingDistance = (transform.position - end).sqrMagnitude;
      yield return null;
    }
    rigidbody2D.MovePosition(end);
    isMoving = false;
  }

  protected abstract void OnCantMove<T>(T component)
    where T : Component;

}
