namespace ASP_09._Swagger_documentation.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseSwaggerUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice Manager API v1");
            options.RoutePrefix = string.Empty;
        });

        return app;
    }
}