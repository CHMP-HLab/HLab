using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using HLab.Base;
using HLab.Mvvm.Flowchart.Models;

namespace HLab.Mvvm.Flowchart
{
    public class GraphService : IGraphService
    {
        readonly Func<IEnumerable<IToolGraphBlock>> _getBlocks;
        public GraphService(Func<IEnumerable<IToolGraphBlock>> getBlocks)
        {
            _getBlocks = getBlocks;
            // TODO :
            /*
            Register();
            */
        }

        public void Register()
        {
            Register(Assembly.GetAssembly(GetType()));
            foreach (var assembly in AssemblyHelper.GetReferencingAssemblies(GetType()))
            {
                Register(assembly);
            }
        }

        public ObservableCollection<IToolGraphBlock> Blocks { get; } = new ObservableCollection<IToolGraphBlock>();
        public ConcurrentDictionary<string,Type> Types = new ConcurrentDictionary<string, Type>();
        public ConcurrentDictionary<string,GraphValueType> ValueTypes = new ConcurrentDictionary<string, GraphValueType>();


        public void Register(Assembly assembly)
        {
            var elements = assembly.GetTypes().Where(t => typeof(IGraphElement).IsAssignableFrom(t)).ToList();
            foreach (var block in _getBlocks())
            {
                    Blocks.Add(block);

                Types.GetOrAdd(block.GetType().Name,n => block.GetType());
            }

            //var pins = assembly.GetTypes().Where(t => typeof(IPin).IsAssignableFrom(t)).ToList();
            //foreach (var type in pins)
            //{
            //    Types.GetOrAdd(type.Name,n => type);
            //}
        }




        public void LoadXmlAttributes(IGraphElement o, XmlElement r)
        {
            foreach (var p in o.GetType().GetProperties().ToList())
            {
                if (p.GetCustomAttributes().OfType<DataMemberAttribute>().Any())
                {
                    if (typeof(IList).IsAssignableFrom(p.PropertyType))
                    {
                        var list = (IList)p.GetValue(o);
                        foreach (var e in r.ChildNodes.OfType<XmlElement>().Where(n => n.LocalName == p.Name))
                        {
                            LoadXmlCollection(o, e, list);
                        }
                        continue;
                    }

                    if (!p.CanWrite) continue;
                    try
                    {
                        var attribute = r.Attributes.OfType<XmlAttribute>().FirstOrDefault(a => a.LocalName == p.Name);

                        if (attribute == null) continue;

                        var v = attribute.Value;//r.GetAttribute(p.Name);

                        if (p.PropertyType == typeof(double))
                        {
                            p.SetValue(o, double.Parse(v, CultureInfo.InvariantCulture));
                        }
                        if (p.PropertyType == typeof(int))
                        {
                            p.SetValue(o, int.Parse(v, CultureInfo.InvariantCulture));
                        }
                        if (p.PropertyType == typeof(string))
                        {
                            p.SetValue(o, v);
                        }
                    }
                    catch (ArgumentException)
                    { }
                }
            }
        }

        public void LoadXmlCollection(IGraphElement parent, XmlElement element, IList list)
        {
            foreach (var xe in element.ChildNodes.OfType<XmlElement>())
            {
                var id = xe.GetAttribute("Id");
                var ge = list.OfType<IGraphElement>().FirstOrDefault(b => b.Id == id);
                if (ge == null)
                {
                    if (Types.TryGetValue(xe.Name, out var type))
                    {
                        ge = (IGraphElement)Activator.CreateInstance(type);
                        ge.Parent = parent;
                        ge.Id = id;
                        LoadXmlAttributes(ge, xe);
                        list.Add(ge);
                    }
                    else throw new ArgumentException(xe.Name + " type not found");
                }
                else
                    LoadXmlAttributes(ge, xe);
            }
        }


    }

    public static class XmlSerialisableExtention
    {
    }
}
