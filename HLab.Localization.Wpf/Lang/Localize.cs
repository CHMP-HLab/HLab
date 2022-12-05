using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using HLab.Base.Wpf;
using HLab.Mvvm.Annotations;

namespace HLab.Localization.Wpf.Lang
{
    using H = DependencyHelper<Localize>;

    [ContentProperty(nameof(Id))]
    public class Localize : TextBlock
    {
        public Localize()
        {
            if(DesignerProperties.GetIsInDesignMode(this))
                _updateAsync = UpdateDesignModeAsync;
            else
                _updateAsync = InitAsync;

            var pdIsAvailable = System.ComponentModel.DependencyPropertyDescriptor.FromProperty
                    (LanguageProperty, typeof(FrameworkElement));

            pdIsAvailable.AddValueChanged(this, async (o, a) =>
            {
                await _updateAsync();
            });

        }

        public string Id
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
        public static readonly DependencyProperty IdProperty =
            H.Property<string>()
                .OnChange(async (e, a) =>
                {

                    await e._updateAsync();
                })
                .Register();

        public static readonly DependencyProperty StringFormatProperty =
            H.Property<string>()
//                .Default("{}{0}")
                .OnChange(async (e, a) =>
                {
                    if (a.NewValue == null)
                    {
                        e._updateAsync = e.InitAsync;
                    }
                    await e._updateAsync();
                })
                .Register();

        public static readonly DependencyProperty LocalizationServiceProperty =
            H.Property<ILocalizationService>()
                .OnChange(async (e, a) =>
                {
                    if (a.NewValue == null)
                    {
                        e._updateAsync = e.InitAsync;
                    }
                    await e._updateAsync();
                })
                .Inherits
                .RegisterAttached();


        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        public ILocalizationService LocalizationService
        {
            get => (ILocalizationService)GetValue(LocalizationServiceProperty);
            set => SetValue(LocalizationServiceProperty, value);
        }

        Func<Task> _updateAsync;

        public async Task InitAsync()
        {
            if (LocalizationService == null) return;
            if (Language == null) return;
            if (Id == null) return;
            _updateAsync = UpdateAsync;
            await UpdateAsync();
        }

        public async Task UpdateAsync()
        {
            var localized = Id;
            try
            {
                localized = await LocalizationService.LocalizeAsync(Language.IetfLanguageTag, localized).ConfigureAwait(false);
            }
            catch (Exception)
            {
            }

            await Dispatcher.InvokeAsync(() => Text = localized);
        }
        public async Task UpdateDesignModeAsync()
        {
            await Dispatcher.InvokeAsync(() => Text = Id);
        }


        public static ILocalizationService GetLocalizationService(DependencyObject obj)
        {
            return (ILocalizationService)obj.GetValue(LocalizationServiceProperty);
        }
        public static void SetLocalizationService(DependencyObject obj, ILocalizationService value)
        {
            obj.SetValue(LocalizationServiceProperty, value);
        }
    }
}
