using Sharper.C.Control;

namespace Sharper.C.Data.Http
{
    public delegate IO<RW, HttpResponse> Handler<I>(HttpRequest<I> r);

    public delegate Handler<I> Handler<A, I>(A args);
}
