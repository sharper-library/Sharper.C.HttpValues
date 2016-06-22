using System;

namespace Sharper.C.Data.Http
{
    public struct Possible<A>
    {
        public enum MergeStrategy { Sum, First, Last, Replace };
        private readonly MergeStrategy strategy;
        public Maybe<A> ToMaybe { get; }

        internal Possible
          ( Maybe<A> a
          , MergeStrategy strategy = MergeStrategy.Sum
          )
        {   ToMaybe = a;
            this.strategy = strategy;
        }

        public Possible<A> Merge(Possible<A> p, Func<A, A, A> merge)
        {   var m = ToMaybe;
            return
                p.strategy == MergeStrategy.Sum
                ? Possible.Sum
                    ( ToMaybe.ZipWith(p.ToMaybe, merge)
                      .Or(() => m.Or(() => p.ToMaybe))
                    )

                : p.strategy == MergeStrategy.First
                ? Possible.First(m.Or(() => p.ToMaybe))

                : p.strategy == MergeStrategy.Last
                ? Possible.Last(p.ToMaybe.Or(() => m))

                : Possible.Replace(p.ToMaybe);
        }
    }

    public static class Possible
    {
        public static Possible<A> Sum<A>(Maybe<A> a)
        =>  new Possible<A>(a, Possible<A>.MergeStrategy.Sum);

        public static Possible<A> First<A>(Maybe<A> a)
        =>  new Possible<A>(a, Possible<A>.MergeStrategy.First);

        public static Possible<A> Last<A>(Maybe<A> a)
        =>  new Possible<A>(a, Possible<A>.MergeStrategy.Last);

        public static Possible<A> Replace<A>(Maybe<A> a)
        =>  new Possible<A>(a, Possible<A>.MergeStrategy.Replace);
    }
}
