﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public class IconProviderXamlFromUri : IIconProvider
    {
        private readonly Uri _uri;
        public IconProviderXamlFromUri(Uri uri)
        {
            _uri = uri;
        }

        
        public async Task<object> GetAsync()
        {
            var icon = Application.LoadComponent(_uri);
            if(icon is DependencyObject d)
                XamlTools.SetBinding(d);

            return icon;
        }
    }
}