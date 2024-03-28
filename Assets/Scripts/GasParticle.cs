using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasParticle : MonoBehaviour
{
    private Rigidbody2D rb;
    public void SetSpeed(float speed)
    {
        if(rb.velocity.magnitude <= 0.01f)
        {
            rb.velocity = Random.insideUnitCircle.normalized * speed;
        }
        else
        {
            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

}
