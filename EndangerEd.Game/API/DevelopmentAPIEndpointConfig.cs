namespace EndangerEd.Game.API;

public class DevelopmentAPIEndpointConfig : APIEndpointConfig
{
    public DevelopmentAPIEndpointConfig()
    {
        BaseUrl = "http://localhost:8001/api/";
    }
}
