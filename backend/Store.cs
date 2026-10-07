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
    public List<Load> Loads() { lock(gate) return data.Cargas.ToList(); }
    public Load? Load(int id) { lock(gate) return data.Cargas.FirstOrDefault(x => x.Id == id); }
    public Load AddLoad(LoadInput x) { lock(gate) { var c = new Load(data.NextLoadId++,x.CaminhaoId,x.Descricao.Trim(),x.PesoKg); data.Cargas.Add(c); Save(); return c; } }
    public Load? UpdateLoad(int id, LoadInput x) { lock(gate) { var i=data.Cargas.FindIndex(c => c.Id==id); if(i<0) return null; var c=new Load(id,x.CaminhaoId,x.Descricao.Trim(),x.PesoKg); data.Cargas[i]=c; Save(); return c; } }
    public bool DeleteLoad(int id) { lock(gate) { var changed=data.Cargas.RemoveAll(c=>c.Id==id)>0; if(changed) Save(); return changed; } }
    public Summary Summary()
    {
        lock (gate)
        {
            var caminhoes = new List<TruckSummary>();

            foreach (var caminhao in data.Caminhoes)
            {
                double pesoTotal = data.Cargas
                    .Where(carga => carga.CaminhaoId == caminhao.Id)
                    .Sum(carga => carga.PesoKg);

                double disponivel = caminhao.CapacidadeKg - pesoTotal;
                bool excedido = pesoTotal > caminhao.CapacidadeKg;

                caminhoes.Add(new TruckSummary(
                    caminhao.Id, caminhao.Placa, caminhao.CapacidadeKg,
                    pesoTotal, disponivel, excedido));
            }

            double pesoGeral = data.Cargas.Sum(carga => carga.PesoKg);
            return new Summary(caminhoes, pesoGeral, data.Cargas.Count);
        }
    }
    public class Data { public List<Truck> Caminhoes {get;set;} = new(); public List<Load> Cargas {get;set;} = new(); public int NextTruckId {get;set;} = 1; public int NextLoadId {get;set;} = 1; }
}
