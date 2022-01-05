using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();

var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapGet("/products/{code}", ([FromRoute] string code) => {
	var product = ProductRepository.GetBy(code);
	if(product != null)
		return Results.Ok(product);
	return Results.NotFound();
});

app.MapPost("/products", (Product product) => {
	ProductRepository.Add(product);
	return Results.Created($"/products/{product.Code}", product.Code);
});

app.MapPut("/products", (Product product) => {
	var productSaved = ProductRepository.GetBy(product.Code);
	productSaved.Name = product.Name;
	return Results.Ok();
});

app.MapDelete("/products/{code}", ([FromRoute] string code) => {
	var productSaved = ProductRepository.GetBy(code);
	ProductRepository.Remove(productSaved);
	return Results.Ok();
});

app.Run();

