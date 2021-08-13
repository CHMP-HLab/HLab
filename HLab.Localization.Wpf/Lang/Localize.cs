using System;
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
            System.ComponentModel.DependencyPropertyDescriptor pdIsAvailable = System.ComponentModel.DependencyPropertyDescriptor.FromProperty
                    (LanguageProperty, typeof(FrameworkElement));

            pdIsAvailable.AddValueChanged(this, async (o, a) =>
            {
                if(LocalizationService == null) return;
                if(Id == null) return;
                await UpdateAsync(Language, LocalizationService, Id, StringFormat);
            });

        }

        private WeakReference<ILocalizationService> _localizationServiceReference;
        

        public string Id
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
        public static readonly DependencyProperty IdProperty =
            H.Property<string>()
                .OnChange(async (e, a) =>
                {
                    if(e.LocalizationService ==  null) return;
                    if(e.Language ==  null) return;

                    await e.UpdateAsync(e.Language, e.LocalizationService, a.NewValue, e.StringFormat);
                })
                .Register();

        public static readonly DependencyProperty StringFormatProperty =
            H.Property<string>()
//                .Default("{}{0}")
                .OnChange(async (e, a) =>
                {
                    if(e.LocalizationService ==  null) return;
                    if(e.Language ==  null) return;
                    await e.UpdateAsync(e.Language, e.LocalizationService, e.Id, a.NewValue);
                })
                .Register();

        public static readonly DependencyProperty LocalizationServiceProperty =
            H.Property<ILocalizationService>()
                .OnChange(async (e, a) =>
                {
                        if(a.NewValue ==  null) return;
                        if(e._localizationServiceReference!=null && e._localizationServiceReference.TryGetTarget(out var service) && ReferenceEquals(a.NewValue,service)) return;

                        e._localizationServiceReference = new(a.NewValue);

                        if(e.Language ==  null) return;
                        if(e.Id == null) return;

                        await e.UpdateAsync(e.Language, a.NewValue, e.Id, e.StringFormat);
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

        public async Task UpdateAsync(XmlLanguage lang, ILocalizationService service, string source, string format)
        {
            if (service == null) return;

            if (string.IsNullOrWhiteSpace(source))
            {
                Text = "";
                return;
            }

            string localized = source;
            try
            {
                localized = await service.LocalizeAsync(lang.IetfLanguageTag, source).ConfigureAwait(false);

            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(format))
            {
                try
                {
                    localized = string.Format(format, localized);
                }
                catch(FormatException)
                {
                    localized = "F(" + format + ")" + localized;
                }
            }

            Dispatcher.Invoke(() => Text = localized);
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
