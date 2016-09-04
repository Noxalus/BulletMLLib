namespace BulletMLLib
{
    /// <summary>
    /// Speed 処理
    /// </summary>
    class BulletMLSetSpeed : BulletMLTask
    {
        BulletMLTree node;
        public BulletMLSetSpeed(BulletMLTree node)
        {
            this.node = node;
        }
        public override BLRunStatus Run(BulletMLBullet bullet)
        {
            bullet.Speed = node.GetValue(this);
            //if(bullet.index == DISP_BULLET_INDEX) Debug.WriteLine("SetSpeed:" + bullet.Speed);
            End = true;
            return BLRunStatus.End;
        }
    }
}