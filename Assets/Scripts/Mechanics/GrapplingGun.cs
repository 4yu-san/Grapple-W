using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private Rigidbody rb;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 50f;

    [Header("Pull")]
    public float pullForce = 20f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        rb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Grapple
        if (Input.GetMouseButtonDown(0))
            StartGrapple();
        else if (Input.GetMouseButtonUp(0))
            StopGrapple();

        // Pull towards grapple point
        if (Input.GetMouseButton(1) && joint != null)
            PullTowardsGrapple();

        // Shoot
        if (Input.GetKeyDown(KeyCode.E))
            Shoot();
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;
            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    void PullTowardsGrapple()
    {
        Vector3 direction = (grapplePoint - player.position).normalized;
        rb.AddForce(direction * pullForce, ForceMode.Force);
    }

    void Shoot()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
            bulletRb.linearVelocity = camera.forward * bulletSpeed;

        Destroy(bullet, 3f); // auto destroy after 3 seconds
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        if (!joint) return;
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}