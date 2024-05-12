using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSupplyPackage : MonoBehaviour
{
    protected Animator Animator { get; set; }
    protected Rigidbody2D Rigidbody { get; set; }

    protected virtual void Start()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Environment"))
            OnTouchDown();
    }

    protected virtual void OnTouchDown()
    {
        Animator.SetTrigger("TouchDown");
        Rigidbody.mass = 8;
        Rigidbody.drag = 0;
    }
}
