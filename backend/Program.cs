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

// Rotas de cargas.
app.MapGet("/api/cargas", (Store s) => s.Loads());
app.MapGet("/api/cargas/{id:int}", (int id, Store s) => s.Load(id) is { } c ? Results.Ok(c) : Results.NotFound());
app.MapPost("/api/cargas", (LoadInput input, Store s) => {
    var error = ValidateLoad(input, s);
    if (error is not null)
    {
        return Results.BadRequest(new { erro = error });
    }
    var load = s.AddLoad(input);
    return Results.Created($"/api/cargas/{load.Id}", load);
});

app.MapPut("/api/cargas/{id:int}", (int id, LoadInput input, Store s) => {
    var error = ValidateLoad(input, s);
    if (error is not null)
    {
        return Results.BadRequest(new { erro = error });
    }
    return s.UpdateLoad(id, input) is { } load ? Results.Ok(load) : Results.NotFound();
});

app.MapDelete("/api/cargas/{id:int}", (int id, Store s) => s.DeleteLoad(id) ? Results.NoContent() : Results.NotFound());

// O resumo calcula o peso total de cada caminhao.
app.MapGet("/api/resumo", (Store s) => s.Summary());
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
static string? ValidateLoad(LoadInput carga, Store dados)
{
    if (string.IsNullOrWhiteSpace(carga.Descricao)
        || carga.PesoKg <= 0
        || !double.IsFinite(carga.PesoKg))
    {
        return "Informe descrição e peso positivo em kg.";
    }

    if (dados.Truck(carga.CaminhaoId) is null)
    {
        return "Caminhão inexistente.";
    }

    return null;
}
