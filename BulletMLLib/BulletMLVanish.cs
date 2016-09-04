namespace BulletMLLib
{
    class BulletMLVanish : BulletMLTask
    {
        public override BLRunStatus Run(BulletMLBullet bullet)
        {
            bullet.Vanish();
            End = true;
            //if(bullet.index == DISP_BULLET_INDEX) Debug.WriteLine("Vanish");
            return BLRunStatus.End;
        }
    }
}