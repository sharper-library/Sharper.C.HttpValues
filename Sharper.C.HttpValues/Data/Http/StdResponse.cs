using System.Collections.Immutable;
using static System.Text.Encoding;

namespace Sharper.C.Data.Http
{
    public static class StdResponse
    {
        public static HttpResponse StatusCode(int code)
        =>  HttpResponse.MkReplace(statusCode: code);

        public static HttpResponse Redirect(int code, string uri)
        =>  HttpResponse.MkReplace
              ( statusCode: code
              , headers:
                    MultiMap
                    .Empty<InvString, string>()
                    .Add
                      ( And.Mk(InvString.Mk("Location"), new[] { uri })
                      )
              );

        public static HttpResponse Five00(string message)
        =>  HttpResponse.MkReplace
              ( statusCode: 500
              , contentType: "text/plain"
              , body: new[] { UTF8.GetBytes(message).ToImmutableArray() }
              );

        public static HttpResponse ShortText(string s)
        =>  HttpResponse.MkReplace
              ( statusCode: 200
              , contentType: "text/plain"
              , body: new[] { UTF8.GetBytes(s).ToImmutableArray() }
              );
    }
}
