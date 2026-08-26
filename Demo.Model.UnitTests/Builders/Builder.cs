using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Reflection;

namespace Demo.Model.UnitTests
{
    public class Builder<T> 
    {
        protected Builder(T target)
        {
            Target = target;
        }

        protected T Target { get; set; }


        /// <summary>
        /// Sets a property value
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression">The property expression</param>
        /// <param name="newValue">The value to set</param>
        /// <returns>The builder, for further fluent style modifications</returns>
        public Builder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, TProperty newValue)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var property = (PropertyInfo)memberExpression.Member;
            property.SetValue(Target, newValue, null);

            return this;
        }

        /// <summary>
        /// Build a clone using Json Serialization / Deserialization approach
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Builder<T> BuildFrom(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            Target = JsonConvert.DeserializeObject<T>(serialized);

            return this;
        }

        /// <summary>
        /// Build the underlying target object
        /// </summary>
        /// <returns></returns>
        public virtual T Build()
        {
            return Target;
        }
    }
}
