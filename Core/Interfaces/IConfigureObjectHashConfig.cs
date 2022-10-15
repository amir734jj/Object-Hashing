using Core.Models;

namespace Core.Interfaces
{
    public interface IConfigureObjectHashConfig<TSource>
    {
        /// <summary>
        /// Specifies which hashing algorithm to use.
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public IConfigureObjectHashConfigProperty<TSource> Algorithm(HashAlgorithm algorithm);
    }
}