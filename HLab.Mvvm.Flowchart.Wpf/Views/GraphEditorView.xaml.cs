using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Flowchart.ViewModel;

namespace HLab.Mvvm.Flowchart.Views
{
    /// <summary>
    /// Logique d'interaction pour MonographGraphEditorView.xaml
    /// </summary>
    public partial class FlowchartEditorView : UserControl, 
        IView<ViewModeDefault,IGraphViewModel>,
        IViewClassFlowchart
    {
        public FlowchartEditorView(Func<GraphToolboxViewModel> getGraphToolboxViewModel)
        {
            _getGraphToolboxViewModel = getGraphToolboxViewModel;
            InitializeComponent();
            DataContextChanged += GraphEditorView_DataContextChanged;
        }

        void GraphEditorView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(ViewModel is IViewModel vm)
            {
                foreach (var block in ViewModel.Blocks)
                {
                    var view = vm.MvvmContext.GetView<ViewModeDefault>(block);
                    WorkArea.Children.Add((UIElement)view);
                }

                ViewModel.Blocks.CollectionChanged += Blocks_CollectionChanged;
            }


        }

        void Blocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ViewModel is IViewModel vm)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        var view = vm.MvvmContext.GetView<ViewModeDefault>(item);
                        WorkArea.Children.Add((UIElement)view);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                        foreach (var view in WorkArea.Children.OfType<BlockView>().ToList())
                        {
                            if (e.OldItems.Contains((view.DataContext as BlockViewModel)?.Model))
                            {
                                WorkArea.Children.Remove(view);
                            }
                        }
                }
            }
        }

        IGraphViewModel ViewModel => DataContext as IGraphViewModel;

        readonly Func<GraphToolboxViewModel> _getGraphToolboxViewModel;

        FrameworkElement _toolbox;

        void ShowToolBox(Point p)
        {
            if (_toolbox == null)
                _toolbox = (FrameworkElement)ViewModel.MvvmService.ViewHelperFactory.Get(this).Context.GetView<ViewModeDefault>(_getGraphToolboxViewModel());

            _toolbox.Margin = new Thickness(p.X + 20, p.Y - 20, 0, 0);

            if(!WorkArea.Children.Contains(_toolbox))
                WorkArea.Children.Add(_toolbox);

            _toolbox.Visibility = Visibility.Visible;
        }

        public void HideToolbox()
        {
            if (_toolbox != null) _toolbox.Visibility = Visibility.Hidden;
        }

        void LinkArea_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowToolBox(e.GetPosition(WorkArea));
        }

        void LinkArea_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HideToolbox();
        }
    }
}
