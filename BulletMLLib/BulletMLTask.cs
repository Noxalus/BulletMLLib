using System.Collections.Generic;

namespace BulletMLLib
{
    /// <summary>
    /// BulletMLタスク
    /// 実際に弾を動かすクラス
    /// </summary>
    public class BulletMLTask
    {
        public const int DISP_BULLET_INDEX = 1;

        public enum BLRunStatus { Continue, End, Stop };

        public List<BulletMLTask> TaskList;
        protected bool End = false;
        //public BulletMLTree paramNode = null;
        public readonly List<float> ParamList = new List<float>();
        public BulletMLTask Owner = null;      

        public BulletMLTask()
        {
            TaskList = new List<BulletMLTask>();
        }

        public virtual void Init()
        {
            End = false;

            foreach (BulletMLTask task in TaskList)
            {
                task.Init();
            }
        }

        public virtual BLRunStatus Run(BulletMLBullet bullet)
        {
            End = true;

            foreach (var task in TaskList)
            {
                if (!task.End)
                {
                    BLRunStatus sts = task.Run(bullet);
                    if (sts == BLRunStatus.Stop)
                    {
                        End = false;
                        return BLRunStatus.Stop;
                    }
                    else if(sts == BLRunStatus.Continue)
                        End = false;
                }
            }

            if (End)
                return BLRunStatus.End;

            return BLRunStatus.Continue; // 継続して実行
        }

        // BulletMLTreeの内容を元に、実行のための各種クラスを生成し、自身を初期化する
        public void Parse(BulletMLTree tree, BulletMLBullet bullet)
        {
            foreach (BulletMLTree node in tree.Children)
            {
                // Action
                if (node.Name == BLName.Repeat)
                {
                    Parse(node, bullet);
                }
                else if (node.Name == BLName.Action)
                {
                    ////Debug.WriteLine("Action");
                    int repeatNum = 1;
                    if (node.Parent.Name == BLName.Repeat)
                        repeatNum = (int)node.Parent.GetChildValue(BLName.Times, this);
                    BulletMLAction task = new BulletMLAction(node , repeatNum);
                    task.Owner = this;
                    TaskList.Add(task);
                    task.Parse(node, bullet);
                }
                else if (node.Name == BLName.ActionRef)
                {
                    BulletMLTree refNode = tree.GetLabelNode(node.Label, BLName.Action);
                    int repeatNum = 1;
                    if (node.Parent.Name == BLName.Repeat)
                        repeatNum = (int)node.Parent.GetChildValue(BLName.Times, this);
                    BulletMLAction task = new BulletMLAction(refNode , repeatNum);
                    task.Owner = this;
                    TaskList.Add(task);

                    // パラメータを取得
                    for (int i = 0; i < node.Children.Count; i++)
                    {
                        task.ParamList.Add(node.Children[i].GetValue(this));
                    }
                    //if (node.children.Count > 0)
                    //{
                    //    task.paramNode = node;
                    //}

                    task.Parse(refNode, bullet);
                }
                else if (node.Name == BLName.ChangeSpeed)
                {
                    BulletMLChangeSpeed blChangeSpeed = new BulletMLChangeSpeed(node);
                    blChangeSpeed.Owner = this;
                    TaskList.Add(blChangeSpeed);
                    ////Debug.WriteLine("ChangeSpeed");
                }
                else if (node.Name == BLName.ChangeDirection)
                {
                    BulletMLChangeDirection blChangeDir = new BulletMLChangeDirection(node);
                    blChangeDir.Owner = this;
                    TaskList.Add(blChangeDir);
                    ////Debug.WriteLine("ChangeDirection");
                }
                else if (node.Name == BLName.Fire)
                {
                    if (TaskList == null) TaskList = new List<BulletMLTask>();
                    BulletMLFire fire = new BulletMLFire(node);
                    fire.Owner = this;
                    TaskList.Add(fire);

                }
                else if (node.Name == BLName.FireRef)
                {
                    if (TaskList == null) TaskList = new List<BulletMLTask>();
                    BulletMLTree refNode = tree.GetLabelNode(node.Label, BLName.Fire);
                    BulletMLFire fire = new BulletMLFire(refNode);
                    fire.Owner = this;
                    TaskList.Add(fire);
                    // パラメータを取得
                    //if (node.children.Count > 0)
                    //{
                    //    fire.paramNode = node;
                    //}
                    for (int i = 0; i < node.Children.Count; i++)
                    {
                        fire.ParamList.Add(node.Children[i].GetValue(this));
                    }
                }
                else if (node.Name == BLName.Wait)
                {
                    BulletMLWait wait = new BulletMLWait(node);
                    wait.Owner = this;
                    TaskList.Add(wait);
                }
                else if (node.Name == BLName.Speed)
                {
                    //BulletMLSetSpeed task = new BulletMLSetSpeed(node);
                    //task.owner = this;
                    //taskList.Add(task);
                    bullet.GetFireData().SpeedInit = true; // 値を明示的にセットしたことを示す
                    bullet.Speed = node.GetValue(this);

                }
                else if (node.Name == BLName.Direction)
                {
                    BulletMLSetDirection task = new BulletMLSetDirection(node);
                    task.Owner = this;
                    TaskList.Add(task);
                }
                else if (node.Name == BLName.Vanish)
                {
                    BulletMLVanish task = new BulletMLVanish();
                    task.Owner = this;
                    TaskList.Add(task);
                }
                else if (node.Name == BLName.Accel)
                {
                    BulletMLAccel task = new BulletMLAccel(node);
                    task.Owner = this;
                    TaskList.Add(task);
                }
                else
                {
                    ////Debug.WriteLine("node.name:{0}", node.name);
                }
            }
        }
    }
}