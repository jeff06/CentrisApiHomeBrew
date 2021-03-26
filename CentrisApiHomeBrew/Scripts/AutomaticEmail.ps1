while(1)
{
   Invoke-RestMethod https://localhost:44361/CentrisPropertyAPI/AutomaticEmail
   start-sleep -seconds 60
}