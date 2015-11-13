using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Tuto
{
    [DataContract]
    public class NotifierModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class NotifierModelExtensions
    {
        public static void NotifyByExpression<T>(this T obj, Expression<Func<T, object>> field)
            where T : NotifierModel
        {
			var expression = field.Body;
			if (expression.NodeType == ExpressionType.Convert)
				expression = ((UnaryExpression)expression).Operand;
            obj.NotifyPropertyChanged((expression as MemberExpression).Member.Name);
        }
    }
}