using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event System.Action OnReachedEndOfLevel;
    
    Rigidbody myrigidbody;

    bool disabled ;

    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidBody = null;

    [SerializeField] public float speed = 8;
    public float orginspeed ;
    public float TurnSpeed = 10;
    public float Movetime = .1f;

    Vector3 velocity;

     float smoothspeed;
     float angel;
     float smoothMagnutide;


    // Start is called before the first frame update
    void Start()
    {
        if (!m_animator) { gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }
        orginspeed = speed;
        Guard.onGuardSpottedPlayer += Disabled;
   
        myrigidbody = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 direction = Vector3.zero;
       
        if (!disabled){
              direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }

        smoothMagnutide = Mathf.SmoothDamp( direction.magnitude,smoothMagnutide, ref smoothspeed, Movetime);
        float targetAngel = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        angel =  Mathf.LerpAngle(angel,targetAngel,Time.deltaTime * TurnSpeed * direction.magnitude);
        velocity = direction * speed;
       // transform.eulerAngles =Vector3.up * angel;
       // transform.Translate( direction * speed * Time.deltaTime  , Space.World);

    }
    void FixedUpdate()
    {
        myrigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angel));
        myrigidbody.MovePosition(myrigidbody.position + velocity * Time.deltaTime);

    }

    void OnTriggerEnter(Collider hitcollider)
    {
        if (hitcollider.tag == "Finish")
        {
            Disabled();
            if (OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }

    }


    void Disabled()
    { 
        disabled = true;

    }

     void OnDestroy()
    {
        Guard.onGuardSpottedPlayer -= Disabled;
    }

  
}
