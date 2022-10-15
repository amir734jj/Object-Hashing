using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using Core.Models;
using Newtonsoft.Json;
using HashAlgorithm = Core.Models.HashAlgorithm;

namespace Core.Logic
{
    internal static class HashingUtility
    {
        private static readonly MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();
        private static readonly SHA1Managed Sha1 = new SHA1Managed();
        private static readonly SHA256Managed Sha256 = new SHA256Managed();
        private static readonly SHA384Managed Sha384 = new SHA384Managed();
        private static readonly SHA512Managed Sha512 = new SHA512Managed();
        private static readonly UTF8Encoding Utf8 = new UTF8Encoding();
        private static byte[] _arrBytes;
        private static readonly ModuleBuilder DynamicModule;
        private static int _counter;

        static HashingUtility()
        {
            var dynamicAssemblyName = new AssemblyName("ObjectHashingTempAssembly");
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            var dynamicModule = dynamicAssembly.DefineDynamicModule("TypeProjectionTempAssembly");
            DynamicModule = dynamicModule;
        }

        /// <summary>
        /// Encrypts the informed string using a specific algorithm
        /// </summary>
        /// <param name="str">The string that will be encrypted</param>
        /// <param name="algorithm">The algorithm that will be used</param>
        /// <returns>The encrypted string</returns>
        private static string GetHashFromString(string str, HashAlgorithm algorithm)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                _arrBytes = algorithm switch
                {
                    HashAlgorithm.Md5 => Md5.ComputeHash(Utf8.GetBytes(str)),
                    HashAlgorithm.Sha1 => Sha1.ComputeHash(Utf8.GetBytes(str)),
                    HashAlgorithm.Sha256 => Sha256.ComputeHash(Utf8.GetBytes(str)),
                    HashAlgorithm.Sha384 => Sha384.ComputeHash(Utf8.GetBytes(str)),
                    HashAlgorithm.Sha512 => Sha512.ComputeHash(Utf8.GetBytes(str)),
                    _ => Md5.ComputeHash(Utf8.GetBytes(str))
                };

                foreach (var x in _arrBytes)
                {
                    stringBuilder.Append(x.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while hashing: " + e.Message);
                return string.Empty;
            }
        }

        public static Func<T, string> BuildRecipe<T>(ConfigureObjectShaConfig<T> config)
        {
            var sourceExpr = Expression.Parameter(typeof(T));

            var dynamicAnonymousType =
                DynamicModule.DefineType($"{typeof(T).Name}_ObjectHashing_Generated{_counter++}", TypeAttributes.Public);
            var bindings = new List<MemberBinding>();

            foreach (var propertyInfo in config.PropertyInfos)
            {
                var fieldBldr = dynamicAnonymousType.DefineField(
                    propertyInfo.Name[..1].ToLower() + propertyInfo.Name[1..],
                    propertyInfo.PropertyType,
                    FieldAttributes.Private);

                // The last argument of DefineProperty is null, because the
                // property has no parameters. (If you don't specify null, you must
                // specify an array of Type objects. For a parameterless property,
                // use an array with no elements: new Type[] {})
                var propertyBldr = dynamicAnonymousType.DefineProperty(propertyInfo.Name,
                    PropertyAttributes.HasDefault,
                    propertyInfo.PropertyType,
                    null);

                // The property set and property get methods require a special
                // set of attributes.
                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName |
                                                    MethodAttributes.HideBySig;

                // Define the "get" accessor method for Property.
                var getPropMthdBldr =
                    dynamicAnonymousType.DefineMethod($"get_{propertyInfo.Name}",
                        getSetAttr,
                        propertyInfo.PropertyType,
                        Type.EmptyTypes);

                var propertyGetIl = getPropMthdBldr.GetILGenerator();

                propertyGetIl.Emit(OpCodes.Ldarg_0);
                propertyGetIl.Emit(OpCodes.Ldfld, fieldBldr);
                propertyGetIl.Emit(OpCodes.Ret);

                // Define the "set" accessor method for Property.
                var custNameSetPropMthdBldr =
                    dynamicAnonymousType.DefineMethod($"set_{propertyInfo.Name}",
                        getSetAttr,
                        null,
                        new[] { propertyInfo.PropertyType });

                var propertySetIl = custNameSetPropMthdBldr.GetILGenerator();

                propertySetIl.Emit(OpCodes.Ldarg_0);
                propertySetIl.Emit(OpCodes.Ldarg_1);
                propertySetIl.Emit(OpCodes.Stfld, fieldBldr);
                propertySetIl.Emit(OpCodes.Ret);

                // Last, we must map the two methods created above to our PropertyBuilder to
                // their corresponding behaviors, "get" and "set" respectively.
                propertyBldr.SetGetMethod(getPropMthdBldr);
                propertyBldr.SetSetMethod(custNameSetPropMthdBldr);
            }

            var projectionType = dynamicAnonymousType.CreateType();
            var creationExpr = Expression.New(projectionType.GetConstructor(Type.EmptyTypes)!);
            var bodyExpr = Expression.MemberInit(creationExpr, bindings);
            var projectExpr = Expression.Lambda<Func<T, object>>(bodyExpr, sourceExpr);
            var projectFunc = projectExpr.Compile();

            bindings.AddRange(config.PropertyInfos.Select(propertyInfo =>
                Expression.Bind(projectionType.GetProperty(propertyInfo.Name)!,
                    Expression.PropertyOrField(sourceExpr, propertyInfo.Name))));

            return source => GetHashFromString(config.Serializer(projectFunc(source)), config.HashAlgorithm);
        }
    }
}