namespace BulletMLLib
{
    /// <summary>
    /// 加速処理
    /// </summary>
    class BulletMLAccel : BulletMLTask
    {
        BulletMLTree node;
        float term;
        float verticalAccel;
        float horizontalAccel;
        bool first;

        public BulletMLAccel(BulletMLTree node)
        {
            this.node = node;
        }

        public override void Init()
        {
            base.Init();
            first = true;
        }

        public override BLRunStatus Run(BulletMLBullet bullet)
        {
            if( first )
            {
                first = false;
                term = node.GetChildValue(BLName.Term, this);
                if(node.Type == BLType.Sequence)
                {
                    horizontalAccel = node.GetChildValue(BLName.Horizontal, this);
                    verticalAccel = node.GetChildValue(BLName.Vertical, this);
                }
                else if(node.Type == BLType.Relative)
                {
                    horizontalAccel = node.GetChildValue(BLName.Horizontal, this) / term;
                    verticalAccel   = node.GetChildValue(BLName.Vertical, this) / term;
                }
                else
                {
                    // speedX = (float)(bullet.speed * Math.Sin(bullet.Direction));
                    // speedY = (float)(bullet.speed * Math.Cos(bullet.Direction));
                    horizontalAccel = (node.GetChildValue(BLName.Horizontal, this) - bullet.speedX) / term;
                    verticalAccel = (node.GetChildValue(BLName.Vertical, this) - bullet.speedY) / term;
                }
            }

            term--;
            if (term < 0)
            {
                End = true;
                return BLRunStatus.End;
            }

            bullet.speedX += horizontalAccel;
            bullet.speedY += verticalAccel;

            //Debug.WriteLine(String.Format("accel speedX={0} speedY={1} verAcl={2} horAcl={3} term={4}", bullet.speedX, bullet.speedY, verticalAccel, horizontalAccel, term)); 

            /*
            double speedX = bullet.speed * Math.Sin(bullet.Direction);
            double speedY = bullet.speed * Math.Cos(bullet.Direction);
            speedX += horizontalAccel;
            speedY += verticalAccel;
            
            bullet.Direction = (float)Math.Atan2(speedX ,speedY);
            bullet.speed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            //Debug.WriteLine("accel speed={0} dir={1} verAcl={2} horAcl={3} term={4}", bullet.speed, bullet.Direction / Math.PI * 180, verticalAccel, horizontalAccel, term); */
            return BLRunStatus.Continue;
        }


    }
}