using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SquadSync.Api.Rosters;

public sealed class AddPlayerRequestSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(AddPlayerRequest) || schema is not OpenApiSchema request) return;
        request.Description = "Supply exactly one non-null personId or newPerson. Reuses an unrostered Player membership; staff and duplicate roster entries conflict. No PlayerProfile is created.";
        if (request.Properties?["jerseyNumber"] is OpenApiSchema jersey)
            jersey.Description = "Nullable, otherwise 1-3 ASCII digits. Leading zeros are preserved.";
    }
}
