//#define ExpandedBulletML

using System;
using System.Xml;
using System.Diagnostics;

namespace BulletMLLib
{
    public class BulletMLParser
    {
        public BulletMLTree Tree;

        //static void Main(string[] args)
        //{
        //    Random r = new Random();
        //    BulletMLSystem.Init(r);
        //    BulletMLParser parser = new BulletMLParser();
        //    parser.ParseXML("test.xml");
        //    BulletMLSrc mover = new BulletMLSrc(parser.tree);
        //    for (int i = 0; i < 200; i++)
        //    {
        //        mover.Update();
        //    }
        //    Debug.Write("\n--end--\n");
        //    Debug.Read();
        
        //}

        public void ParseXml(string xmlFileName)
        {
            //Debug.WriteLine(" ----- " + xmlFileName + " ----- ");
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;

            settings.ValidationType = ValidationType.DTD;
            XmlReader reader = XmlReader.Create(xmlFileName, settings);
            BulletMLParser parser = new BulletMLParser();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // The node is an element.
                            //Debug.Write("<" + reader.Name + ">\n");

                            BulletMLTree element = new BulletMLTree();
                            element.Name = parser.StringToName(reader.Name);
                            if (reader.HasAttributes)
                            {
                                element.Type = parser.StringToType(reader.GetAttribute("type"));
                                element.Label = reader.GetAttribute("label");
#if ExpandedBulletML
                                element.visible = reader.GetAttribute("visible") == "false" ? false : true;
                                element.bulletName = reader.GetAttribute("name");
#endif
                            }

                            if (Tree == null)
                                Tree = element;
                            else
                            {
                                Tree.Children.Add(element);
                                if (Tree.Children.Count > 1)
                                    Tree.Children[Tree.Children.Count - 2].Next = Tree.Children[Tree.Children.Count - 1];

                                element.Parent = Tree;
                                if (!reader.IsEmptyElement)
                                    Tree = element;
                            }

                            break;

                        case XmlNodeType.Text: //Display the text in each element.

                            //Debug.WriteLine(reader.Value +"\n");

                            string line = reader.Value;
                            string word = "";
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (('0' <= line[i] && line[i] <= '9') || line[i] == '.')
                                {
                                    word = word + line[i];
                                    if (i < line.Length - 1) //まだ続きがあれば
                                        continue;
                                }

                                if (word != "")
                                {
                                    float num;
                                    if (float.TryParse(word, out num))
                                    {
                                        Tree.Values.Add(new BulletValue(BLValueType.Number, num));
                                        word = "";
                                        //Debug.WriteLine("数値を代入" + num);
                                    }
                                    else
                                    {
                                        //Debug.WriteLine("構文にエラーがあります : " + line[i]);
                                    }
                                }

                                if (line[i] == '$')
                                {
                                    if (line[i + 1] >= '0' && line[i + 1] <= '9')
                                    {
                                        Tree.Values.Add(new BulletValue(BLValueType.Param, Convert.ToInt32(line[i + 1].ToString())));
                                        i++;
                                        //Debug.WriteLine("パラメータを代入");
                                    }
                                    else if (line.Substring(i, 5) == "$rank")
                                    {
                                        //Debug.WriteLine("ランクを代入");
                                        i += 4;
                                        Tree.Values.Add(new BulletValue(BLValueType.Rank, 0));
                                    }
                                    else if (line.Substring(i, 5) == "$rand")
                                    {
                                        //Debug.WriteLine("Randを代入");
                                        i += 4;
                                        Tree.Values.Add(new BulletValue(BLValueType.Rand, 0));
                                    }
                                }
                                else if (line[i] == '*' || line[i] == '/' || line[i] == '+' || line[i] == '-' || line[i] == '(' || line[i] == ')')
                                {
                                    Tree.Values.Add(new BulletValue(BLValueType.Operator, line[i]));
                                    //Debug.WriteLine("演算子を代入 " + line[i]);
                                }
                                else if (line[i] == ' ' || line[i] == '\n')
                                {
                                }
                                else
                                {
                                    //Debug.WriteLine("構文にエラーがあります : " + line[i]);
                                }
                            }

                            break;
                        case XmlNodeType.EndElement: //Display the end of the element.
                            if (Tree.Parent != null)
                                Tree = Tree.Parent;

                            //Debug.Write("</" + reader.Name + ">\n");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }

            //Debug.WriteLine("\n-------------end-----------------");
        }
     
        string[] name2string = {
	        "bullet", "action", "fire", "changeDirection", "changeSpeed", "accel",
	        "wait", "repeat", "bulletRef", "actionRef", "fireRef", "vanish",
	        "horizontal", "vertical", "term", "times", "direction", "speed", "param",
	        "bulletml"
        };

        BLType StringToType(string str) 
        {
            switch (str)
            {
                case "aim":
                    return BLType.Aim;
                case "absolute":
                    return BLType.Absolute;
                case "relative":
                    return BLType.Relative;
                case "sequence":
                    return BLType.Sequence;
                default:
                    Debug.WriteLine("BulletML parser: unknown type " + str);
                    return BLType.None;
            }
        }

        //タグ文字列をBLNameに変換する
        BLName StringToName(string str)
        {
            Debug.WriteLine(" tag " + str);
            switch (str)
            {
                case "bulletml":
                    return BLName.Bulletml;
                case "bullet":
                    return BLName.Bullet;
                case "action":
                    return BLName.Action;
                case "fire":
                    return BLName.Fire;
                case "changeDirection":
                    return BLName.ChangeDirection;
                case "changeSpeed":
                    return BLName.ChangeSpeed;
                case "accel":
                    return BLName.Accel;
                case "vanish":
                    return BLName.Vanish;
                case "wait":
                    return BLName.Wait;
                case "repeat":
                    return BLName.Repeat;
                case "direction":
                    return BLName.Direction;
                case "speed":
                    return BLName.Speed;
                case "horizontal":
                    return BLName.Horizontal;
                case "vertical":
                    return BLName.Vertical;
                case "term":
                    return BLName.Term;
                case "bulletRef":
                    return BLName.BulletRef;
                case "actionRef":
                    return BLName.ActionRef;
                case "fireRef":
                    return BLName.FireRef;
                case "param":
                    return BLName.Param;
                case "times":
                    return BLName.Times;
                default:
                    Debug.WriteLine("BulletML parser: unknown tag " + str);
                    return BLName.None;
            }
        }
    }
}
