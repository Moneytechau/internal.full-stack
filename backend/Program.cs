using Serilog;

var builder = WebApplication.CreateBuilder(args);

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEnd", policy =>
        policy.WithOrigins(corsOrigins).AllowAnyMethod().AllowAnyHeader());
});

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseCors("FrontEnd");
app.UseAuthorization();
app.MapControllers();

app.Run();
