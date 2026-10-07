using CargaCerta.Api;

var builder = WebApplication.CreateBuilder(args);

// O Angular roda em outra porta durante o desenvolvimento.
builder.Services.AddCors(opcoes =>
{
    opcoes.AddPolicy("frontend", politica =>
    {
        politica.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});
var arquivoDados = Path.Combine(builder.Environment.ContentRootPath, "dados.json");
builder.Services.AddSingleton(new Store(arquivoDados));
var app = builder.Build();
app.UseCors("frontend");

// Consultas: leem os dados sem alterar o cadastro.
app.MapGet("/api/caminhoes", (Store s) => s.Trucks());
app.MapGet("/api/caminhoes/{id:int}", (int id, Store s) => s.Truck(id) is { } t ? Results.Ok(t) : Results.NotFound());
// Comandos: criam, alteram ou excluem registros.
app.MapPost("/api/caminhoes", (TruckInput input, Store s) => {
    var error = ValidateTruck(input);
    if (error is not null)
    {
        return Results.BadRequest(new { erro = error });
    }
    if (s.PlateExists(input.Placa, 0))
    {
        return Results.Conflict(new { erro = "Placa já cadastrada." });
    }
    var truck = s.AddTruck(input);
    return Results.Created($"/api/caminhoes/{truck.Id}", truck);
});

app.MapPut("/api/caminhoes/{id:int}", (int id, TruckInput input, Store s) => {
    var error = ValidateTruck(input);
    if (error is not null)
    {
        return Results.BadRequest(new { erro = error });
    }
    if (s.PlateExists(input.Placa, id))
    {
        return Results.Conflict(new { erro = "Placa já cadastrada." });
    }
    return s.UpdateTruck(id, input) is { } truck ? Results.Ok(truck) : Results.NotFound();
});

app.MapDelete("/api/caminhoes/{id:int}", (int id, Store s) => s.DeleteTruck(id) ? Results.NoContent() : Results.NotFound());
app.Run();

static string? ValidateTruck(TruckInput caminhao)
{
    bool dadosInvalidos = string.IsNullOrWhiteSpace(caminhao.Placa)
        || string.IsNullOrWhiteSpace(caminhao.Modelo)
        || caminhao.CapacidadeKg <= 0
        || !double.IsFinite(caminhao.CapacidadeKg);

    if (dadosInvalidos)
    {
        return "Informe placa, modelo e capacidade positiva em kg.";
    }

    return null;
}
