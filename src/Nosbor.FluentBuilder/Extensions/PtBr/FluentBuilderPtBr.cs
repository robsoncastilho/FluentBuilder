using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nosbor.FluentBuilder.Extensions.PtBr
{
    public static class FluentBuilderPtBr
    {
        public static FluentBuilder<T> Com<T, TPropriedade>(this FluentBuilder<T> builder, Expression<Func<T, TPropriedade>> expressao, TPropriedade valor)
            where T : class
        {
            return builder.With(expressao, valor);
        }

        public static FluentBuilder<T> ComDependencia<T, TServiceInterface, TServiceImplementation>(this FluentBuilder<T> builder, TServiceImplementation serviceImplementation)
            where T : class
            where TServiceImplementation : TServiceInterface
        {
            return builder.WithDependency<TServiceInterface, TServiceImplementation>(serviceImplementation);
        }

        public static FluentBuilder<T> AdicionandoEm<T, TCollectionProperty, TElement>(this FluentBuilder<T> builder, Expression<Func<T, TCollectionProperty>> expression, TElement newElement)
            where T : class
            where TCollectionProperty : IEnumerable<TElement>
        {
            return builder.AddingTo<TCollectionProperty, TElement>(expression, newElement);
        }

        public static T Criar<T>(this FluentBuilder<T> builder) where T : class
        {
            return builder.Build();
        }
    }
}