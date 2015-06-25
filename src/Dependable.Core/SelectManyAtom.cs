using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dependable.Core
{
    public class SelectManyAtom<TFirst, TSecond, TOut> : Atom<TOut>
    {
        readonly Func<TFirst, Atom<TSecond>> _selector;
        readonly Func<TFirst, TSecond, TOut> _projector;

        public Atom<TFirst> Source { get; }

        public Expression<Func<TFirst, Atom<TSecond>>> Selector { get; }

        public Expression<Func<TFirst, TSecond, TOut>> Projector { get; }

        public SelectManyAtom(Atom<TFirst> source, 
            Expression<Func<TFirst, Atom<TSecond>>> selector, 
            Expression<Func<TFirst, TSecond, TOut>> projector)
        {
            Source = source;            
            Selector = selector;
            Projector = projector;

            _selector = selector.Compile();
            _projector = projector.Compile();
        }
        
        public override async Task<TOut> Charge(object input = null)
        {
            var first = await Source.Charge(input);
            var second = await _selector(first).Charge();
            return _projector(first, second);
        }
    }

    public static partial class Atom
    {
        public static Atom<TOut> SelectMany<TFirst, TSecond, TOut>(
            this Atom<TFirst> first,
            Expression<Func<TFirst, Atom<TSecond>>> selector,
            Expression<Func<TFirst, TSecond, TOut>> projector
            )
        {
            return new SelectManyAtom<TFirst, TSecond, TOut>(first, selector, projector);
        }
    }
}