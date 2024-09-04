using System;

namespace HLab.Notify.PropertyChanged.PropertyHelpers;

public class PropertyAttribute : Attribute
{
    public string Name { get; }
    public PropertyAttribute(string name) => Name = name;
}