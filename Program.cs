var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add CORS rules
builder.Services.AddCors((options) => 
{
    options.AddPolicy("DevCors", (corsBuilder) =>
    {
        corsBuilder
        .WithOrigins("http://localhost:4200", "ttp://localhost:3000", "ttp://localhost:8000")
        .AllowAnyHeader()
        .AllowCredentials();
    });

        options.AddPolicy("ProdCors", (corsBuilder) =>
    {
        corsBuilder
        .WithOrigins("https://production.com")
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCors");
}
else if (app.Environment.IsProduction())
{
    app.UseCors("ProdCors");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
