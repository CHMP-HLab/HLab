using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using HLab.Base;

namespace HLab.Notify.Annotations;

public interface INotifyPropertyChangedWithHelper : INotifyPropertyChanged
{
    INotifyClassHelper ClassHelper { get; }
}



public interface INotifyClassHelper
{
    SuspenderToken GetSuspender();
    IPropertyEntry GetPropertyEntry(string name);
    IEnumerable<IPropertyEntry> LinkedProperties();
    IEnumerable<IPropertyEntry> Properties();
    void Initialize<T>() where T : class, INotifyPropertyChangedWithHelper;
    void AddHandler(PropertyChangedEventHandler value);
    void RemoveHandler(PropertyChangedEventHandler value);
    void OnPropertyChanged(PropertyChangedEventArgs args);
}

public class TriggerPathNext : TriggerPath
{
    internal TriggerPathNext(TriggerPath next, string propertyName) : base(propertyName)
    {
        Next = next;
    }

    public override ITriggerEntry GetTrigger(INotifyClassHelper helper, EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
        return helper.GetPropertyEntry(PropertyName).BuildTrigger(Next, handler);
    }

    public override TriggerPath Next { get; }

    public override string ToString()
    {
        var sb = new StringBuilder(PropertyName);
        var next = Next;
        while (next != null)
        {
            sb.Append('.');
            sb.Append(next.PropertyName);
            next = next.Next;
        }
        return sb.ToString();
    }
}

public class TriggerPathNull : TriggerPath
{
    internal TriggerPathNull() : base(null)
    {
    }
}

public class TriggerPath
{

    internal TriggerPath(string propertyName)
    {
        PropertyName = propertyName;
    }

    public virtual TriggerPath Next => null;

    public virtual ITriggerEntry GetTrigger(INotifyClassHelper helper, EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
        return helper.GetPropertyEntry(PropertyName).BuildTrigger(handler);
    }

    public string PropertyName { get; }

    public override string ToString()
    {
        return PropertyName;
    }

    static TriggerPath Factory(TriggerPath next, string propertyName)
    {
        return next == null ? new TriggerPath(propertyName) : new TriggerPathNext(next, propertyName);
    }

    public static TriggerPath Factory(params string[] path)
    {
        var stack = new Stack<string>();
        foreach (var subPath in path)
        {
            var sub = subPath.Split('.');
            foreach (var propertyName in sub)
            {
                stack.Push(propertyName);
            }
        }

        TriggerPath triggerPath = null;

        while (stack.TryPop(out var propertyName))
            triggerPath = Factory(triggerPath, propertyName);

        return triggerPath ?? new TriggerPathNull();
    }

    public static TriggerPath Factory(Expression path)
    {
        var body = (path as LambdaExpression)?.Body;

        var e = body;
        TriggerPath triggerPath = null;

        while (e != null)
        {
            var name = "";
            if (e.NodeType == ExpressionType.Convert)
            {
                e = (e as UnaryExpression)?.Operand;
            }
            else
            {
                switch (e)
                {
                    case MemberExpression m:
                        name = m.Member.Name;
                        e = m.Expression;
                        break;
                    case MethodCallExpression mc:
                        name = mc.Method.Name;
                        e = mc.Arguments[0];
                        break;
                    default:
                        throw new ArgumentException("Error parsing expression : " + path.ToString());
                }
            }

            if (e is { NodeType: ExpressionType.Parameter })
            {
                e = null;
                triggerPath = Factory(triggerPath, name);
            }
            else if (!string.IsNullOrWhiteSpace(name))
            {
                triggerPath = Factory(triggerPath, name);
            }
        }

        return triggerPath;
    }
}