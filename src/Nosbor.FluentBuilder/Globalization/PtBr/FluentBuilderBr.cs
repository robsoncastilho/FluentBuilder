using Nosbor.FluentBuilder.Lib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nosbor.FluentBuilder.Globalization.PtBr
{
    /// <summary>
    /// Wrapper exposing builder API in Brazilian Portuguese
    /// </summary>
    public sealed class FluentBuilderBr<T> where T : class
    {
        private readonly FluentBuilder<T> _builder;

        private FluentBuilderBr()
        {
            _builder = FluentBuilder<T>.New();
        }

        public static FluentBuilderBr<T> Novo()
        {
            return new FluentBuilderBr<T>();
        }

        public static implicit operator T(FluentBuilderBr<T> builder)
        {
            return builder.Criar();
        }

        public T Criar()
        {
            return _builder.Build();
        }

        public IEnumerable<T> EmUmaLista()
        {
            return _builder.AsList();
        }

        public FluentBuilderBr<T> Com<TPropriedade>(Expression<Func<T, TPropriedade>> expressao, TPropriedade valor)
        {
            _builder.With(expressao, valor);
            return this;
        }

        public FluentBuilderBr<T> ComDependencia<TServiceInterface, TServiceImplementation>(TServiceImplementation serviceImplementation)
            where TServiceImplementation : TServiceInterface
        {
            _builder.WithDependency<TServiceInterface, TServiceImplementation>(serviceImplementation);
            return this;
        }

        public FluentBuilderBr<T> AdicionandoEm<TCollectionProperty, TElement>(Expression<Func<T, TCollectionProperty>> expression, TElement newElement)
            where TCollectionProperty : IEnumerable<TElement>
        {
            _builder.AddingTo(expression, newElement);
            return this;
        }
    }
}