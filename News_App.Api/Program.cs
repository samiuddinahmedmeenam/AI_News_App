using News_App;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowReactFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

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

app.MapPost("/api/ask", async (AskRequest request) =>
{
    DatabaseService.InitializeDatabase();

    if (string.IsNullOrWhiteSpace(request.Question))
    {
        return Results.BadRequest(new
        {
            message = "Question cannot be empty."
        });
    }

    // 1. Create embedding for user's question
    List<float> questionEmbedding = await EmbeddingService.GetEmbedding(request.Question);

    // 2. Load chunks and saved chunk embeddings
    List<ArticleChunk> allChunks = DatabaseService.GetAllChunks();
    Dictionary<int, List<float>> chunkEmbeddings = DatabaseService.GetAllChunkEmbeddings();

    if (allChunks.Count == 0 || chunkEmbeddings.Count == 0)
    {
        return Results.Ok(new AskResponse(
            "No chunks or embeddings were found in the database.",
            new List<EvidenceDto>()
        ));
    }

    // 3. Semantic retrieval
    List<ArticleChunk> relevantChunks =
        RetrievalService.RetrieveRelevantChunksSemantic(
            questionEmbedding,
            allChunks,
            chunkEmbeddings,
            topK: 5
        );

    // 4. Build context
    string context = RetrievalService.BuildContextFromChunks(relevantChunks);

    // 5. Ask AI using retrieved context
    string answer = await AiSummaryServices.AnswerWithContext(request.Question, context);

    // 6. Return answer + evidence
    List<EvidenceDto> evidence = relevantChunks.Select(chunk =>
        new EvidenceDto(
            chunk.ChunkText,
            chunk.ArticleUrl,
            chunk.ChunkIndex
        )
    ).ToList();

    return Results.Ok(new AskResponse(answer, evidence));
});

app.Run();


record AskRequest(string Question);

record EvidenceDto(
    string ChunkText,
    string ArticleUrl,
    int ChunkIndex
);

record AskResponse(
    string Answer,
    List<EvidenceDto> Evidence
);


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


