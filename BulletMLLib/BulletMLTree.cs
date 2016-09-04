//#define ExpandedBulletML

using System.Collections.Generic;

namespace BulletMLLib
{
    // このインタフェースを実装し、BulletMLManager.Init()を初期化のために必ず呼ぶこと。

    public class BulletMLTree
    {
        public BLName Name;
        public BLType Type;
        public string Label;
        public BulletMLTree Parent;
        public BulletMLTree Next;
        public readonly List<BulletValue> Values;
        public readonly List<BulletMLTree> Children;
#if ExpandedBulletML
        public bool visible;
        public string bulletName;
#endif
        public BulletMLTree()
        {
            Children = new List<BulletMLTree>();
            Values = new List<BulletValue>();
            Parent = null;
            Next = null;
#if ExpandedBulletML
            visible = true;
            bulletName = "";
#endif
        }

        public BulletMLTree GetLabelNode(string label, BLName name)
        {
            BulletMLTree rootNode = this; //先頭までさかのぼる

            while (rootNode.Parent != null)
                rootNode = rootNode.Parent;

            foreach (BulletMLTree tree in rootNode.Children)
            {
                if (tree.Label == label && tree.Name == name)
                    return tree;
            }

            return null;
        }

        public float GetChildValue(BLName name, BulletMLTask task)
        {
            foreach (BulletMLTree tree in Children)
            {
                if (tree.Name == name)
                    return tree.GetValue(task);
            }
            return 0;
        }

        public BulletMLTree GetChild(BLName name)
        {
            foreach (BulletMLTree node in Children)
            {
                if (node.Name == name)
                    return node;
            }
            return null;
        }

        public float GetValue(BulletMLTask task)
        {
            int startIndex = 0;

            return GetValue(0, ref startIndex, task);
        }

        public float GetValue(float v, ref int i, BulletMLTask task)
        {

            for (; i < Values.Count; i++)
            {

                if (Values[i].ValueType == BLValueType.Operator)
                {
                    if (Values[i].Value == '+')
                    {
                        i++;
                        if (IsNextNum(i))
                            v += GetNumValue(Values[i], task);
                        else
                            v += GetValue(v, ref i, task);
                    }
                    else if (Values[i].Value == '-')
                    {
                        i++;
                        if (IsNextNum(i))
                            v -= GetNumValue(Values[i], task);
                        else
                            v -= GetValue(v, ref i, task);
                    }
                    else if (Values[i].Value == '*')
                    {
                        i++;
                        if (IsNextNum(i))
                            v *= GetNumValue(Values[i], task);
                        else
                            v *= GetValue(v, ref i, task);
                    }
                    else if (Values[i].Value == '/')
                    {
                        i++;
                        if (IsNextNum(i))
                            v /= GetNumValue(Values[i], task);
                        else
                            v /= GetValue(v, ref i, task);
                    }
                    else if (Values[i].Value == '(')
                    {
                        i++;
                        float res = GetValue(v, ref i, task);
                        if ((i < Values.Count - 1 && Values[i + 1].ValueType == BLValueType.Operator)
                               && (Values[i + 1].Value == '*' || Values[i + 1].Value == '/'))
                            return GetValue(res, ref i, task);
                        else
                            return res;
                    }
                    else if (Values[i].Value == ')')
                    {
                        //Debug.WriteLine(" ）の戻り値:" + v);
                        return v;
                    }
                }
                else if (i < Values.Count - 1 && Values[i + 1].ValueType == BLValueType.Operator && Values[i + 1].Value == '*')
                {
                    // 次が掛け算のとき
                    float val = GetNumValue(Values[i], task);
                    i += 2;
                    if (IsNextNum(i))
                        return val * GetNumValue(Values[i], task);
                    else
                        return val * GetValue(v, ref i, task);
                }
                else if (i < Values.Count - 1 && Values[i + 1].ValueType == BLValueType.Operator && Values[i + 1].Value == '/')
                {
                    // 次が割り算のとき
                    float val = GetNumValue(Values[i], task);
                    i += 2;
                    if (IsNextNum(i))
                        return val / GetNumValue(Values[i], task);
                    else
                        return val / GetValue(v, ref i, task);
                }
                else
                    v = GetNumValue(Values[i], task);

            }

            return v;
        }

        bool IsNextNum(int i)
        {
            if ((i < Values.Count - 1 && Values[i + 1].ValueType == BLValueType.Operator) && (Values[i + 1].Value == '*' || Values[i + 1].Value == '/'))
            {
                return false;
            }
            else if (Values[i].Value == ')' || Values[i].Value == '(')
            {
                return false;
            }
            else
                return true;
        }

        float GetNumValue(BulletValue v, BulletMLTask task)
        {
            if (v.ValueType == BLValueType.Number)
            {
                return v.Value;
            }
            else if (v.ValueType == BLValueType.Rand)
            {
                return (float)BulletMLManager.GetRandom();
            }
            else if (v.ValueType == BLValueType.Rank)
            {
                return BulletMLManager.GetRank();
            }
            else if (v.ValueType == BLValueType.Param)
            {

                BulletMLTask ownerTask = task;
                while (ownerTask.ParamList.Count == 0)
                    ownerTask = ownerTask.Owner;
                float val = ownerTask.ParamList[(int)v.Value - 1];
                
                //BulletMLTask ownerTask = task;
                //while (ownerTask.paramNode == null)
                //    ownerTask = ownerTask.owner;
                //float val = ownerTask.paramNode.children[(int)v.value - 1].GetValue(ownerTask.owner);
                
                //Debug.WriteLine(String.Format( "{2} param{0} = {1}", (int)v.value - 1, val, ownerTask));
                return val;
            }
            else
            {
                //Debug.WriteLine("不正な値がパラメータになっています");
                return 0;
            }
        }

        internal float GetParam(int p, BulletMLTask task)
        {
            return Children[p].GetValue(task); //<param>以外のタグは持っていないので
        }
    }
}
