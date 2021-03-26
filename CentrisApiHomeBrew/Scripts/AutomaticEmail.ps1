while(1)
{
   Invoke-RestMethod https://localhost:44361/CentrisPropertyAPI/AutomaticEmail?email=jeffreyd@live.fr
   start-sleep -seconds 60
}