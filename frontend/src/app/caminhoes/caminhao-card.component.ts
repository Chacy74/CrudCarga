import { Component, input } from '@angular/core';
import { Caminhao } from '../models';
@Component({selector:'app-caminhao-card',template:`<strong>{{caminhao().placa}}</strong> · {{caminhao().modelo}} · capacidade {{caminhao().capacidadeKg}} kg`})
export class CaminhaoCardComponent { caminhao=input.required<Caminhao>(); }
