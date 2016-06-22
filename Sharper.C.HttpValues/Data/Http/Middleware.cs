namespace Sharper.C.Data.Http
{
    public delegate Handler<B, J>
    Middleware<B, A, J, I>(Handler<A, I> ab);

    public delegate Handler<J>
    Middleware<J, I>(Handler<I> h);

    public static class Middleware
    {
        public static Middleware<C, A, K, I> Then<C, B, A, K, J, I>
          ( this Middleware<C, B, K, J> m2
          , Middleware<B, A, J, I> m1
          )
        =>  h => m2(m1(h));

        public static Middleware<K, I> Then<K, J, I>
          ( this Middleware<K, J> m2
          , Middleware<J, I> m1
          )
        =>  h => m2(m1(h));
    }
}
