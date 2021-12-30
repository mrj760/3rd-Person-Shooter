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
        moveSpeedX = 1;
        moveSpeedY = 1;
        moveSpeedZ = 1;
        moveAccelX = 1;
        moveAccelY = 1;
        moveAccelZ = 1;
        moveDecelX = 1;
        moveDecelY = 1;
        moveDecelZ = 1;
        aimX = 0;
        aimY = 0;
        aimSpeedX = 1;
        aimSpeedY = 1;
        aimAccelX = 1;
        aimAccelY = 1;
        aimDecelX = 1;
        aimDecelY = 1;
    }
}
