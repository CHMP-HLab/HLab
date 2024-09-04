namespace HLab.Core.Annotations;

public interface INotificationProvider
{
    void SetIcon(object icon);
    void Notify(string message);
}