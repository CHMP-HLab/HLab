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
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Notify;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    public abstract class GraphElement<T> : N<T>, IGraphElement
    where T : GraphElement<T>
    {
        [Import]
        public IMvvmService MvvmService { get; }

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

                    if (v is IList l)
                    {
                        lists.Add(new Tuple<string, IList>(p.Name, l));
                        continue;
                    }

                    if (v == null) continue;
                    if (v is IFormattable f)
                        w.WriteAttributeString(p.Name, f.ToString(null, CultureInfo.InvariantCulture));
                    else
                        w.WriteAttributeString(p.Name, v.ToString());
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

        private readonly IProperty<string> _id = H.Property<string>(c => c.Default(""));

        public IGraphElement Parent
        {
            get => _parent.Get();
            set => _parent.Set(value);
        }

        private readonly IProperty<IGraphElement> _parent = H.Property<IGraphElement>(c => c.Default(default));


        [DataMember]
        public virtual string Caption
        {
            get => _caption.Get();
            set => _caption.Set(value);
        }

        private readonly IProperty<string> _caption = H.Property<string>(c => c.Default(""));



        public virtual Color Color => Colors.White;
    }
}