import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Caminhao, CaminhaoInput, Carga, CargaInput, Resumo } from './models';
@Injectable({providedIn:'root'})
export class ApiService {
 private http=inject(HttpClient);
 private url='http://localhost:5080/api';
 caminhoes(){return this.http.get<Caminhao[]>(`${this.url}/caminhoes`)}
 criarCaminhao(x:CaminhaoInput){return this.http.post<Caminhao>(`${this.url}/caminhoes`,x)}
 editarCaminhao(id:number,x:CaminhaoInput){return this.http.put<Caminhao>(`${this.url}/caminhoes/${id}`,x)}
 excluirCaminhao(id:number){return this.http.delete<void>(`${this.url}/caminhoes/${id}`)}
 cargas(){return this.http.get<Carga[]>(`${this.url}/cargas`)}
 criarCarga(x:CargaInput){return this.http.post<Carga>(`${this.url}/cargas`,x)}
 editarCarga(id:number,x:CargaInput){return this.http.put<Carga>(`${this.url}/cargas/${id}`,x)}
 excluirCarga(id:number){return this.http.delete<void>(`${this.url}/cargas/${id}`)}
 resumo(){return this.http.get<Resumo>(`${this.url}/resumo`)}
}
