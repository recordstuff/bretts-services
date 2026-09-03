using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace bretts_services.Utilities;

public class SwaggerFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        switch (context.MethodInfo.Name)
        {
            case "Login":
                if (operation.RequestBody?.Content?.TryGetValue(
                        "application/json",
                        out var content) == true)
                {
                    content.Example = new JsonObject
                    {
                        ["email"] = "adminanduser@brettdrake.org",
                        ["password"] = "test123"
                    };
                }
                break;
        }
    }
}