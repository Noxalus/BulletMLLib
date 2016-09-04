using System;

namespace BulletMLLib
{
    /// <summary>
    /// 方向設定
    /// </summary>
    class BulletMLSetDirection : BulletMLTask
    {
        public BulletMLTree node;

        public BulletMLSetDirection(BulletMLTree node)
        {
            this.node = node;
        }

        public override BLRunStatus Run(BulletMLBullet bullet)
        {
            BLType blType = node.Type;
            float value = (float)(node.GetValue(this) * Math.PI / 180);

            if (blType == BLType.Sequence)
            {
                bullet.Direction = bullet.GetFireData().SourceDirection + value;
            }
            else if (blType == BLType.Absolute)
            {
                bullet.Direction = value; 
            }
            else if (blType == BLType.Relative)
            {
                bullet.Direction = bullet.Direction + value;
            }
            else //if (blType == BLType.Aim || blType == BLType.None)
            {
                bullet.Direction = bullet.GetAimDir() + value;
            }

            //Debug.WriteLine(String.Format("SetDirecton:{0},  (type:{1} val:{2})", bullet.Direction / Math.PI * 180, node.type, value / Math.PI * 180));
            End = true;

            return BLRunStatus.End;
        }

    }
}