using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();


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

public static class ProductRepository{
    public static List<Product> Products { get; set; } = new List<Product>();
    
    public static void Init(IConfiguration configuration) {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product){
        Products.Add(product);
    }

    public static Product GetBy(string code){
        return Products.FirstOrDefault( p=> p.Code == code);
    }

    public static void Remove(Product product){
        Products.Remove(product);
    }
}

public class Product {
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }   
}

public class ApplicationDbContext : DbContext {
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) 
        => options.UseSqlServer(
            "Server=localhost;Database=Products;User Id=sa;Password=@Sqlfrwk;MultipleActiveResultSets=True;Encrypt=YES;TrustServerCertificate=YES"
            );
}

