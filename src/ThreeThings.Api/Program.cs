using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using ThreeThings.Utils.Extensions;
using ThreeThings.Utils.Filters;
using ThreeThings.Data;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelValidFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}).AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DocumentFilter<JsonPatchDocumentFilter>();
    options.IncludeAllXmlComments();
});

builder.Services.AddTransient(typeof(Lazy<>));
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
// builder.Services.AddJwtBearer(configuration);
builder.Services.AddCorsSetting(configuration);
builder.Services.AddDbContext<ThreeThingsDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(configuration.GetConnectionString("ThreeThings"));
});
builder.Services.AddBasicServiceByLifeScope();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseBasicException();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
