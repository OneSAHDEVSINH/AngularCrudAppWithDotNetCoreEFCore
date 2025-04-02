using AngularApp2.Server.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressConsumesConstraintForFormFileParameters = true;
    options.SuppressInferBindingSourcesForParameters = true;
    options.SuppressModelStateInvalidFilter = true;
    options.SuppressMapClientErrors = true;
    options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
        "https://httpstatuses.com/404";
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//builder.Services.AddSpaStaticFiles(configuration =>
//{
//    configuration.RootPath = "ClientApp/dist";
//});
var app = builder.Build();

app.
    UseDefaultFiles();
app.MapStaticAssets();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAllOrigins");
// Configure the HTTP request pipeline.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
//if (!app.Environment.IsDevelopment())
//{
//    app.UseSpaStaticFiles();
//}
//app.UseSpa(spa =>
//{
//    spa.Options.SourcePath = "ClientApp";
//    if (app.Environment.IsDevelopment())
//    {
//        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
//    }
//});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();