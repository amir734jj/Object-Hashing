using System;

namespace ObjectHashing.Interfaces
{
    public interface IConfigureObjectHashConfigSerialization<out TSource>
    {
        /// <summary>
        /// Specifies the default serialization to be used which is JSON.net.
        /// </summary>
        /// <returns></returns>
        public IConfigureObjectHashConfigBuild DefaultSerialization();

        /// <summary>
        /// Specifies custom serialization function.
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public IConfigureObjectHashConfigBuild CustomSerialization(Func<object, string> serializer);
    }
}