using System;
using System.ComponentModel;
using HLab.Notify.PropertyChanged;
using Microsoft.Win32;

namespace HLab.Notify.Wpf
{
    public interface IRegistryStored
    {
        string Key { get; }
    }

    public class RegistryPersister : Persister
    {
        public RegistryPersister(INotifyPropertyChanged obj, string key) : base(obj)
        {
            Key = key;
        }

        public string Key { get; set; }

        protected override bool Load(string propertyName, out object value)
        {
            var none = new object();
            value = none;
            if(string.IsNullOrEmpty(Key)) return false;

            var key = Registry.CurrentUser.OpenSubKey(Key, false);
            if(key==null) return false;

            value = key.GetValue(propertyName,none);
            if (ReferenceEquals(value,none) )
                return false;

            return true;
        }

        protected override void Save(string entry, object value)
        {
            if(value==null)
                try{Registry.CurrentUser.OpenSubKey(Key,true)?.DeleteValue(entry);}
                catch (ArgumentException) { }
            else
                Registry.CurrentUser.CreateSubKey(Key,true).SetValue(entry,value);
        }
    }


    public class PersisterLoadException : Exception
    {
        
    }
}
