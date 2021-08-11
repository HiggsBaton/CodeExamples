using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletControl : MonoBehaviour
{
    [SerializeField] int maxCollisions = 10;
    [SerializeField] float moveSpeed = 1;
    Rigidbody2D rigidbody;
    public Vector2 direction = Vector2.up;

    FreePrisoner freePrisoner;
    
    void Start()
    {
        freePrisoner = new FreePrisoner();
        EventManager.AddFreePrisonerInvoker(this);

        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.AddRelativeForce( direction * moveSpeed, ForceMode2D.Impulse );
    }

    void OnCollisionEnter2D( Collision2D other )
    {
        if( other.gameObject.tag == "Lock" )
        {
            AudioManager.Play( AudioClipName.LockCollision );
            //Destroy( other.gameObject, 0f );
            freePrisoner.Invoke();
            Destroy( gameObject, 0f );
        }
        else if( other.gameObject.tag == "Breakable" )
        {
            AudioManager.Play( AudioClipName.BreakableCollision );
            Destroy( other.gameObject, 0f );
            Destroy( gameObject, 0f );
        }
        else if( other.gameObject.tag == "Hat" )
        {
            AudioManager.Play( AudioClipName.HatCollision );
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        }
        else
        {
            AudioManager.Play( AudioClipName.BulletCollision );
            maxCollisions--;
        }

        if( maxCollisions < 1 )
            Destroy( gameObject, 0f );

        

        //Debug.Log( "Collision" );
    }

    void OnBecameInvisible()
    {
        Destroy( gameObject, 0f );
    }

    public void AddFreePrisonerListener(UnityAction listener)
    {
        freePrisoner.AddListener(listener);
    }
}
