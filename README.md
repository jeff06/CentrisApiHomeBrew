# CentrisApiHomeBrew

This C# api poke centris to retrive a listing of property base on a json body.
The return is based on the html pages. It support multiple return pages.
Ive put in the Doc file exemple to retrive information.

I also made a Postman collection to help you.

If you would like to see a specific payload, you need to intercept the query.
Any help is welcome

Please do not overload the centris website with multiple request. Use common sense.

## How to generate the payload
On the centris website

- There is an image at the end with the steps
- Input all of your search criteria *BUT DO NOT PRESS SEARCH*
- Press F12, go into the Sources tab and on the left expend -> top -> www.centris.ca -> js -> and click on property.js
- Pretty-print inside the browser
- On line 1481 place a break point. We need to see the content of the "t" variable.
  - if the centris code change and the line number is not valid, just search for "UseGeographyShapes" and get the returned value at the end
- Copie de content of the variable
  - When the code stop on the break point, in the console type JSON.stringify(t);
- The payload must be built this way -> 
{
  "query": {
  *INSERT THE VARIABLE PAYLOAD*
  },
  "isHomePage": true
}

- You've made the payload. Gratz you can now poke the centris api and get the search resault. Have fun.

![image](https://user-images.githubusercontent.com/21128028/112655594-f083c100-8e26-11eb-9886-0ed87a1cce0c.png)
