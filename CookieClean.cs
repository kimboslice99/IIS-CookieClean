using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CookieClean
{
    public class CookieClean : IHttpModule
    {
        #region IHttpModule implementation
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.PostReleaseRequestState += new EventHandler(Remove);
        }
        

        public void Remove(Object source, EventArgs e)
        {
            HttpApplication app = (HttpApplication)source;
            HttpRequest request = app.Context.Request;
            HttpResponse Response = app.Context.Response;
            // see if we have a set-cookie header to begin with
            if (Response.Headers.AllKeys.Contains("Set-Cookie"))
            {
#if DEBUG
                Debug.WriteLine("[CookieClean]: found set-cookie header, path=" + request.Url);
#endif
                HttpCookieCollection CookieCollection;
                HttpCookie Cookie;

                CookieCollection = Response.Cookies;

                // Capture all cookie names into a string array so we can iterate through them
                String[] cookie_array = CookieCollection.AllKeys;

                // our new dictionary that will contain only changed cookies
                Dictionary<string, string> new_cookies = new Dictionary<string, string>() { };

                // the loop
                for (int i = 0; i < cookie_array.Length; i++)
                {
                    Cookie = CookieCollection[cookie_array[i]];

                    // this will match [value];but;not;this;stuff
                    string pattern = @"(?<=^)[^;]+";
                    Match h = Regex.Match(Response.Cookies.Get(Cookie.Name).Value, pattern);
#if DEBUG
                    Debug.WriteLine("[CookieClean]: match " + Cookie.Value);
#endif
                    // if we have cookies && if cookie header contains value of new set-cookie header
                    if (request.Headers["Cookie"] != null && request.Headers["Cookie"].Contains(h.Value))
                    {
                        Debug.WriteLine("[CookieClean]: skipping " + Cookie.Name +  " " + h.Value);
                        continue;
                    }
                    // if we get this far were adding it to our new_cookie list since this cookies value has changed
                    new_cookies.Add(Cookie.Name, Cookie.Value);
#if DEBUG
                    Debug.WriteLine("[CookieClean]: adding to new cookies list name=" + Cookie.Name + " value=" + Cookie.Value);
#endif
                }
                // remove all cookies now that we have our new_cookies dictionary
#if DEBUG
                Debug.WriteLine("[CookieClean]: removing all set-cookie headers");
#endif
                Response.Headers.Remove("Set-Cookie");
                foreach (KeyValuePair<string, string> cookie in new_cookies)
                {
#if DEBUG
                    Debug.WriteLine("[CookieClean]: adding back " + cookie.Key + " " + cookie.Value);
#endif
                    Response.Headers.Add("Set-Cookie", cookie.Key + '=' + cookie.Value);
                }
            }
        }
        #endregion
    }
}
