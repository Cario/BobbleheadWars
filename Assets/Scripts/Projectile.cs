using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
        Debug.Log("Hello");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
