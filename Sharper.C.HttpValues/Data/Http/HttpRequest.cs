using System;
using System.Collections.Immutable;

namespace Sharper.C.Data.Http
{
    using StringMap = ImmutableDictionary<string, string>;

    public struct HttpRequest<A>
    {
        internal HttpRequest
          ( InvString scheme
          , Maybe<int> port
          , Maybe<string> queryString
          , InvString method
          , string path
          , A body
          , Maybe<long> contentLength
          , Maybe<string> contentType
          , StringMap cookies
          , MultiMap<InvString, string> headers
          , MultiMap<InvString, string> query
          , object unsafeRawSource
          )
        {   Scheme = scheme;
            Port = port;
            QueryString = queryString;
            Method = method;
            Path = path;
            Body = body;
            ContentLength = contentLength;
            ContentType = contentType;
            Cookies = cookies;
            Headers = headers;
            Query = query;
            UnsafeRawSource = unsafeRawSource;
        }

        public HttpRequest<A> Update
          ( Func<InvString, InvString> scheme = null
          , Func<Maybe<int>, Maybe<int>> port = null
          , Func<Maybe<string>, Maybe<string>> queryString = null
          , Func<InvString, InvString> method = null
          , Func<string, string> path = null
          , Func<A, A> body = null
          , Func<Maybe<long>, Maybe<long>> contentLength = null
          , Func<Maybe<string>, Maybe<string>> contentType = null
          , Func<StringMap, StringMap> cookies = null
          , Func<MultiMap<InvString, string>, MultiMap<InvString, string>> headers = null
          , Func<MultiMap<InvString, string>, MultiMap<InvString, string>> query = null
          )
        =>  new HttpRequest<A>
              ( scheme.Update(Scheme)
              , port.Update(Port)
              , queryString.Update(QueryString)
              , method.Update(Method)
              , path.Update(Path)
              , body.Update(Body)
              , contentLength.Update(ContentLength)
              , contentType.Update(ContentType)
              , cookies.Update(Cookies)
              , headers.Update(Headers)
              , query.Update(Query)
              , UnsafeRawSource
              );

        public HttpRequest<B> UpdateBody<B>(Func<A, B> f)
        =>  new HttpRequest<B>
              ( Scheme
              , Port
              , QueryString
              , Method
              , Path
              , f(Body)
              , ContentLength
              , ContentType
              , Cookies
              , Headers
              , Query
              , UnsafeRawSource
              );

        public InvString Scheme { get; }
        public Maybe<int> Port { get; }
        public Maybe<string> QueryString { get; }
        public InvString Method { get; }
        public string Path { get; }
        public A Body { get; }
        public Maybe<long> ContentLength { get; }
        public Maybe<string> ContentType { get; }
        public StringMap Cookies { get; }
        public MultiMap<InvString, string> Headers { get; }
        public MultiMap<InvString, string> Query { get; }
        public object UnsafeRawSource { get; }

        public string PortString
        =>  Port.Cata(() => "", p => $":{p}");
    }

    public static class HttpRequest
    {
        public static HttpRequest<A> Mk<A>
          ( InvString scheme
          , Maybe<int> port
          , Maybe<string> queryString
          , InvString method
          , string path
          , A body
          , Maybe<long> contentLength
          , Maybe<string> contentType
          , StringMap cookies
          , MultiMap<InvString, string> headers
          , MultiMap<InvString, string> query
          , object unsafeRawSource = null
          )
        =>  new HttpRequest<A>
              ( scheme
              , port
              , queryString
              , method
              , path
              , body
              , contentLength
              , contentType
              , cookies
              , headers
              , query
              , unsafeRawSource
              );

        internal static A Update<A>(this Func<A, A> f, A a)
        =>  f == null ? a : f(a);
    }
}
