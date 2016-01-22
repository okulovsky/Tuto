using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Linq;

namespace Tuto
{
    [DataContract]
    public class NotifierModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
		
		
		public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			if (Subscribers!=null && Subscribers.ContainsKey(propertyName))
				Subscribers[propertyName]();
        }

        public void NotifyAll()
        {
            foreach (var e in GetType().GetProperties())
                NotifyPropertyChanged(e.Name);
        }

		Dictionary<string, Action> Subscribers = new Dictionary<string, Action>();

		public void Subscribe(string memberName, Action handler)
		{
			if (Subscribers == null) Subscribers = new Dictionary<string, Action>();
			if (!Subscribers.ContainsKey(memberName))
				Subscribers[memberName] = handler;
			else
				Subscribers[memberName] += handler;
		}

		public void UnsubscribeAll(object obj)
		{
			foreach(var e in Subscribers.Keys.ToList())
			{
				var value = Subscribers[e];
				foreach(var x in value.GetInvocationList())
				{
					if (x.Target == obj)
						value -= (Action)x;
				}
				Subscribers[e] = value;
			}
		}
    }

    public static class NotifierModelExtensions
    {
		public static string GetExpressionName<T>(Expression<Func<T,object>> field)
		{
			var expression = field.Body;
			if (expression.NodeType == ExpressionType.Convert)
				expression = ((UnaryExpression)expression).Operand;
			return (expression as MemberExpression).Member.Name;
		}

        public static void NotifyByExpression<T>(this T obj, Expression<Func<T, object>> field)
            where T : NotifierModel
        {
			obj.NotifyPropertyChanged(GetExpressionName<T>(field));
        }

		public static void SubsrcibeByExpression<T>(this T obj, Expression<Func<T,object>> field,Action action)
			where T : NotifierModel
		{
			obj.Subscribe(GetExpressionName<T>(field), action);
		}
    }
}