# CentrisApiHomeBrew

This C# api poke centris to retrive a listing of property base on a json body.
The return is based on the html pages. It support multiple return pages.
Ive put in the Doc file exemple to retrive information.

I also made a Postman collection to help you.

If you would like to see a specific payload, you need to intercept the query.
Any help is welcome

Please do not overload the centris website with multiple request. Use common sense.

## How to generate the payload
On the centris website :
1) Input all of your search criteria *BUT DO NOT PRESS SEARCH*
2) Press F12, go into the Sources tab and on the left expend top -> www.centris.ca -> js -> and click on property.js
3) Pretty-print inside the browser
4) and on line 1481 place a break point. We need to see the content of the "t" variable.
  i) if the centris code change and the line number is not valid, just search for "UseGeographyShapes" and get the returned value at the end
5) copie de content of the variable
6) The payload must be built this way -> 
{
  "query": {
  *INSERT THE VARIABLE PAYLOAD*
  },
  "isHomePage": true
}

7) You've made the payload. Gratz you can now poke the centris api and get the search resault. Have fun.
