import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../api.service';
import { Caminhao, CaminhaoInput } from '../models';
import { CaminhaoCardComponent } from './caminhao-card.component';
@Component({selector:'app-caminhoes',imports:[FormsModule,CaminhaoCardComponent],template:`
<h2>Caminhões</h2><p>Cadastre e gerencie a frota.</p>
<form (ngSubmit)="salvar()"><label>Placa <input name="placa" [(ngModel)]="form.placa" required></label><label>Modelo <input name="modelo" [(ngModel)]="form.modelo" required></label><label>Capacidade (kg) <input name="capacidade" type="number" min="0.01" step="any" [(ngModel)]="form.capacidadeKg" required></label><button type="submit">{{editando === null ? 'Cadastrar' : 'Salvar alterações'}}</button><button type="button" (click)="limpar()">Limpar</button></form>
@if (erro) {<p class="erro" role="alert">{{erro}}</p>}
<ul>@for (c of lista; track c.id) {<li><app-caminhao-card [caminhao]="c" /> <button (click)="editar(c)">Editar</button> <button (click)="excluir(c)">Excluir</button></li>} @empty {<li>Nenhum caminhão cadastrado.</li>}</ul>`})
export class CaminhoesComponent implements OnInit {
 private api=inject(ApiService); lista:Caminhao[]=[]; erro=''; editando:number|null=null;
 form:CaminhaoInput={placa:'',modelo:'',capacidadeKg:1000};
 ngOnInit(){this.carregar()}
 carregar(){this.api.caminhoes().subscribe({next:x=>this.lista=x,error:()=>this.erro='Não foi possível carregar caminhões. Confira a API.'})}
 salvar(){this.erro=''; const x={...this.form}; const req=this.editando===null?this.api.criarCaminhao(x):this.api.editarCaminhao(this.editando,x);req.subscribe({next:()=>{this.limpar();this.carregar()},error:e=>this.erro=e.error?.erro ?? 'Erro ao salvar caminhão.'})}
 editar(c:Caminhao){this.editando=c.id;this.form={placa:c.placa,modelo:c.modelo,capacidadeKg:c.capacidadeKg}}
 excluir(c:Caminhao){if(!confirm('Excluir caminhão e todas as cargas vinculadas?'))return;this.api.excluirCaminhao(c.id).subscribe({next:()=>this.carregar(),error:()=>this.erro='Erro ao excluir.'})}
 limpar(){this.editando=null;this.form={placa:'',modelo:'',capacidadeKg:1000}}
}
