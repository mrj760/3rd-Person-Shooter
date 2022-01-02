using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

class Projectile
{
    public GameObject projectile;
    public Vector3 travelDir;

    public Projectile(GameObject projectile, Vector3 travelDir)
    {
        this.projectile = projectile;
        this.travelDir = travelDir;
    }
}

public class Player : MonoBehaviour
{
    public GameObject move;
    public Transform camtx;
    public Camera cam;
    public Transform fwdtx;
    PlayerStats ps;

    public GameObject projectile;
    public float projectileTime = 5f;
    float dt;
    float gravity = -9.8f;
    float gravityFactor = 1.0f;
    float maxFallSpeed = -20.0f;
    float rh;
    float rv;
    public float camDistance = 1.2f;
    [Range(-180, 0)]
    float lookDownMaxAngle = -70f;
    [Range(0,180)]
    float lookUpMaxAngle = 30f;
    float x = 0, y = 0, z = 0;
    float radAroundY = 0;
    float radAroundX = 0;

    List<Projectile> projectiles;

    void Start()
    {
        ps = new PlayerStats();
        projectiles = new List<Projectile>();
    }

    void Update()
    {
        dt = Time.deltaTime;

        /* Moving Camera around player */
        // Input to move camera right & left, and limiting speed
        rh = 10 * Input.GetAxis("Horizontal Aim");
        rv = 10 * Input.GetAxis("Vertical Aim");

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
        x = (camDistance * Mathf.Sin(radAroundY));
        y = (camDistance * Mathf.Sin(radAroundX));
        z = (camDistance * Mathf.Cos(radAroundY));
        //Debug.Log($"X:{x}, Y:{y}, Z:{z}");

        // move camera
        camtx.position = transform.position - new Vector3(x, y, z);
        camtx.LookAt(transform.position);
        // track forward vector on opposite side of camera
        fwdtx.position = transform.position + new Vector3(x, transform.position.y, z);
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
            Debug.Log("Normalizing Input");
        }
        if (moveInp != Vector2.zero)
        {
            Debug.Log($"Move Input: {moveInp}");
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
            // Attempt #1
            // Vector3 v1 = transform.forward * new Vector3(ps.moveX, 0, ps.moveZ).magnitude; // currently pointing toward
            // Vector3 v2 = fwdtx.position - transform.position;  // desired cam-relative forward direction
            // float c = (Vector3.Dot(v1, v2));
            // float a = v1.magnitude;
            // float theta = 0;
            // if (c < 0)
            // {
            //     theta = Mathf.Acos(Mathf.Abs(c) * Mathf.Rad2Deg / v1.magnitude);
            //     theta = 360 - theta;
            // }
            // else if (c > 0)
            // {
            //     theta = Mathf.Acos(c) * Mathf.Rad2Deg / v1.magnitude;
            // }


            
            // // transform.RotateAround(transform.position, Vector3.up, theta);
            // Vector3 fwd = fwdtx.position - transform.position;
            // Vector3 right = Vector3.Cross(fwd, transform.up);
            // Vector3 moveDir = (fwd * ps.moveZ - right * ps.moveX);
            // transform.localEulerAngles = new Vector3(0, theta, 0);
            // move.transform.Translate(moveDir * dt, Space.World);


            // attempt #2
            // Vector3 moveDirFwd = fwdtx.position - move.transform.position;
            // Vector3 moveDirRight = Vector3.Cross(transform.up, moveDirFwd);
            // moveDirFwd.x *= -ps.moveX;
            // moveDirFwd.y = 0;
            // moveDirFwd.z *= ps.moveZ;

            // moveDirRight.x *= -ps.moveX;
            // moveDirRight.y = 0;
            // moveDirFwd.z *= ps.moveZ;

            // Vector3 moveTotal = (moveDirFwd + moveDirRight).normalized;

            // move.transform.Translate(moveTotal * dt);
            // transform.LookAt(moveDirFwd);


            // attempt #3
            var moveDir = fwdtx.right * ps.moveX + fwdtx.forward * ps.moveZ;
            moveDir.y = 0;
            float angle = Vector3.Angle(moveDir, transform.forward);
            // transform.Rotate(0,angle,0);
            transform.LookAt(move.transform.position + moveDir);
            move.transform.Translate(moveDir * dt);


            // attempt #4
            // var moveDir = transform.InverseTransformDirection(fwdtx.position - move.transform.position);
            // moveDir.z *= ps.moveZ;
            // moveDir.x *= ps.moveX;
            // move.transform.Translate(moveDir * dt);
            // transform.LookAt(transform.position + moveDir);

            if (moveDir != Vector3.zero)
            {
                Debug.Log($"PS.Move: {ps.moveX}, {ps.moveY}, {ps.moveZ}");
                Debug.Log($"MoveDir: {moveDir}");
                Debug.Log($"Move Magnitude: {moveDir.magnitude}");
            }
        }

        // Falling
        if (Mathf.Abs(ps.moveY) < maxFallSpeed)
            ps.moveY -= gravity * gravityFactor * dt;
        ps.moveY = Mathf.Clamp(ps.moveY, 0.0f, maxFallSpeed);

        float L2 = Input.GetAxis("L2");

        // Send projectiles
        float R2 = Input.GetAxis("R2");
        if (R2 > 0.1f)
        {
            Projectile proj = (new Projectile(projectile, fwdtx.position - transform.position));
            projectiles.Add(proj);
            int idx = projectiles.IndexOf(proj);
            Instantiate(projectiles[idx].projectile, fwdtx.position, fwdtx.rotation);
            StartCoroutine(EndProjectile(proj));
        }
        foreach (var p in projectiles)
        {
            p.projectile.transform.position += p.travelDir * dt * 10f;
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
        yield return new WaitForSeconds(5f);
        projectiles.Remove(proj);
        Destroy(proj.projectile);
    }
}
