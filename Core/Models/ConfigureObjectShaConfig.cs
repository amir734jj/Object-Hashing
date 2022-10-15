using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using InfoViaLinq;
using InfoViaLinq.Interfaces;
using Newtonsoft.Json;
using ObjectHashing.Interfaces;

namespace ObjectHashing.Models
{
    internal class ConfigureObjectShaConfig<TSource> :
        IConfigureObjectHashConfig<TSource>,
        IConfigureObjectHashConfigProperty<TSource>,
        IConfigureObjectHashConfigBuild
    {
        public HashAlgorithm HashAlgorithm;

        public readonly HashSet<PropertyInfo> PropertyInfos = new HashSet<PropertyInfo>();

        public Func<object, string> Serializer = JsonConvert.SerializeObject;

        private readonly IInfoViaLinq<TSource> _infoViaLinq = new InfoViaLinq<TSource>();

        public IConfigureObjectHashConfigProperty<TSource> Algorithm(HashAlgorithm algorithm)
        {
            HashAlgorithm = algorithm;

            return this;
        }

        public IConfigureObjectHashConfigProperty<TSource> DefaultAlgorithm()
        {
            HashAlgorithm = HashAlgorithm.Sha1;

            return this;
        }

        public IConfigureObjectHashConfigProperty<TSource> Property(Expression<Func<TSource, object>> property)
        {
            PropertyInfos.Add(_infoViaLinq.PropLambda(property).Members().FirstOrDefault());

            return this;
        }

        public IConfigureObjectHashConfigSerialization<TSource> AllProperties()
        {
            PropertyInfos.UnionWith(
                typeof(TSource).GetProperties(BindingFlags.Instance |
                                              BindingFlags.Public |
                                              BindingFlags.GetField));
            return this;
        }

        public void Build()
        {
            // Do nothing
        }

        public IConfigureObjectHashConfigBuild DefaultSerialization()
        {
            return this;
        }

        public IConfigureObjectHashConfigBuild CustomSerialization(Func<object, string> serializer)
        {
            Serializer = serializer;

            return this;
        }
    }
}