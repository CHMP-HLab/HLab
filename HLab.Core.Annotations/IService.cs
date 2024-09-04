namespace HLab.Core.Annotations;

public enum ServiceState
{
    NotConfigured,
    Available
}


public interface IService
{
    public ServiceState ServiceState { get; }
}