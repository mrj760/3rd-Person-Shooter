using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class Player : MonoBehaviour
{

    public Transform camtx;
    public Camera cam;
    public Transform fwdtx;
    PlayerStats ps;

    float dt;
    float gravity = -9.8f;
    float gravityFactor = 1.0f;
    float maxFallSpeed = -20.0f;
    float rh;
    float rv;
    public float camDistance = 1.2f;
    float lookDownAngle = -70f;
    float lookUpAngle = 30f;
    float x = 0, y = 0, z = 0;
    float radAroundY = 0;
    float radAroundX = 0;

    void Start()
    {
        ps = new PlayerStats();
    }

    void Update()
    {
        dt = Time.deltaTime;
        if (Mathf.Abs(ps.moveY) < maxFallSpeed)
            ps.moveY -= gravity * gravityFactor * dt;
        ps.moveY = Mathf.Clamp(ps.moveY, 0.0f, maxFallSpeed);

        // Moving camera sideways input
        rh = 10 * Input.GetAxis("Horizontal Aim");
        // Debug.Log($"rh:{rh}");
        if (Mathf.Abs(rh) > .1f)
        {
            if (Mathf.Abs(ps.aimX) < ps.aimSpeedX * Mathf.Abs(rh))
                ps.aimX += rh/Mathf.Abs(rh) * ps.aimAccelX;
            else
                ps.aimX = ps.aimSpeedX * rh;
        }
        else
        {
            if (Mathf.Abs(ps.aimX) < .05f || ps.aimX / rh < 0.0f)
                ps.aimX = 0;
            else if (ps.aimX > 0)
                ps.aimX -= ps.aimDecelX;
            else
                ps.aimX += ps.aimDecelX;
        }
        // Moving camera up/down input
        rv = 10 * Input.GetAxis("Vertical Aim");
        if (Mathf.Abs(rv) > .1f)
        {
            if (Mathf.Abs(ps.aimY) < ps.aimSpeedY * Mathf.Abs(rv))
                ps.aimY += rv/Mathf.Abs(rv) * ps.aimAccelY;
            else
                ps.aimY = ps.aimSpeedY * rv;
        }
        else
        {
            if (Mathf.Abs(ps.aimY) < .05f || ps.aimY / rv < 0.0f)
                ps.aimY = 0;
            else if (ps.aimY > 0)
                ps.aimY -= ps.aimDecelY;
            else
                ps.aimY += ps.aimDecelY;
        }
        // Moving camera
        // aimX is num rads to rotate around y axis
        // aimY is num rads to rotate around x axis
        if (radAroundX <= (lookDownAngle * Mathf.Deg2Rad))
        {
            if (rv > 0)
            {
                radAroundX += ps.aimY * dt;
            }
        }
        else if (radAroundX >= (lookUpAngle * Mathf.Deg2Rad))
        {
            if (rv < 0)
            {
                radAroundX += ps.aimY * dt;
            }
        }
        else
        {
            radAroundX += (ps.aimY) * dt;
        }

        radAroundY = (radAroundY + (ps.aimX * dt)) % (Mathf.PI*2);
        Debug.Log($"rv:{rv}");
        Debug.Log($"Radians Around Y: {radAroundY}... Radians Around X: {radAroundX}");
        Debug.Log($"Degrees Around Y: {radAroundY * Mathf.Rad2Deg}... Degrees Around X: {radAroundX * Mathf.Rad2Deg}");

        x = (camDistance * Mathf.Sin(radAroundY));
        y = (camDistance * Mathf.Sin(radAroundX));
        z = (camDistance * Mathf.Cos(radAroundY));

        // Debug.Log($"X:{x}, Y:{y}, Z:{z}");

        camtx.position = transform.position - new Vector3(x, y, z);
        camtx.LookAt(transform.position);

        fwdtx.position = transform.position + new Vector3(x, transform.position.y, z);

        // Moving character side to side
        bool moving = false;
        float lh = Input.GetAxis("Horizontal");
        if (Mathf.Abs(lh) > .1f)
        {
            moving = true;
            ps.moveX += lh * ps.moveAccelX;
            if (Mathf.Abs(ps.moveX) > ps.moveSpeedX * lh)
            {
                ps.moveX = ps.moveSpeedX * lh;
            }
        }
        else
        {
            if (Mathf.Abs(ps.moveX) < .1f)
                ps.moveX = 0;
            else
                if (ps.moveX < 0)
                    ps.moveX += ps.moveDecelX;
                else
                    ps.moveX -= ps.moveDecelX;
        }
        //Moving character back/forth
        float lv = Input.GetAxis("Vertical");
        if (Mathf.Abs(lv) > .1f)
        {
            moving = true;
            ps.moveZ += lv * ps.moveAccelZ;
            if (Mathf.Abs(ps.moveZ) > ps.moveSpeedZ * lv)
            {
                ps.moveZ = ps.moveSpeedZ * lv;
            }
        }
        else
        {
            if (Mathf.Abs(ps.moveZ) < .1f)
                ps.moveZ = 0;
            else
                if (ps.moveZ < 0)
                    ps.moveZ += ps.moveDecelZ;
                else
                    ps.moveZ -= ps.moveDecelZ;
        }

        // Facing player toward movement
        if (moving)
        {   
            transform.LookAt(fwdtx.position);
            transform.Translate(new Vector3(ps.moveX * dt, 0, ps.moveZ * dt));
        }

        // float L2 = Input.GetAxis("L2");
        // float R2 = Input.GetAxis("R2");

        // Debug.Log($"Movement ({ps.moveX}, {ps.moveZ})");
        // Debug.Log($"Aim ({ps.aimX}, {ps.aimY})");
        // Debug.Log($"R2 ({R2})");
        // Debug.Log($"L2 ({L2})");
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
}
