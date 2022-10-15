using System;
using ObjectHashing.Interfaces;
using ObjectHashing.Logic;
using ObjectHashing.Models;
using HashAlgorithm = ObjectHashing.Models.HashAlgorithm;

namespace ObjectHashing
{
    public abstract class ObjectHash<T>
    {
        private Func<object, string> _hashFunc;

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
            if (_hashFunc != null)
            {
                return _hashFunc(this);
            }

            var config = new ConfigureObjectShaConfig<T>();
            ConfigureObjectSha(config);

            var typedHashFunc = HashingUtility.BuildRecipe(config);
            _hashFunc = untypedSource =>
            {
                if (untypedSource is T typedSource)
                {
                    return typedHashFunc(typedSource);
                }

                return null;
            };

            return _hashFunc(this);
        }
    }
}