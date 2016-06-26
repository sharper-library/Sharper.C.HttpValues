using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using H = Microsoft.AspNetCore.Http;

namespace Sharper.C.Data.Http.AspNetCore
{
    using CookieMap = ImmutableDictionary<string, Cookie>;
    using Control;
    public static class CompatModule
    {
        public static HttpRequest<I> ReadRequest<I>
          ( H.HttpRequest r
          , Func<Stream, I> readBody
          )
        =>  HttpRequest.Mk
              ( method: InvString.Mk(r.Method)
              , path: r.Path
              , body: readBody(r.Body)
              , contentLength: Maybe.When(r.ContentLength.HasValue, () => r.ContentLength.Value)
              , contentType: Maybe.When(r.ContentType != null, r.ContentType)
              , cookies: r.Cookies.ToImmutableDictionary()
              , headers: MultiMap.Mk(r.Headers.ToImmutableDictionary(x => InvString.Mk(x.Key), x => x.Value.ToImmutableList()))
              , query: MultiMap.Mk(r.Query.ToImmutableDictionary(x => InvString.Mk(x.Key), x => x.Value.ToImmutableList()))
              , unsafeRawSource: r
              );

        public static IO<WO, Unit> WriteResponse
          ( HttpResponse r
          , H.HttpResponse hr
          )
        =>  IO<WO>.Mk
              ( async tok =>
                {   hr.StatusCode = r.StatusCode.ToMaybe.ValueOr(200);
                    await hr.Headers.Populate(r.Headers.ToMaybe).Awaitable(tok);
                    await hr.Cookies.Populate(r.Cookies.ToMaybe).Awaitable(tok);
                    hr.ContentType = r.ContentType.ToMaybe.ValueOr((string)null);
                    hr.ContentLength = r.ContentLength.ToMaybe.Map(x => new long?(x)).ValueOr(default(long?));
                    foreach (var x in r.Body.ToMaybe.ValueOr(() => new ImmutableArray<byte>[] {}))
                    {   await hr.Body.WriteAsync(x.ToArray(), 0, x.Length);
                    }
                }
              );

        private static IO<WO, Unit> Populate
          ( this IHeaderDictionary headers
          , Maybe<MultiMap<InvString, string>> map
          )
        =>  map.WhenJust
              ( xs =>
                    IO<WO>.Defer
                      ( () =>
                        {   foreach (var x in xs)
                            {   headers.Append
                                  ( x.Fst.String
                                  , new StringValues(x.Snd.ToArray())
                                  );
                            }
                        }
                      )
              );

        private static IO<WO, Unit> Populate
          ( this IResponseCookies cookies
          , Maybe<CookieMap> map
          )
        =>  map.WhenJust
              ( xs =>
                  IO<WO>.Defer
                    ( () =>
                      {   foreach (var x in xs)
                          {   cookies
                              .Append(x.Key, x.Value.Value, x.Value.Options());
                          }
                      }
                    )
              );

        private static CookieOptions Options(this Cookie c)
        =>  new CookieOptions
            { Domain = c.Domain.ValueOr((string)null)
            , HttpOnly = c.HttpOnly
            , Expires =
                c
                .Expiry
                .Map(o => new DateTimeOffset?(o))
                .ValueOr(default(DateTimeOffset?))
            , Path = c.Path.ValueOr((string)null)
            , Secure = c.Secure
            };
    }
}
