using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Security;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody head;

    public LayerMask layerMask;
    private Vector3 currentLookTarget = Vector3.zero;

    public float moveSpeed = 50.0f;
    private CharacterController characterController;

    public Animator bodyAnimator;

    public float[] hitForce;

    public float timeBetweenHits = 2.5f; //Grace periods between hits
    private bool isHit = false;          //Indicates that the hero took a hit
    private float timeSinceHit = 0;      //the amount of time passed in the grace period
    private int hitNumber = -1;        //The number of times the hero took a hit

    public Rigidbody marineBody;
    //private bool isDead = false;   //Is literally never called. Unnecessary

    private DeathParticles deathParticles;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        deathParticles = gameObject.GetComponentInChildren<DeathParticles>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        characterController.SimpleMove(moveDirection * moveSpeed);

        if (isHit)
        {
            timeSinceHit += Time.deltaTime;
            if (timeSinceHit > timeBetweenHits)
            {
                isHit = false;
                timeSinceHit = 0;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (moveDirection == Vector3.zero)
        {
            bodyAnimator.SetBool("IsMoving", false);
        }
        else
        {
            head.AddForce(transform.right * 150, ForceMode.Acceleration);

            bodyAnimator.SetBool("IsMoving", true);
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.point != currentLookTarget)
            {
                currentLookTarget = hit.point;
            }

            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10.0f);
        }

        //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);
    }

    void OnTriggerEnter(Collider other)
    {
        Alien alien = other.gameObject.GetComponent<Alien>();
        if (alien != null) //Checks if the colliding object has an Alien script attacked to it
        {
            if (!isHit)  //Checks if the alien and player hasn't been hit before, then officially considers the player hit
            {
                hitNumber += 1;
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
                if (hitNumber < hitForce.Length)
                {
                    cameraShake.intensity = hitForce[hitNumber];
                    cameraShake.Shake();
                }
                else
                {
                    Die();
                }
                isHit = true;
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.hurt);
            }
            alien.Die();
        }
    }

    public void Die()
    {
        bodyAnimator.SetBool("IsMoving", false);  //Stops all body animations
        marineBody.transform.parent = null;  //Set parent to null to remove current gameObject from parent
        marineBody.isKinematic = false;   //Disabing IsKinematic and enabling Gravity to cause the body
        marineBody.useGravity = true;     //to drop and roll
        marineBody.gameObject.GetComponent<CapsuleCollider>().enabled = true; //Collider enabled to allow drop and roll to work
        
        marineBody.gameObject.GetComponent<Gun>().enabled = false; //Disabled to prevent gun from being fireable
        Destroy(head.gameObject.GetComponent<HingeJoint>());
        head.transform.parent = null;
        head.useGravity = true;
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.marineDeath);
        deathParticles.Activate();
        Destroy(gameObject);
    }
}
