using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Sharper.C.Data.Http
{
    public static class MultiMap
    {
        public static MultiMap<A, B> Empty<A, B>()
          where A : IEquatable<A>
        =>  new MultiMap<A, B>
              ( ImmutableDictionary.Create<A, ImmutableList<B>>()
              );

        public static MultiMap<A, B> Mk<A, B>
          ( ImmutableDictionary<A, ImmutableList<B>> map
          )
          where A : IEquatable<A>
        =>  new MultiMap<A, B>(map);

        public static MultiMap<A, B> Mk<A, B>
          ( IDictionary<A, IEnumerable<B>> map
          )
          where A : IEquatable<A>
        =>  Mk
              ( map
                .Select
                  ( x =>
                        new KeyValuePair<A, ImmutableList<B>>
                          ( x.Key
                          , x.Value.ToImmutableList()
                          )
                  )
                .ToImmutableDictionary()
              );
    }

    public struct MultiMap<A, B>
      : IEnumerable<And<A, ImmutableList<B>>>
      where A : IEquatable<A>
    {
        private readonly
        ImmutableDictionary<A, ImmutableList<B>>
        map;

        internal MultiMap
          ( ImmutableDictionary<A, ImmutableList<B>> map
          )
        {   this.map = map;
        }

        public ImmutableList<B> AllValues(A key)
        =>  map.MaybeGet(key).ValueOr(ImmutableList.Create<B>());

        public Maybe<ImmutableList<B>> AllValuesNonEmpty(A key)
        =>  map.MaybeGet(key);

        public bool ContainsKey(A key)
        =>  map.ContainsKey(key);

        public Maybe<B> FirstValue(A key)
        =>  map.MaybeGet(key).FlatMap(xs => xs.MaybeFirst());

        public MultiMap<A, B> Add(A a, IEnumerable<B> bs)
        =>  new MultiMap<A, B>(map.Add(a, bs.ToImmutableList()));

        public MultiMap<A, B> Add
          ( params And<A, B[]>[] pairs
          )
        =>  AddRange(pairs.Select(p => p.MapSnd(x => x as IEnumerable<B>)));

        public MultiMap<A, B> AddRange
          (IEnumerable<And<A, IEnumerable<B>>> pairs
          )
        =>  new MultiMap<A, B>
              ( map.AddRange
                  ( pairs.Select
                      ( p =>
                            new KeyValuePair<A, ImmutableList<B>>
                              ( p.Fst
                              , p.Snd.ToImmutableList()
                              )
                      )
                  )
              );

        public MultiMap<A, B> Remove(A a)
        =>  new MultiMap<A, B>(map.Remove(a));

        public MultiMap<A, B> Merge(MultiMap<A, B> overwrite)
        =>  new MultiMap<A, B>(map.AddRange(overwrite.map));

        public IEnumerator<And<A, ImmutableList<B>>> GetEnumerator()
        =>  map.Select(p => And.Mk(p.Key, p.Value)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        =>  GetEnumerator();

        public IEnumerable<A> Keys
        =>  map.Keys;
    }
}
