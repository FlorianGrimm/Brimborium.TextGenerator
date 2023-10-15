
namespace Brimborium.TextGenerator.WebApp.MinimalController;

public class TextRunController : IMinimalController
{
    public TextRunController()
    {
    }

    public void Map(WebApplication app)
    {
        var group=app.MapGroup("TextRun").WithOpenApi();
        group.MapGet("", () => {
            //return Results.Content("hallo", "text/plain", statusCode: 200);
            var result = new TGFileBuilder();
            result.ListOperation.Add(new TGOperation("Const", 0));
            result.ListRun.Add(new TGRun("Hello", 0));
            return result.Build();
        }).WithOpenApi();
    }
}
