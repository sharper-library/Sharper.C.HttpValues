using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Sharper.C.Data.Http
{
    using System;
    using ByteString = IEnumerable<ImmutableArray<byte>>;
    using CookieMap = ImmutableDictionary<string, Cookie>;

    public struct Cookie
    {
        public static Cookie Mk
          ( string value
          , string domain = null
          , DateTimeOffset? expiry = default(DateTimeOffset?)
          , bool httpOnly = true
          , string path = null
          , bool secure = false
          )
        =>  new Cookie
              ( value
              , Maybe.FromReference(domain)
              , Maybe.FromNullable(expiry)
              , httpOnly
              , Maybe.FromReference(path)
              , secure
              );

        private Cookie
          ( string value
          , Maybe<string> domain
          , Maybe<DateTimeOffset> expiry
          , bool httpOnly
          , Maybe<string> path
          , bool secure
          )
        {
            Value = value;
            Domain = domain;
            Expiry = expiry;
            HttpOnly = httpOnly;
            Path = path;
            Secure = secure;
        }

        public string Value { get; }
        public Maybe<string> Domain { get; }
        public Maybe<DateTimeOffset> Expiry { get; }
        public bool HttpOnly { get; }
        public Maybe<string> Path { get; }
        public bool Secure { get; }
    }

    public struct InvString
      : IEquatable<InvString>
    {
        private readonly string value;

        public string String
        =>  value ?? "";

        public static InvString Mk(string s)
        =>  new InvString(s.ToLowerInvariant());

        private InvString(string s)
        {   value = s;
        }

        public bool Equals(InvString other)
        =>  String.Equals(other.String);

        public override bool Equals(object obj)
        =>  obj is InvString && Equals((InvString) obj);

        public override int GetHashCode()
        =>  String.GetHashCode();

        public override string ToString()
        =>  String;

        public static bool operator==(InvString x, InvString y)
        =>  x.Equals(y);

        public static bool operator!=(InvString x, InvString y)
        =>  !x.Equals(y);
    }

    public struct HttpResponse
      : HasHttpResponse
    {
        public Possible<ByteString> Body { get; }
        public Possible<long> ContentLength { get; }
        public Possible<string> ContentType { get; }
        public Possible<MultiMap<InvString, string>> Headers { get; }
        public Possible<int> StatusCode { get; }
        public Possible<CookieMap> Cookies { get; }

        HttpResponse HasHttpResponse.HttpResponse
        => this;

        internal HttpResponse
          ( Possible<ByteString> body
          , Possible<long> length
          , Possible<string> type
          , Possible<MultiMap<InvString, string>> headers
          , Possible<int> status
          , Possible<CookieMap> cookies
          )
        {   Body = body;
            ContentLength = length;
            ContentType = type;
            Headers = headers;
            StatusCode = status;
            Cookies = cookies;
        }

        public HttpResponse Merge(HttpResponse r)
        =>  new HttpResponse
              ( Body.Merge(r.Body, (x, y) => x.Concat(y))
              , ContentLength.Merge(r.ContentLength, (x, y) => x + y)
              , ContentType.Merge(r.ContentType, (_, y) => y)
              , Headers.Merge(r.Headers, (x, y) => x.Merge(y))
              , StatusCode.Merge(r.StatusCode, (_, y) => y)
              , Cookies.Merge(r.Cookies, (x, y) => x.AddRange(y))
              );

        public static HttpResponse operator+(HttpResponse x, HttpResponse y)
        =>  x.Merge(y);

        public static HttpResponse Mk
          ( Possible<ByteString> body
          , Possible<long> contentLength
          , Possible<string> contentType
          , Possible<MultiMap<InvString, string>> headers
          , Possible<int> statusCode
          , Possible<CookieMap> cookies
          )
        => new HttpResponse
             ( body
             , contentLength
             , contentType
             , headers
             , statusCode
             , cookies
             );

        public static HttpResponse MkSum
          ( ByteString body = null
          , long? contentLength = default(long?)
          , string contentType = null
          , MultiMap<InvString, string>? headers = null
          , int? statusCode = default(int?)
          , CookieMap cookies = null
          )
        =>  Mk
              ( Possible.Sum(Maybe.FromReference(body))
              , Possible.Sum(Maybe.FromNullable(contentLength))
              , Possible.Sum(Maybe.FromReference(contentType))
              , Possible.Sum(Maybe.FromNullable(headers))
              , Possible.Sum(Maybe.FromNullable(statusCode))
              , Possible.Sum(Maybe.FromReference(cookies))
              );

        public static HttpResponse MkReplace
          ( ByteString body = null
          , long? contentLength = default(long?)
          , string contentType = null
          , MultiMap<InvString, string>? headers = null
          , int? statusCode = default(int?)
          , CookieMap cookies = null
          )
        =>  Mk
              ( Possible.Replace(Maybe.FromReference(body))
              , Possible.Replace(Maybe.When(contentLength.HasValue, () => contentLength.Value))
              , Possible.Replace(Maybe.FromReference(contentType))
              , Possible.Replace(Maybe.FromNullable(headers))
              , Possible.Replace(Maybe.When(statusCode.HasValue, () => statusCode.Value))
              , Possible.Replace(Maybe.FromReference(cookies))
              );

        public static HttpResponse MkEmpty()
        =>  default(HttpResponse);
    }
}
