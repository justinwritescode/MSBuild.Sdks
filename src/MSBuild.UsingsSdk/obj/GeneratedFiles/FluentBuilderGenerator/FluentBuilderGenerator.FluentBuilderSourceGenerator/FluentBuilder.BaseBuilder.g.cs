﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/FluentBuilder version 0.7.1.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

namespace JustinWritesCode.MSBuild.Sdks.MSBuild.UsingsSdk.FluentBuilder
{
    public abstract class Builder<T>
    {
        protected Lazy<T> Object;

        protected Type InstanceType = typeof(T);

        public abstract T Build(bool useObjectInitializer = true);

        public Builder<T> WithObject(T value) => WithObject(() => value);

        public Builder<T> WithObject(Func<T> func)
        {
            Object = new Lazy<T>(func);
            return this;
        }
    
        protected virtual void PostBuild(T value) {}
    }
}