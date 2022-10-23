﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/FluentBuilder version 0.7.1.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable
using System;
using System.Collections.Generic;

namespace JustinWritesCode.MSBuild.Sdks.MSBuild.UsingsSdk.FluentBuilder
{
    public partial class IDictionaryBuilder<TKey, TValue> : Builder<IDictionary<TKey, TValue>> where TKey : notnull
    {
        private readonly Lazy<IDictionary<TKey, TValue>> _dictionary = new Lazy<IDictionary<TKey, TValue>>(() => new Dictionary<TKey, TValue>());

        public virtual IDictionaryBuilder<TKey, TValue> Add(TKey key, TValue value) => Add(() => new KeyValuePair<TKey, TValue>(key, value));
        public virtual IDictionaryBuilder<TKey, TValue> Add(Func<KeyValuePair<TKey, TValue>> func)
        {
            var result = func();
            _dictionary.Value.Add(result.Key, result.Value);

            return this;
        }

        public override IDictionary<TKey, TValue> Build(bool useObjectInitializer = true)
        {
            if (Object?.IsValueCreated != true)
            {
                Object = new Lazy<IDictionary<TKey, TValue>>(() =>
                {
                    return _dictionary.Value;
                });
            }

            PostBuild(Object.Value);

            return Object.Value;
        }
    }
}
#nullable disable