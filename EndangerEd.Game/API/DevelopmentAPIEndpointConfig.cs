namespace EndangerEd.Game.API;

public class DevelopmentAPIEndpointConfig : APIEndpointConfig
{
    public DevelopmentAPIEndpointConfig()
    {
        APIBaseUrl = "http://localhost:8001/api/";
        KnowledgeBaseUrl = "http://localhost:8000/";
        GameUrl = "http://localhost:8001/";
    }
}
