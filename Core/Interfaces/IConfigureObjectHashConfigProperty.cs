using System;
using System.Linq.Expressions;

namespace ObjectHashing.Interfaces
{
    public interface IConfigureObjectHashConfigProperty<TSource> : IConfigureObjectHashConfigSerialization<TSource>
    {
        /// <summary>
        /// Specifies property to include in the projection of object to be hashed.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public IConfigureObjectHashConfigProperty<TSource> Property(Expression<Func<TSource, object>> property);
        
        /// <summary>
        /// Specifies all properties to be include in the projection of object to be hashed.
        /// </summary>
        /// <returns></returns>
        public IConfigureObjectHashConfigSerialization<TSource> AllProperties();
    }
}