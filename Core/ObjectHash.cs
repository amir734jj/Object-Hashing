using System;
using ObjectHashing.Interfaces;
using ObjectHashing.Logic;
using ObjectHashing.Models;

namespace ObjectHashing
{
    public abstract class ObjectHash<T>
    {
        private readonly Lazy<Func<object, string>> _hashFunc;

        protected ObjectHash()
        {
            _hashFunc = new Lazy<Func<object, string>>(() =>
            {
                var config = new ConfigureObjectShaConfig<T>();
                ConfigureObjectSha(config);

                var typedHashFunc = HashingUtility.BuildRecipe(config);
                return untypedSource =>
                {
                    if (untypedSource is T typedSource)
                    {
                        return typedHashFunc(typedSource);
                    }

                    return null;
                };
            });
        }

        protected virtual void ConfigureObjectSha(IConfigureObjectHashConfig<T> config)
        {
            config
                .DefaultAlgorithm()
                .AllProperties()
                .DefaultSerialization()
                .Build();
        }
        
        public string GenerateHash()
        {
            return _hashFunc.Value(this);
        }
    }
}