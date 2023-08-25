namespace HLab.Mvvm.Annotations;

public class LocalizeEntryDesign : ILocalizeEntry
{
    public LocalizeEntryDesign(string code)
    {
        Tag = string.Empty;
        Code = code;
        Value = code;
    }

    public string Tag { get; set; }
    public string Code { get; set; }
    public string Value { get; set; }
}