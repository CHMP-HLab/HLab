namespace HLab.Mvvm;

public class ProgressMessage
{
    public ProgressMessage(double progress, string text)
    {
        Progress = progress;
        Text = text;
    }

    public double Progress { get; }
    public string Text { get; }
}