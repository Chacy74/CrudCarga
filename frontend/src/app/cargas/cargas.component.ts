import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../api.service';
import { Carga, CargaInput, Caminhao } from '../models';
@Component({selector:'app-cargas',imports:[FormsModule],template:`
<h2>Cargas</h2><p>Associe cada carga a um caminhão. Pesos em quilogramas.</p>
<form (ngSubmit)="salvar()"><label>Caminhão <select name="caminhao" [(ngModel)]="form.caminhaoId" required><option [ngValue]="0" disabled>Selecione</option>@for (c of caminhoes; track c.id) {<option [ngValue]="c.id">{{c.placa}}</option>}</select></label><label>Descrição <input name="descricao" [(ngModel)]="form.descricao" required></label><label>Peso (kg) <input name="peso" type="number" min="0.01" step="any" [(ngModel)]="form.pesoKg" required></label><button type="submit">{{editando === null ? 'Cadastrar' : 'Salvar alterações'}}</button><button type="button" (click)="limpar()">Limpar</button></form>
@if (erro) {<p class="erro" role="alert">{{erro}}</p>}
<ul>@for (c of lista; track c.id) {<li><strong>{{c.descricao}}</strong> · {{c.pesoKg}} kg · {{placa(c.caminhaoId)}} <button (click)="editar(c)">Editar</button> <button (click)="excluir(c)">Excluir</button></li>} @empty {<li>Nenhuma carga cadastrada.</li>}</ul>`})
export class CargasComponent implements OnInit {
 private api=inject(ApiService); lista:Carga[]=[]; caminhoes:Caminhao[]=[]; erro=''; editando:number|null=null;
 form:CargaInput={caminhaoId:0,descricao:'',pesoKg:1};
 ngOnInit(){this.carregar()}
 carregar(){this.api.caminhoes().subscribe({next:x=>this.caminhoes=x,error:()=>this.erro='API indisponível.'});this.api.cargas().subscribe({next:x=>this.lista=x,error:()=>this.erro='Não foi possível carregar cargas.'})}
 placa(id:number){return this.caminhoes.find(x=>x.id===id)?.placa ?? 'Caminhão não encontrado'}
 salvar(){this.erro='';const x={...this.form};const req=this.editando===null?this.api.criarCarga(x):this.api.editarCarga(this.editando,x);req.subscribe({next:()=>{this.limpar();this.carregar()},error:e=>this.erro=e.error?.erro ?? 'Erro ao salvar carga.'})}
 editar(c:Carga){this.editando=c.id;this.form={caminhaoId:c.caminhaoId,descricao:c.descricao,pesoKg:c.pesoKg}}
 excluir(c:Carga){if(!confirm('Excluir carga?'))return;this.api.excluirCarga(c.id).subscribe({next:()=>this.carregar(),error:()=>this.erro='Erro ao excluir.'})}
 limpar(){this.editando=null;this.form={caminhaoId:0,descricao:'',pesoKg:1}}
}
