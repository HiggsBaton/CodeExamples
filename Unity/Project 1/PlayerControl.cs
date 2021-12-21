using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] Transform gun;
    [SerializeField] float posMod = 1f;

    [SerializeField] Transform barrel;
    [SerializeField] GameObject bullet;

    bool canShoot = true;
    bool oneBullet = false;
    
    void Awake()
    {
        gun = GameObject.FindGameObjectWithTag( "HandGun" ).transform;
        barrel = GameObject.FindGameObjectWithTag( "Barrel" ).transform;
    }

    void Update()
    {
        RotateHandGun();
        
        if( canShoot )
            ShootGun();
    }

    void RotateHandGun()
    {
        if( oneBullet )
            canShoot = false;
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );

        float hor = mousePos.x - transform.position.x;
        float ver = mousePos.y - transform.position.y;

        Vector2 dist = new Vector2 ( hor, ver );

        dist.Normalize();

        //Debug.Log(dist);

        float angle = Mathf.Atan2(dist.y, dist.x) * 180 / Mathf.PI;

        gun.transform.rotation = Quaternion.Euler( 0, 0, angle );

        Vector2 newPos = new Vector2 ( transform.position.x + dist.x * posMod, transform.position.y + dist.y * posMod );
        //Debug.Log( newPos );

        gun.transform.position = newPos;
    }

    void ShootGun()
    {
        if( Input.GetMouseButtonDown( 0 ) )
        {
            GameObject newBullet;
            newBullet = Instantiate( bullet, barrel );
            newBullet.transform.SetParent( null );
            newBullet.transform.localScale = Vector2.one;    
        }
    }

    void SetOneBullet()
    {
        oneBullet = true;
    }
}
