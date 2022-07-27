using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    using H = H<GraphElement>;

    public abstract class GraphElement : NotifierBase, IGraphElement
    {
        public IMvvmService MvvmService { get; set; }

        protected GraphElement()
        {
            H.Initialize(this);
        }


        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteXml(XmlWriter w)
        {
            w.WriteStartElement(GetType().Name.Split('`')[0]);
            WriteXmlAttributes(w);
            w.WriteEndElement();
        }
        public void WriteXmlAttributes(XmlWriter w)
        {
            var lists = new List<Tuple<string, IList>>();

            foreach (var p in GetType().GetProperties().ToList())
            {
                foreach (var attribute in p.GetCustomAttributes().OfType<DataMemberAttribute>())
                {
                    var v = p.GetValue(this);

                    switch (v)
                    {
                        case IList l:
                            lists.Add(new Tuple<string, IList>(p.Name, l));
                            continue;

                        case null:
                            continue;

                        case IFormattable f:
                            w.WriteAttributeString(p.Name, f.ToString(null, CultureInfo.InvariantCulture));
                            break;

                        default:
                            w.WriteAttributeString(p.Name, v.ToString());
                            break;
                    }
                }
            }

            foreach (var list in lists)
            {
                WriteXmlList(w, list.Item2, list.Item1);
            }
        }
        public void WriteXmlList(XmlWriter w, IEnumerable en, string name)
        {
            var list = en.OfType<IXmlSerializable>().ToList();
            if (list.Count < 1) return;

            w.WriteStartElement(name);
            foreach (var e in list)
            {
                e.WriteXml(w);
            }
            w.WriteEndElement();
        }

        [DataMember]
        public string Id
        {
            get => _id.Get();
            set => _id.Set(value);
        }

        readonly IProperty<string> _id = H.Property<string>(c => c.Default(""));

        public IGraphElement Parent
        {
            get => _parent.Get();
            set => _parent.Set(value);
        }

        readonly IProperty<IGraphElement> _parent = H.Property<IGraphElement>(c => c.Default((IGraphElement)default));


        [DataMember]
        public virtual string Caption
        {
            get => _caption.Get();
            set => _caption.Set(value);
        }

        readonly IProperty<string> _caption = H.Property<string>(c => c.Default(""));

        [DataMember]
        public virtual string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        readonly IProperty<string> _iconPath = H.Property<string>(c => c.Default(""));



        public virtual Color Color => Colors.White;
    }
}