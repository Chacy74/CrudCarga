import { Component, OnInit, inject } from '@angular/core';
import { ApiService } from '../api.service';
import { Resumo } from '../models';
@Component({selector:'app-resumo',template:`<h2>Peso por caminhão</h2><p>Visão consolidada das cargas cadastradas.</p>
@if (erro) {<p class="erro" role="alert">{{erro}}</p>}
@if (dados; as r) {<p><strong>{{r.totalCargas}}</strong> cargas · <strong>{{r.pesoTotalKg}}</strong> kg no total</p><ul>@for(c of r.caminhoes; track c.id){<li><strong>{{c.placa}}</strong> · {{c.pesoTotalKg}} / {{c.capacidadeKg}} kg · disponível {{c.disponivelKg}} kg @if(c.excedido){<strong class="erro">CAPACIDADE EXCEDIDA</strong>}</li>} @empty {<li>Cadastre um caminhão para começar.</li>}</ul>}`})
export class ResumoComponent implements OnInit {private api=inject(ApiService);dados:Resumo|null=null;erro='';ngOnInit(){this.api.resumo().subscribe({next:x=>this.dados=x,error:()=>this.erro='Não foi possível carregar o resumo. Confira a API.'})}}
