using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using en = Nosbor.FluentBuilder.Lib;

namespace Nosbor.FluentBuilder.Br
{
    /// <summary>
    /// FluentBuilder - API traduzida para o português do Brasil
    /// </summary>
    public sealed class FluentBuilder<T> where T : class
    {
        private readonly en.FluentBuilder<T> _builder;

        private FluentBuilder()
        {
            _builder = en.FluentBuilder<T>.New();
        }

        public static FluentBuilder<T> Novo()
        {
            return new FluentBuilder<T>();
        }

        public static implicit operator T(FluentBuilder<T> builder)
        {
            return builder.Criar();
        }

        public static IEnumerable<T> Muitos(int quantidade)
        {
            return en.FluentBuilder<T>.Many(quantidade);
        }

        public T Criar()
        {
            return _builder.Build();
        }

        public IEnumerable<T> EmUmaLista()
        {
            return _builder.AsList();
        }

        public FluentBuilder<T> Com<TPropriedade>(Expression<Func<T, TPropriedade>> expressao, TPropriedade novoValor)
        {
            _builder.With(expressao, novoValor);
            return this;
        }

        public FluentBuilder<T> ComColecao<TCollectionProperty, TElement>(Expression<Func<T, TCollectionProperty>> expressao, params TElement[] novosElementos)
            where TCollectionProperty : IEnumerable<TElement>
        {
            _builder.WithCollection(expressao, novosElementos);
            return this;
        }

        public FluentBuilder<T> ComValorParaOCampo<TCampo>(TCampo novoValor) where TCampo : class
        {
            _builder.WithFieldValue(novoValor);
            return this;
        }

        public FluentBuilder<T> ComDependencia<TInterface, TImplementacao>(TImplementacao implementacao)
            where TImplementacao : TInterface
        {
            _builder.WithDependency<TInterface, TImplementacao>(implementacao);
            return this;
        }

        public FluentBuilder<T> AdicionandoEm<TColecao, TElemento>(Expression<Func<T, TColecao>> expressao, TElemento novoElemento)
            where TColecao : IEnumerable<TElemento>
        {
            _builder.AddingTo(expressao, novoElemento);
            return this;
        }

        public FluentBuilder<T> ComColecaoVazia<TColecao>(Expression<Func<T, TColecao>> expressao)
            where TColecao : IEnumerable
        {
            _builder.WithEmptyCollection(expressao);
            return this;
        }
    }
}