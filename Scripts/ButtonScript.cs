using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] int buttonValue;
    ButtonPressed pressedEvent;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite OnSprite;
    [SerializeField] Sprite OffSprite;

    Animator animator;
    void Start()
    {
        pressedEvent = new ButtonPressed();
        EventManager.AddButtonPressInvoker(this);

        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    void OnMouseDown()
    {
        animator.enabled = true;
        //ChangePic();
        pressedEvent.Invoke(buttonValue);
    }

    void AnimFinish()
    {
        animator.enabled = false;
    }

    void ChangePic()
    {
        if( spriteRenderer != null)
        {
            if( spriteRenderer.sprite == OnSprite )
                spriteRenderer.sprite = OffSprite;
            else
                spriteRenderer.sprite = OnSprite;
        }
    }

    public void UpdatePic( bool status )
    {
        if( status )
            spriteRenderer.sprite = OnSprite;
        else
            spriteRenderer.sprite = OffSprite;
    }

    public void AddButtonPressListener(UnityAction<int> listener)
    {
        pressedEvent.AddListener(listener);
    }
}
