using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerStats
{
    public float hp;
    public float stam;
    public float moveX, moveY, moveZ;
    public float moveSpeedX, moveSpeedY, moveSpeedZ;
    public float moveAccelX, moveAccelY, moveAccelZ;
    public float moveDecelX, moveDecelY, moveDecelZ;
    public float aimX, aimY;
    public float aimSpeedX, aimSpeedY;
    public float aimAccelX, aimAccelY;
    public float aimDecelX, aimDecelY;

    public PlayerStats()
    {
        hp = 500;
        stam = 50;
        moveX = 0;
        moveY = 0;
        moveZ = 0;
        moveSpeedX = 3;
        moveSpeedY = 1;
        moveSpeedZ = 3;
        moveAccelX = 20;
        moveAccelY = 1;
        moveAccelZ = 20;
        moveDecelX = 10;
        moveDecelY = 1;
        moveDecelZ = 10;
        aimX = 0;
        aimY = 0;
        aimSpeedX = 4;
        aimSpeedY = 2;
        aimAccelX = 3;
        aimAccelY = 3;
        aimDecelX = 3;
        aimDecelY = 3;
    }

    public void SetMoveSpeed(float val)
    {
        moveSpeedX = val;
        moveSpeedY = val;
        moveSpeedZ = val;
    }

    public void SetAimSpeed(float val)
    {
        aimSpeedX = val;
        aimSpeedY = val;
    }

    public void SetMoveAccelXZ(float val)
    {
        moveAccelX = val;
        moveAccelZ = val;
    }
}
