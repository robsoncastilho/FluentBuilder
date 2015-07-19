using en = Nosbor.FluentBuilder.Lib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        public FluentBuilder<T> ComValor<TMembro>(TMembro novoValor) where TMembro : class
        {
            _builder.WithValue(novoValor);
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
    }
}