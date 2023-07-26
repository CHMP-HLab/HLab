namespace HLab.Mvvm.Application;

public interface IListableModel
{
    string Caption => $"{{{GetType()}}}";
    string IconPath => $"icons/entities/{GetType().Name}";
}
