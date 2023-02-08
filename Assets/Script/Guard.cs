using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.VirtualTexturing;

public class Guard : MonoBehaviour
{
    public static event System.Action onGuardSpottedPlayer;
   

    Transform player;

    public LayerMask mask;

    public Light spotlight;
    Color  orginSpotlight;
    public float wiewDistance;
    float wiewAngle;

    public float playerVisibleTimer ;
   public float toTimeSpotPlayer = .5f;

    public float speed = 15;
    
     

    public float waitTime = .3f;
    public float turningSpeed = 10;

    public Transform pathHolder;

    void Start()
    {
         //float playersSpeed = FindObjectOfType<Player>().speed;
         player = GameObject.FindGameObjectWithTag("Player").transform;
        orginSpotlight = spotlight.color;
       // float Speedorigin = playersSpeed;


        wiewAngle = spotlight.spotAngle;
        Vector3[] waypoints = new Vector3 [pathHolder.childCount];

        for (int i= 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y , waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));

    }

     void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;

            
            // spotlight.color = Color.red;
        }
        else
        {
            
               
            playerVisibleTimer -= Time.deltaTime;
    //spotlight.color = orginSpotlight;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer , 0 , toTimeSpotPlayer);
        
        spotlight.color = Color.Lerp(orginSpotlight, Color.red, playerVisibleTimer / toTimeSpotPlayer);

       if (playerVisibleTimer >= toTimeSpotPlayer)
        {
            if (onGuardSpottedPlayer != null)
            {
                onGuardSpottedPlayer();
            }

        }
       
    }

    

    bool CanSeePlayer(){
        if (Vector3.Distance(transform.position,player.position) < wiewDistance )
        {
            Vector3 dirtoLookPlayer = ( player.position- transform.position).normalized;
            float angleBetweenGuardandPlayer = Vector3.Angle(transform.forward , dirtoLookPlayer); 
            if (angleBetweenGuardandPlayer < wiewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position,player.position, mask))
                {
                    return true;

                }

            }

        }
        return false;
    }

    
    IEnumerator FollowPath(Vector3[] waypoints)
    {

        transform.position = waypoints[0];

        int targetWaypointIndex = 1 ;

        Vector3 targetwaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetwaypoint);
        while (true)
        {

            transform.position = Vector3.MoveTowards(transform.position ,targetwaypoint, speed * Time.deltaTime );
            if (transform.position == targetwaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetwaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetwaypoint));
            }
           
            yield return null;
        }
        
    }
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirtoLookTarget = (lookTarget - transform.position);
        float targetAngel =  Mathf.Atan2(dirtoLookTarget.x, dirtoLookTarget.z) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngel)) > 0.05f)
        {
            float Angel = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngel, turningSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * Angel;
            yield return null;

        }


    }

   

    void OnDrawGizmos()
    {
        Vector3 startPositon = pathHolder.GetChild(0).position;
        Vector3 previousPositon = startPositon;
        

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPositon, waypoint.position);
            previousPositon = waypoint.position;
            
        }

        Gizmos.DrawLine(previousPositon, startPositon );
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, wiewDistance * transform.forward);
    }


}
