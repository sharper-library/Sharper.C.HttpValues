using System.Collections.Immutable;

namespace Sharper.C.Data.Http
{
    using System;
    using System.Threading.Tasks;
    using StringMap = ImmutableDictionary<string, string>;

    public struct HttpRequest<A>
    {
        internal HttpRequest
          ( InvString method
          , string path
          , A body
          , Maybe<long> contentLength
          , Maybe<string> contentType
          , StringMap cookies
          , MultiMap<InvString, string> headers
          , MultiMap<InvString, string> query
          )
        {   Method = method;
            Path = path;
            Body = body;
            ContentLength = contentLength;
            ContentType = contentType;
            Cookies = cookies;
            Headers = headers;
            Query = query;
        }

        public HttpRequest<A> Update
          ( Func<InvString, InvString> method = null
          , Func<string, string> path = null
          , Func<A, A> body = null
          , Func<Maybe<long>, Maybe<long>> contentLength = null
          , Func<Maybe<string>, Maybe<string>> contentType = null
          , Func<StringMap, StringMap> cookies = null
          , Func<MultiMap<InvString, string>, MultiMap<InvString, string>> headers = null
          , Func<MultiMap<InvString, string>, MultiMap<InvString, string>> query = null
          )
        =>  new HttpRequest<A>
              ( method.Update(Method)
              , path.Update(Path)
              , body.Update(Body)
              , contentLength.Update(ContentLength)
              , contentType.Update(ContentType)
              , cookies.Update(Cookies)
              , headers.Update(Headers)
              , query.Update(Query)
              );

        public HttpRequest<B> UpdateBody<B>(Func<A, B> f)
        =>  new HttpRequest<B>
              ( Method
              , Path
              , f(Body)
              , ContentLength
              , ContentType
              , Cookies
              , Headers
              , Query
              );

        public async Task<HttpRequest<B>>
        UpdateBodyAsync<B>(Func<A, Task<B>> f)
        {   var body = await f(Body);
            return
                new HttpRequest<B>
                  ( Method
                  , Path
                  , body
                  , ContentLength
                  , ContentType
                  , Cookies
                  , Headers
                  , Query
                  );
        }

        public InvString Method { get; }
        public string Path { get; }
        public A Body { get; }
        public Maybe<long> ContentLength { get; }
        public Maybe<string> ContentType { get; }
        public StringMap Cookies { get; }
        public MultiMap<InvString, string> Headers { get; }
        public MultiMap<InvString, string> Query { get; }
    }

    public static class HttpRequest
    {
        public static HttpRequest<A> Mk<A>
          ( InvString method
          , string path
          , A body
          , Maybe<long> contentLength
          , Maybe<string> contentType
          , StringMap cookies
          , MultiMap<InvString, string> headers
          , MultiMap<InvString, string> query
          )
        =>  new HttpRequest<A>
              ( method
              , path
              , body
              , contentLength
              , contentType
              , cookies
              , headers
              , query
              );

        internal static A Update<A>(this Func<A, A> f, A a)
        =>  f == null ? a : f(a);
    }
}