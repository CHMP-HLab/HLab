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
            pdIsAvailable.AddValueChanged(this, async (o,h) => await UpdateAsync(Language, LocalizationService, Id,StringFormat));
        }

        public string Id
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
        public static readonly DependencyProperty IdProperty =
            H.Property<string>()
                .OnChange(async(s,  e) => await s.UpdateAsync(s.Language, s.LocalizationService, e.NewValue, s.StringFormat))
                .AffectsRender
                .Register();

        public static readonly DependencyProperty StringFormatProperty =
            H.Property<string>()
//                .Default("{}{0}")
                .OnChange(async (s, e) => await s.UpdateAsync(s.Language, s.LocalizationService, s.Id, e.NewValue))
                .AffectsRender
                .Register();

        public static readonly DependencyProperty LocalizationServiceProperty =
            H.Property<ILocalizationService>()
                .OnChange(async (s, e) =>
                {
                        await s.UpdateAsync(s.Language, e.NewValue, s.Id, s.StringFormat);
                })
                .Inherits.AffectsRender
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
            if(service==null) return;

            if (string.IsNullOrWhiteSpace(source))
            {
                Text = "";
                return;
            }

            string localized = source;
            try
            {
                localized = await service.LocalizeAsync(lang.IetfLanguageTag, source).ConfigureAwait(true);

            }
            catch (Exception e)
            {
            }

            if (string.IsNullOrEmpty(format))
                Text = localized;
            else
            {
                try
                {
                    Text = string.Format(format, localized);
                }
                catch(FormatException e)
                {
                    Text = "F(" + format + ")" + localized;
                }
            }
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
