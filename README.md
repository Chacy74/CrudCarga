# CargaCerta | v3 final
Projeto didático: Angular 22 (standalone organizado em feature folders) + ASP.NET Core .NET 10.

## Pré-requisitos
Node.js compatível com Angular 22, npm e SDK .NET 10. Necessária conexão para instalar pacotes npm.

## Executar
Em um terminal: `cd backend` e `dotnet run --urls http://localhost:5080`.
Em outro terminal: `cd frontend`, `npm install`, `npm start`.
Abra http://localhost:4200 no navegador.

## Funcionalidades
CRUD de caminhões (placa, modelo, capacidade kg), CRUD de cargas (descrição, peso kg, caminhão), resumo de peso total/disponível/excedido por caminhão. Exclusão de caminhão também exclui suas cargas. Dados persistidos em `backend/dados.json` (ignorado pelo Git). Não use para operação logística real sem validação, autenticação, testes e regras legais de peso por eixo.

## Rotas API
GET/POST `/api/caminhoes`; GET/PUT/DELETE `/api/caminhoes/{id}`; GET/POST `/api/cargas`; GET/PUT/DELETE `/api/cargas/{id}`; GET `/api/resumo`.
Exemplo POST caminhão: `{"placa":"ABC1D23","modelo":"Truck","capacidadeKg":12000}`.
Exemplo POST carga: `{"caminhaoId":1,"descricao":"Grãos","pesoKg":5000}`.

## Conceitos para estudar
`CaminhoesComponent` implementa `ngOnInit`, `ApiService` consome a API, `CaminhaoCardComponent` recebe dados do pai via `[caminhao]`. Pastas `caminhoes`, `cargas`, `resumo` são features com rotas lazy-loaded; Angular recomenda standalone para código novo, então não usamos NgModule clássico.

## Histórico sugerido
v1: API e CRUD de caminhões; v2: CRUD de cargas e peso; v3: interface Angular. Cada pasta é um snapshot independente: publique como commits/tags v1, v2, v3, sem publicar `dados.json`.

## Arquitetura e autoria
Esta versão foi gerada com auxílio de IA e revisada para ficar mais didática. Não usa CQRS: a classe `Store` concentra leituras e escritas sobre o mesmo arquivo JSON. Os comentários e a formatação foram ajustados para facilitar o estudo; não são prova de autoria humana.
