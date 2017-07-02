using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace VPackager
{
    public static class Lang
    {
        public static Language Current { get; set; }
        
        public static bool Contains(string name)
        {
            if (Current != null)
                return Current.Words.ContainsKey(name);
            else
                return false;
        }

        public static string _(string name)
        {
            return GetText(name);
        }

        public static string GetText(string name)
        {
            if (Current != null && Current.Words.ContainsKey(name))
                return Current.Words[name];
            else
                return name;
        }

        public static string GetText(string name, string defaultValue)
        {
            if (name == null)
                return defaultValue;

            if (Current != null && Current.Words.ContainsKey(name))
            {
                return Current.Words[name];
            }
            else
            {
                return defaultValue;
            }
        }

        public static string GetText2(string name1, string name2)
        {
            if (name1 != null && Current != null && Current.Words.ContainsKey(name1))
                return Current.Words[name1];
            else if (name2 != null && Current != null && Current.Words.ContainsKey(name2))
                return Current.Words[name2];
            else if (string.IsNullOrEmpty(name1))
                return name2;
            else
                return name1;
        }
        
        public static string GetTextWithEllipsis(string name)
        {
            if (Current != null && Current.Words.ContainsKey(name))
                return Current.Words[name] + "...";
            else
                return name + "...";
        }

        public static string GetTextWithColon(string name)
        {
            return GetText(name, false, true, '\0');
        }

        public static string GetTextWithAccelerator(string name, bool withEllipsis, char accelerator)
        {
            return GetText(name, true, false, accelerator);
        }

        public static string GetTextWithAccelerator(string name, char accelerator)
        {
            return GetText(name, false, false, accelerator);
        }
        
        public static string GetText(string name, bool withEllipsis, bool withColon, char accelerator)
        {
            var str = (Current != null && Current.Words.ContainsKey(name)) ? Current.Words[name] : name;
            if (str == null)
                return str;

            if (accelerator > 0)
                str = StringHelper.AddAccelerator(str, accelerator);

            if (withEllipsis)
                str += "...";

            if (withColon)
                str += ":";

            return str;
        }

        public static string Format(string name, params object[] args)
        {
            return Format(name, true, args);
        }

        public static string Format(string name, bool withArgs, params object[] args)
        {
            if (withArgs && args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    object arg = args[i];
                    if (arg is string)
                    {
                        args[i] = GetText((string)arg);
                    }
                }
            }

            return string.Format(GetText(name), args);
        }

        public static void Localize(UIElement control)
        {
            if (control != null)
            {
                if (!string.IsNullOrEmpty(control.Uid))
                {
                    if (control is TextBlock)
                    {
                        var tb = (TextBlock)control;
                        tb.Text = GetText(control.Uid);
                    }
                    else if (control is Page)
                    {
                        var page = (Page)control;
                        page.Title = GetText(control.Uid);
                    }
                    else if (control is HeaderedContentControl)
                    {
                        var hcc = (HeaderedContentControl)control;
                        if (hcc.HasHeader && hcc.Header is string)
                            hcc.Header = GetText(control.Uid);
                    }
                    else if (control is ContentControl && ((ContentControl)control).Content is string)
                    {
                        var cc = (ContentControl)control;
                        cc.Content = Lang.GetText(control.Uid);
                    }
                }               

                if (control is ContentControl && ((ContentControl)control).Content is UIElement)
                {
                    Localize((UIElement)((ContentControl)control).Content);
                }
                else if(control is Decorator)
                {
                    Localize(((Decorator)control).Child);
                }
                else if(control is Page && ((Page)control).Content is UIElement)
                {
                    Localize((UIElement)((Page)control).Content);
                }
                else if (control is Panel)
                {
                    var panel = (Panel)control;
                    foreach (UIElement child in panel.Children)
                    {
                        Localize(child);
                    }
                }
            }
        }

        #region load xml 
        public static Language LoadXml(string xml)
        {
            var dom = new XmlDocument();
            dom.LoadXml(xml);
            return LoadXml(dom);
        }

        public static Language LoadXmlFile(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;

            var dom = new XmlDocument();
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                dom.Load(stream);
                return LoadXml(dom);
            }
        }

        public static Language LoadXml(Stream stream)
        {
            var dom = new XmlDocument();
            try
            {
                dom.Load(stream);
            }
            catch
            {
                return null;
            }

            return LoadXml(dom);
        }

        public static Language LoadXml(XmlDocument dom)
        {
            if (dom.DocumentElement == null)
                return null;

            var infoElement = dom.DocumentElement.SelectSingleNode("information") as XmlElement;
            if (infoElement == null)
                return null;

            var language = new Language();
            language.Id = StringHelper.GetIntDefault(infoElement.GetAttribute("id"));
            language.Number = infoElement.GetAttribute("no");
            language.Name = infoElement.GetAttribute("name");
            language.IsRightToLeft = StringHelper.GetBoolDefault(infoElement.GetAttribute("is_right_to_left"));
            
            var wordsNode = dom.DocumentElement.SelectSingleNode("words");
            if (wordsNode != null)
            {
                LoadXmlNodes(language.Words, wordsNode);
            }

            return language;
        }

        static void LoadXmlNodes(Dictionary<string, string> words, XmlNode group)
        {
            XmlNodeList list = group.SelectNodes("item");
            if (list != null)
            {
                foreach (XmlElement node in list)
                {
                    string name = node.GetAttribute("name");
                    if (!words.ContainsKey(name))
                        words.Add(name, node.InnerText);
                }
            }
        }
        #endregion
    }
}
