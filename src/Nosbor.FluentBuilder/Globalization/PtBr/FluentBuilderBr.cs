using Nosbor.FluentBuilder.Lib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nosbor.FluentBuilder.Globalization.PtBr
{
    /// <summary>
    /// FluentBuilder - API traduzida para o português do Brasil
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

        public FluentBuilderBr<T> Com<TPropriedade>(Expression<Func<T, TPropriedade>> expressao, TPropriedade novoValor)
        {
            _builder.With(expressao, novoValor);
            return this;
        }

        public FluentBuilderBr<T> ComDependencia<TInterface, TImplementacao>(TImplementacao implementacao)
            where TImplementacao : TInterface
        {
            _builder.WithDependency<TInterface, TImplementacao>(implementacao);
            return this;
        }

        public FluentBuilderBr<T> AdicionandoEm<TColecao, TElemento>(Expression<Func<T, TColecao>> expressao, TElemento novoElemento)
            where TColecao : IEnumerable<TElemento>
        {
            _builder.AddingTo(expressao, novoElemento);
            return this;
        }
    }
}