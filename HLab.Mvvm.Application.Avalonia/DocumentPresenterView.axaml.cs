﻿using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HLab.Mvvm.Annotations;
using HLab.Options;

namespace HLab.Mvvm.Application.Avalonia;

/// <summary>
/// Logique d'interaction pour DocumentPresenterView.xaml
/// </summary>
public partial class DocumentPresenterView : UserControl, IView<DocumentPresenterViewModel>
{
    readonly IOptionsService _options;
    int? UserId => null; // TODO _acl.Connection.UserId
    public DocumentPresenterView(IOptionsService options)
    {
        _options = options;
        InitializeComponent();

        Unloaded += DocumentPresenterView_Unloaded;
    }

    void DocumentPresenterView_Unloaded(object sender, RoutedEventArgs e)
    {
        SaveLayout();
    }

    void SaveLayout()
    {
        //var layoutSerializer = new XmlLayoutSerializer(DockingManager);
        //var sb = new StringBuilder();
        //using var writer = new StringWriter(sb);
        //layoutSerializer.Serialize(writer);
        //_options.SetValue<string>("","Layout", sb.ToString(),"", UserId);
    }

    async void LoadLayout()
    {
        //var layoutSerializer = new XmlLayoutSerializer(DockingManager);

        //try
        //{
        //    var layout = await _options.GetValueAsync<string>("","Layout",null,null,UserId).ConfigureAwait(true);
        //    if (!string.IsNullOrWhiteSpace(layout))
        //    {
        //        using var reader = new StringReader(layout);
        //        layoutSerializer.Deserialize(reader);
        //    }
        //}
        //catch (FileNotFoundException ex)
        //{
        //    var res = MessageBox.Show(ex.Message, "Debug", MessageBoxButton.OK,
        //        MessageBoxImage.Information);
        //}
        //catch (DirectoryNotFoundException ex)
        //{
        //    var res = MessageBox.Show(ex.Message, "Debug", MessageBoxButton.OK,
        //        MessageBoxImage.Information);
        //}
        //catch (InvalidOperationException ex)
        //{
        //    var res = MessageBox.Show(ex.Message, "Debug", MessageBoxButton.OK,
        //        MessageBoxImage.Information);                
        //}

    }

}