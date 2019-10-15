using System;

namespace HLab.Notify.PropertyChanged
{
    public class PropertyAttribute : Attribute
    {
        public string Name { get; }
        public PropertyAttribute(string name) => Name = name;
    }
}