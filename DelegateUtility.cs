using System;
using System.Linq.Expressions;
using System.Reflection;

public static class DelegateUtility
{
	public static Delegate AddEventHandler(object target, string eventHandler, Action callback)
	{
		EventInfo eventInfo = target.GetType().GetEvent(eventHandler);
		Delegate obj = CreateDelegate(eventInfo, callback);
		eventInfo.AddEventHandler(target, obj);
		return obj;
	}

	public static void RemoveEventHandler(object target, string eventHandler, Delegate handler)
	{
		target.GetType().GetEvent(eventHandler).RemoveEventHandler(target, handler);
	}

	public static Delegate CreateDelegate(EventInfo ev, Action action)
	{
		ParameterExpression[] parameters = Array.ConvertAll(ev.EventHandlerType.GetMethod("Invoke").GetParameters(), (ParameterInfo x) => Expression.Parameter(x.ParameterType, x.Name));
		MethodCallExpression body = Expression.Call(Expression.Constant(action.Target), action.Method);
		return Expression.Lambda(ev.EventHandlerType, body, parameters).Compile();
	}
}
