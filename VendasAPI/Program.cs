using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VendasAPI.Data;
using VendasAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<VendaModel>> GetVendas(AppDbContext context)
{
    return await context.Vendas.ToListAsync();
}


app.MapGet("/Vendas", async (AppDbContext context) =>
{
    return await GetVendas(context);
});


app.MapGet("/Vendas/{id}", async (AppDbContext context, int id) =>
{
    var venda = await context.Vendas.FindAsync(id);

    if(venda == null)
    {
        return Results.NotFound("Venda não localizada!"); 
    }

    return Results.Ok(venda);

});



app.MapPost("/Venda", async (AppDbContext context, VendaModel venda) =>
{
    context.Vendas.Add(venda);
    await context.SaveChangesAsync();

    return await GetVendas(context);
});


app.MapPut("/Venda", async (AppDbContext context, VendaModel venda) =>
{
    var vendaDb = await context.Vendas.AsNoTracking().FirstOrDefaultAsync(vendaDb=> vendaDb.Id == venda.Id);

    if (vendaDb == null) return Results.NotFound("Venda não localizada!");

    vendaDb.Nome = venda.Nome;
    vendaDb.Descricao = venda.Descricao;

    context.Update(venda);
    await context.SaveChangesAsync();

    return Results.Ok(await GetVendas(context));

});


app.MapDelete("/Venda/{id}", async (AppDbContext contex, int id) =>
{
    var vendaDb = await contex.Vendas.FindAsync(id);

    if (vendaDb == null) return Results.NotFound("Venda não localizada!");

    contex.Vendas.Remove(vendaDb);
    await contex.SaveChangesAsync();

    return Results.Ok(await GetVendas(contex));


});



app.Run();

