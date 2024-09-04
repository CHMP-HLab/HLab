using System;

namespace HLab.Mvvm.Annotations;

public interface IProgressProvider
{
    event EventHandler<ProgressEventArgs> Progress;
}
