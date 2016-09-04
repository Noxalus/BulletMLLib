namespace BulletMLLib
{
#if ExpandedBulletML
    enum BLName
    {
        Bullet, Action, Fire, ChangeDirection, ChangeSpeed, Accel,
        Wait, Repeat, BulletRef, ActionRef, FireRef, Vanish,
        Horizontal, Vertical, Term, Times, Direction, Speed, Param,
        Bulletml, None
    }
#else
    public enum BLName
    {
        Bullet, Action, Fire, ChangeDirection, ChangeSpeed, Accel,
        Wait, Repeat, BulletRef, ActionRef, FireRef, Vanish,
        Horizontal, Vertical, Term, Times, Direction, Speed, Param,
        Bulletml, None
    }
#endif
}