using System.Text.Json;
namespace CargaCerta.Api;
public record Truck(int Id, string Placa, string Modelo, double CapacidadeKg);
public record TruckInput(string Placa, string Modelo, double CapacidadeKg);
public record Load(int Id, int CaminhaoId, string Descricao, double PesoKg);
public record LoadInput(int CaminhaoId, string Descricao, double PesoKg);
public record TruckSummary(int Id, string Placa, double CapacidadeKg, double PesoTotalKg, double DisponivelKg, bool Excedido);
public record Summary(List<TruckSummary> Caminhoes, double PesoTotalKg, int TotalCargas);
public class Store {
    private readonly object gate = new();
    // Mantemos os dados em um arquivo JSON para nao exigir banco de dados neste estudo.
    private readonly string path;
    private Data data;
    public Store(string path) { this.path = path; data = File.Exists(path) ? JsonSerializer.Deserialize<Data>(File.ReadAllText(path)) ?? new() : new(); }
    private void Save() => File.WriteAllText(path, JsonSerializer.Serialize(data));
    public List<Truck> Trucks() { lock(gate) return data.Caminhoes.ToList(); }
    public Truck? Truck(int id) { lock(gate) return data.Caminhoes.FirstOrDefault(x => x.Id == id); }
    public bool PlateExists(string plate, int except) { lock(gate) return data.Caminhoes.Any(x => x.Id != except && x.Placa.Equals(plate.Trim(), StringComparison.OrdinalIgnoreCase)); }
    public Truck AddTruck(TruckInput x) { lock(gate) { var t = new Truck(data.NextTruckId++, x.Placa.Trim().ToUpperInvariant(), x.Modelo.Trim(), x.CapacidadeKg); data.Caminhoes.Add(t); Save(); return t; } }
    public Truck? UpdateTruck(int id, TruckInput x) { lock(gate) { var i = data.Caminhoes.FindIndex(t => t.Id == id); if(i < 0) return null; var t = new Truck(id,x.Placa.Trim().ToUpperInvariant(),x.Modelo.Trim(),x.CapacidadeKg); data.Caminhoes[i]=t; Save(); return t; } }
    public bool DeleteTruck(int id) { lock(gate) { var changed = data.Caminhoes.RemoveAll(t => t.Id == id)>0; if(changed) { data.Cargas.RemoveAll(c => c.CaminhaoId == id); Save(); } return changed; } }
    public class Data { public List<Truck> Caminhoes {get;set;} = new(); public List<Load> Cargas {get;set;} = new(); public int NextTruckId {get;set;} = 1; public int NextLoadId {get;set;} = 1; }
}
