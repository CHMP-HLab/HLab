using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HLab.Notify.Annotations
{
    public class TriggerPath
    {
        public TriggerPath(TriggerPath next, string propertyName)
        {
            Next = next;
            PropertyName = propertyName;
        }

        public TriggerPath(Expression path)
        {
            var body = (path as LambdaExpression)?.Body ;
            if (body?.NodeType == ExpressionType.Convert)
                body = (body as UnaryExpression)?.Operand;

            var e = body;
            TriggerPath triggerPath = null;

            while (e != null)
            {
                var name = "";
                if (e is MemberExpression m)
                {
                    name = m.Member.Name;
                    e = m.Expression;
                }
                else if (e is MethodCallExpression mc)
                {
                    name = mc.Method.Name;
                    e = mc.Arguments[0];
                }
                else
                {
                    throw new ArgumentException("Error parsing expression : " + path.ToString());
                }

                if (e.NodeType == ExpressionType.Parameter)
                {
                    e = null;
                    Next = triggerPath;
                    PropertyName = name;
                }
                else
                {
                    triggerPath = new TriggerPath(triggerPath, name);
                }
            }
        }

        public TriggerPath(params string[] path)
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

            while (stack.Count > 1)
                triggerPath = new TriggerPath(triggerPath, stack.Pop());

            Next = triggerPath;
            PropertyName = (stack.Count > 0)?stack.Pop():"";
        }

        public TriggerPath Next { get; }
        public string PropertyName { get; }

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
}