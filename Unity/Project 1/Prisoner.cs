using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Prisoner : MonoBehaviour
{
    [SerializeField] float lockMoveSpeed = 1;
    [SerializeField] float cageDisappearSpeed = 0.01f;

    GameObject lockObj;
    GameObject cageObj;
    SpriteRenderer[] cageRender;

    Animator animator;

    bool isFree = false;

    void Start()
    {
        EventManager.AddFreePrisonerListener(LockWasBroken);

        animator = GetComponent<Animator>();
        animator.enabled = false;
        lockObj = GameObject.FindGameObjectWithTag( "Lock" );
        cageObj = GameObject.FindGameObjectWithTag( "Cage" );
        cageRender = cageObj.GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if( isFree )
        {
            lockObj.transform.Translate( Vector2.down * Time.deltaTime * lockMoveSpeed );
            Color newColor = cageRender[0].color;
            
            foreach( SpriteRenderer renderer in cageRender )
                renderer.color = new Color(newColor.r, newColor.g, newColor.b, newColor.a - cageDisappearSpeed);
        }
    }

    void LockWasBroken()
    {
        animator.enabled = true;
        isFree = true;
        AudioManager.Play( AudioClipName.PrisonerFreed );
    }
}
