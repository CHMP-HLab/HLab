namespace HLab.Core.Annotations
{
    public interface IBootloader
    {
        void Load();
    }
    public interface IPostBootloader
    {
        void Load();
    }
}
