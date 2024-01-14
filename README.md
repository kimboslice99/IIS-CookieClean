# IIS-CookieClean
This module was created to solve an issue with a backend server (looking at you mod_session_dbd) sending duplicate set-cookie headers as well as resending set-cookie headers on every single subresource request

What it does is acts as a filter that will strip any set-cookie headers from a request which already contains a cookie containing the same value

So, if a client makes a request with `cookie: test=value` and the backend tries to send a header containing `set-cookie: test=value` then this module will drop it

However if the backend responds with `set-cookie: test=newvalue` then it will allow the header to be sent

## Install
- build with Visual Studio 2019
- drop in bin folder
