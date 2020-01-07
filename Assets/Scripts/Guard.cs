using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour {

    public static event System.Action OnGuardHasSpotedPlayer;

    public Transform pathHolder;
    public float speed = 5;
    public float waitTime = 0.3f;
    public float turnSpeed = 90; // 90 degrees per second

    public Light spotlight;
    Color originalSpotlightColor;
    public float viewDistance;
    float viewAngle;

    Transform player;
    public LayerMask viewMask;
    public float timeToSpotPlayer = 0.5f;
    public float playerVisibleTimer;

    void Start () {
        player = GameObject.FindGameObjectWithTag ("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalSpotlightColor = spotlight.color;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild (i).position + new Vector3 (0, transform.position.y, 0);
        }

        StartCoroutine (FollowPath (waypoints));
    }

    private void Update () {
        if (CanSeePlayer ()) {
            playerVisibleTimer += Time.deltaTime;
        } else {
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp (playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp (originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer) {
            if (OnGuardHasSpotedPlayer != null) {
                OnGuardHasSpotedPlayer ();
            }
        }
    }

    bool CanSeePlayer () {
        if (Vector3.Distance (transform.position, player.position) < viewDistance) {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle (transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f) {
                if (!Physics.Linecast (transform.position, player.position, viewMask)) {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath (Vector3[] path) {
        transform.position = path[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = path[targetWaypointIndex];
        transform.LookAt (targetWaypoint);

        while (true) {
            transform.position = Vector3.MoveTowards (transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint) {
                targetWaypointIndex = (targetWaypointIndex + 1) % path.Length;
                targetWaypoint = path[targetWaypointIndex];
                yield return new WaitForSeconds (waitTime);
                yield return StartCoroutine (TurnToFace (targetWaypoint));
            }
            yield return null;
        }
    }

    IEnumerator TurnToFace (Vector3 lookTarget) {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs (Mathf.DeltaAngle (transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    void OnDrawGizmos () {
        Vector3 startPosition = pathHolder.GetChild (0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder) {
            Gizmos.DrawSphere (waypoint.position, .3f);
            Gizmos.DrawLine (previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine (previousPosition, startPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay (transform.position, transform.forward * viewDistance);
    }

}