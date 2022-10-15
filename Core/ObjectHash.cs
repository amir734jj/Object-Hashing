using System;
using Core.Interfaces;
using Core.Logic;
using Core.Models;
using HashAlgorithm = Core.Models.HashAlgorithm;

namespace Core
{
    public abstract class ObjectHash<T>
    {
        private Func<object, string> _hashFunc;

        protected virtual void ConfigureObjectSha(IConfigureObjectHashConfig<T> config)
        {
            config
                .Algorithm(HashAlgorithm.Sha1)
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