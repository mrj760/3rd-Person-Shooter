using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class Player : MonoBehaviour
{
    public GameObject move;
    public Transform camtx;
    public Camera cam;
    public float camDistance = 1.2f;
    public Vector3 camOffset;
    public Vector3 camAngleOffset;
    public Transform fwdtx;
    PlayerStats ps;
    public GameObject projectile;
    public float projectileTime = 5f;
    Vector3 projectileTarget;
    float dt;
    float gravity = -9.8f;
    float gravityFactor = 1.0f;
    float maxFallSpeed = -20.0f;
    float rh;
    float rv;
    [Range(-180, 0)]
    public float lookDownMaxAngle = -70f;
    [Range(0,180)]
    public float lookUpMaxAngle = 30f;
    float x = 0, y = 0, z = 0;
    float radAroundY = 0;
    float radAroundX = 0;

    Vector3 moveDir;

    List<Projectile> projectiles;
    float projCoolDown = 0f;

    void Start()
    {
        ps = new PlayerStats();
        projectiles = new List<Projectile>();
    }

    void Update()
    {
        dt = Time.deltaTime;

        /* BEGIN Moving Camera around player */

        // Input to move camera right & left, and limiting speed
        rh = Input.GetAxis("Horizontal Aim");
        rv = Input.GetAxis("Vertical Aim");

        Vector2 aimInp = new Vector2(rh, rv);

        float aimInpMagSq = aimInp.x * aimInp.x + aimInp.y * aimInp.y;
        if (aimInpMagSq > 1.0f)
        {
            float moveInpMag = Mathf.Sqrt(aimInpMagSq);
            aimInp.x /= moveInpMag;
            aimInp.y /= moveInpMag;
        }

        //Debug.Log($"rh:{rh}");
        if (Mathf.Abs(rh) >= .1f)
        {
            if (Mathf.Abs(ps.aimX) < ps.aimSpeedX * Mathf.Abs(rh))
                ps.aimX += aimInp.x/Mathf.Abs(aimInp.x) * ps.aimAccelX * dt;
            else
                ps.aimX = ps.aimSpeedX * aimInp.x;
        }
        else
        {
            if (Mathf.Abs(ps.aimX) < .05f || ps.aimX / rh < 0.0f)
                ps.aimX = 0;
            else if (ps.aimX > 0)
                ps.aimX -= ps.aimDecelX * dt;
            else
                ps.aimX += ps.aimDecelX * dt;
        }
        
        // Input to move camera up & down, and limiting speed
        if (Mathf.Abs(rv) >= .1f)
        {
            if (Mathf.Abs(ps.aimY) < ps.aimSpeedY * Mathf.Abs(rv))
                ps.aimY += rv/Mathf.Abs(rv) * ps.aimAccelY * dt;
            else
                ps.aimY = ps.aimSpeedY * aimInp.y;
        }
        else
        {
            if (Mathf.Abs(ps.aimY) < .05f || ps.aimY / rv < 0.0f)
                ps.aimY = 0;
            else if (ps.aimY > 0)
                ps.aimY -= ps.aimDecelY * dt;
            else
                ps.aimY += ps.aimDecelY * dt;
        }
        
        // Limiting Camera Angle
        if (radAroundX <= (lookDownMaxAngle * Mathf.Deg2Rad))
        {
            if (rv >= 0.1f )
            {
                radAroundX += ps.aimY * dt;
            }
            else
            {
                radAroundX = lookDownMaxAngle * Mathf.Deg2Rad;
            }
        }
        else if (radAroundX >= (lookUpMaxAngle * Mathf.Deg2Rad))
        {
            if (rv <= .1f)
            {
                radAroundX += ps.aimY * dt;
            }
            else
            {
                radAroundX = lookUpMaxAngle * Mathf.Deg2Rad;
            }
        }
        else
        {
            radAroundX += (ps.aimY * dt);
        }
        
        // Keep radAroundY between 0 and 2pi radians
        radAroundY = (radAroundY + (ps.aimX * dt)) % (Mathf.PI*2);
        //Debug.Log($"rv:{rv}");
        //Debug.Log($"Radians Around Y: {radAroundY}... Radians Around X: {radAroundX}");
        //Debug.Log($"Degrees Around Y: {radAroundY * Mathf.Rad2Deg}... Degrees Around X: {radAroundX * Mathf.Rad2Deg}");
        {
        x = ((camDistance + camOffset.x) * Mathf.Sin(radAroundY));
        y = ((camDistance + camOffset.y) * Mathf.Sin(radAroundX));
        z = ((camDistance + camOffset.z) * Mathf.Cos(radAroundY));
        //Debug.Log($"X:{x}, Y:{y}, Z:{z}");

        // move camera
        camtx.position = transform.position - new Vector3(x, y, z);
        camtx.LookAt(transform.position);
        camtx.Rotate(camAngleOffset, Space.World);
        // track forward vector on opposite side of camera
        fwdtx.position = transform.position + new Vector3(x, y, z).normalized;
        fwdtx.RotateAround(transform.position, Vector3.up, camAngleOffset.y);
        fwdtx.LookAt(transform.position);
        fwdtx.Rotate(0,180,0);
        }

        /* END Moving Camera around player */

        /* Character movement */

        // Input character side to side movement
        bool moving = false;
        float lh = Input.GetAxis("Horizontal");
        float lv = Input.GetAxis("Vertical");
        Vector2 moveInp = new Vector2(lh, lv);

        

        float moveInpMagSq = moveInp.x * moveInp.x + moveInp.y * moveInp.y;
        if (moveInpMagSq > 1.0f)
        {
            float moveInpMag = Mathf.Sqrt(moveInpMagSq);
            moveInp.x /= moveInpMag;
            moveInp.y /= moveInpMag;
        }
        if (Mathf.Abs(lh) >= .1f)
        {
            moving = true;
            if (Mathf.Abs(ps.moveX) < ps.moveSpeedX * moveInp.x)
                ps.moveX += moveInp.x * ps.moveAccelX * dt;
            else
                ps.moveX = ps.moveSpeedX * moveInp.x;
        }
        else
        {
            if (Mathf.Abs(ps.moveX) < .1f)
                ps.moveX = 0;
            else
                if (ps.moveX < 0)
                    ps.moveX += ps.moveDecelX * dt;
                else
                    ps.moveX -= ps.moveDecelX * dt;
        }
        // Input character back/forth movement
        if (Mathf.Abs(lv) >= .1f)
        {
            moving = true;
            if (Mathf.Abs(ps.moveZ) < ps.moveSpeedZ * moveInp.y)
                ps.moveZ += moveInp.y * ps.moveAccelZ * dt;
            else
                ps.moveZ = ps.moveSpeedZ * moveInp.y;
        }
        else
        {
            if (Mathf.Abs(ps.moveZ) < .1f)
                ps.moveZ = 0;
            else
                if (ps.moveZ < 0)
                    ps.moveZ += ps.moveDecelZ * dt;
                else
                    ps.moveZ -= ps.moveDecelZ * dt;
        }

        // Move and face character based on camera 
        if (moving)
        {   
            // attempt #3
            moveDir = fwdtx.right * ps.moveX + fwdtx.forward * ps.moveZ;
            moveDir.y = 0;
            transform.LookAt(move.transform.position + moveDir);
            move.transform.Translate(moveDir * dt);
        }

        // Falling
        if (Mathf.Abs(ps.moveY) < maxFallSpeed)
            ps.moveY -= gravity * gravityFactor * dt;
        ps.moveY = Mathf.Clamp(ps.moveY, 0.0f, maxFallSpeed);

        float L2 = Input.GetAxis("L2");
        float R2 = Input.GetAxis("R2");

        // Debug.Log($"Trigger input: {L2}, {R2}");

        if (projCoolDown >= 1f && R2 > 0.1f)
        {   
            GameObject proj = Instantiate(projectile, fwdtx.position, cam.transform.rotation);
            projectiles.Add(proj.GetComponent<Projectile>());
            projCoolDown = 0f;
        }
        else
        {
            projCoolDown += dt;
        }

        Queue<Projectile> toRemove = new Queue<Projectile>();
        foreach (var p in projectiles)
        {   
            if (p == null)
            {
                toRemove.Enqueue(p);
                continue;
            }
            if (p.GetType() == typeof(BasicShot))
            {

                // Vector3 playerToProjectile = p.transform.position - transform.position;
                // Vector3 projectileToCamFwd = Vector3.Project(p.transform.position - transform.position, cam.transform.forward);
                // float distToCamFwd = Vector3.Distance(p.transform.position, transform.position + projectileToCamFwd);
                // float distFromPlayerToAim = Mathf.Max(1f/distToCamFwd, .01f);
                // Vector3 candidateTarget = (distFromPlayerToAim * cam.transform.forward) + transform.position;
                // // Debug.Log($"Dist to normal: {distToCamFwd}, Target = {projectileTarget}, n: {1f/distToCamFwd}");
                // p.transform.LookAt(candidateTarget);
                // if (Vector3.Distance(candidateTarget, transform.position) < Vector3.Distance(projectileTarget, transform.position))
                // {   
                //     transform.Rotate(0,180,0);
                // }
                projectileTarget = transform.position + cam.transform.forward * 300f;
                p.transform.LookAt(projectileTarget);
            }
            // Debug.Log($"Proj dir: {p.travelDir}, Speed: {p.travelSpeed}");
        }
        foreach (var rem in toRemove.ToArray())
        {
            projectiles.Remove(toRemove.Dequeue());
        }

        //Debug.Log($"Movement ({ps.moveX}, {ps.moveZ})");
        //Debug.Log($"Aim ({ps.aimX}, {ps.aimY})");
        // Debug.Log($"R2 ({R2})");
        // Debug.Log($"L2 ({L2})");


    }

    void LateUpdate()
    {
    }
    
    void OnDrawGizmosSelected()
    {
        // Left/right and up/down axes.
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position - new Vector3(2, 0, 0), transform.position + new Vector3(2, 0, 0));
        Gizmos.DrawLine(transform.position - new Vector3(0, 0, 2), transform.position + new Vector3(0, 0, 2));

        // Display the plane.
        Gizmos.color = Color.green;
        Vector3 angle = Vector3.zero;
        Gizmos.DrawLine(transform.position, fwdtx.forward);
    }

    IEnumerator EndProjectile (Projectile proj)
    {
        yield return new WaitForSeconds(1f);
        projectiles.Remove(proj);
    }
}
