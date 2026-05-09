using News_App;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");



app.MapGet("/api/test", () =>
{
    return Results.Ok(new { message = "API is working" });
});

app.MapGet("/api/news", () =>
{
    DatabaseService.InitializeDatabase();

    string dbPath = Path.GetFullPath("news.db");
    Console.WriteLine($"API DB Path: {dbPath}");

    if (!DatabaseService.HasArticles())
    {
        return Results.Ok(new
        {
            message = "No articles found in the database.",
            databasePath = dbPath,
            articles = new List<Article>()
        });
    }

    List<Article> articles = DatabaseService.GetAllArticles();

    return Results.Ok(new
    {
        message = "Articles loaded successfully.",
        databasePath = dbPath,
        count = articles.Count,
        articles = articles
    });
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


