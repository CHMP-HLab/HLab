namespace HLab.Core.Annotations
{
    public interface IBootloader
    {
        bool Load();
    }
    public interface IBootloaderDependent : IBootloader
    {
        string[] DependsOn {get;}
    }
    //public interface IPostBootloader
    //{
    //    void Load();
    //}
}
