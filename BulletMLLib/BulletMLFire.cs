using System;

namespace BulletMLLib
{
    /// <summary>
    /// BulletML Fire処理
    /// </summary>
    class BulletMLFire : BulletMLTask
    {
        BulletMLTree refNode, dirNode, spdNode, node, bulletNode;

        public BulletMLFire(BulletMLTree node)
        {
            this.node = node;
            this.dirNode = node.GetChild(BLName.Direction);
            this.spdNode = node.GetChild(BLName.Speed);
            this.refNode = node.GetChild(BLName.BulletRef);
            this.bulletNode = node.GetChild(BLName.Bullet);
            if(dirNode == null && refNode != null)
                dirNode = refNode.GetChild(BLName.Direction);
            if(dirNode == null && bulletNode != null)
                dirNode = bulletNode.GetChild(BLName.Direction);
            if(spdNode == null && refNode != null)
                spdNode = refNode.GetChild(BLName.Speed);
            if(spdNode == null && bulletNode != null)
                spdNode = bulletNode.GetChild(BLName.Speed);
               
        }

        public override BLRunStatus Run(BulletMLBullet bullet)
        {
            float changeDir = 0;
            float changeSpd = 0;

            // 方向の設定
            if (dirNode != null)
            {
                changeDir = (int)dirNode.GetValue(this) * (float)Math.PI / (float)180;
                if (dirNode.Type == BLType.Sequence)
                {
                    bullet.GetFireData().SourceDirection += changeDir;
                }
                else if (dirNode.Type == BLType.Absolute)
                {
                    bullet.GetFireData().SourceDirection = changeDir;
                }
                else if (dirNode.Type == BLType.Relative)
                {
                    bullet.GetFireData().SourceDirection = changeDir + bullet.Direction;
                }
                else
                {
                    bullet.GetFireData().SourceDirection = changeDir + bullet.GetAimDir();
                }
            }
            else
            {
                bullet.GetFireData().SourceDirection = bullet.GetAimDir();
            }



            // 弾の生成
#if ExpandedBulletML
            string blName = "";
            if (bulletNode != null)
                blName = bulletNode.bulletName;
            else if (refNode != null)
                blName = refNode.bulletName;
            BulletMLBullet newBullet = bullet.GetNewBullet(blName);//bullet.tree);
#else
            BulletMLBullet newBullet = bullet.GetNewBullet();//bullet.tree);
#endif

            if (newBullet == null)
            {
                End = true;
                return BLRunStatus.End;
            }
           
            if (refNode != null)
            {
                // パラメータを取得
                for (int i = 0; i < refNode.Children.Count; i++)
                {
                    newBullet.Tasks[0].ParamList.Add(refNode.Children[i].GetValue(this));
                }

                //if (refNode.children.Count > 0)
                //{
                //    newBullet.task.paramNode = refNode;// node;
                //}
                // refBulletで参照
                newBullet.Init( bullet.Tree.GetLabelNode(refNode.Label, BLName.Bullet) );
#if ExpandedBulletML
                newBullet.Visible = refNode.visible;
#endif
            }
            else
            {
                newBullet.Init(bulletNode);
#if ExpandedBulletML
               newBullet.Visible = bulletNode.visible; 
#endif
            }

            newBullet.X = bullet.X;
            newBullet.Y = bullet.Y;
            newBullet.Tasks[0].Owner = this;
            newBullet.Direction = bullet.GetFireData().SourceDirection;


            if (!bullet.GetFireData().SpeedInit && newBullet.GetFireData().SpeedInit)
            {
                // 自分の弾発射速度の初期化がまだのとき、子供の弾の速度を使って初期値とする
                bullet.GetFireData().SourceSpeed = newBullet.Speed;
                bullet.GetFireData().SpeedInit = true;
            }
            else
            {
                // 自分の弾発射速度の初期化済みのとき
                // スピードの設定
                if (spdNode != null)
                {
                    changeSpd = spdNode.GetValue(this);
                    if (spdNode.Type == BLType.Sequence || spdNode.Type == BLType.Relative)
                    {
                        bullet.GetFireData().SourceSpeed += changeSpd;
                    }
                    else
                    {
                        bullet.GetFireData().SourceSpeed = changeSpd;
                    }
                }
                else
                {
                    // 特に弾に速度が設定されていないとき
                    if(!newBullet.GetFireData().SpeedInit)
                        bullet.GetFireData().SourceSpeed = 1;
                    else
                        bullet.GetFireData().SourceSpeed = newBullet.Speed;
                }
            }

            newBullet.GetFireData().SpeedInit = false;
            newBullet.Speed = bullet.GetFireData().SourceSpeed;

            //if(bullet.index == DISP_BULLET_INDEX) Debug.WriteLine(String.Format("Fire dir:{0} spd:{1} label:{2}", bullet.srcDir / Math.PI * 180, bullet.srcSpeed, refNode != null ? refNode.label : ""));
            //Debug.WriteLine("index({3}) Fire dir:{0} spd:{1} label:{2}", bullet.srcDir / Math.PI * 180, bullet.srcSpeed, refNode != null ? refNode.label : "", bullet.index);

            End = true;
            return BLRunStatus.End;

        }


    }
}