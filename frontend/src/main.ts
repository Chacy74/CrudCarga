import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter, RouterLink, RouterOutlet, Routes } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
const routes: Routes = [
 { path: '', redirectTo: 'resumo', pathMatch: 'full' },
 { path: 'caminhoes', loadComponent: () => import('./app/caminhoes/caminhoes.component').then(m => m.CaminhoesComponent) },
 { path: 'cargas', loadComponent: () => import('./app/cargas/cargas.component').then(m => m.CargasComponent) },
 { path: 'resumo', loadComponent: () => import('./app/resumo/resumo.component').then(m => m.ResumoComponent) },
 { path: '**', redirectTo: 'resumo' }
];
@Component({selector:'app-root', imports:[RouterLink,RouterOutlet],template:`<header><h1>CargaCerta</h1><nav><a routerLink="/resumo">Resumo</a><a routerLink="/caminhoes">Caminhões</a><a routerLink="/cargas">Cargas</a></nav></header><main><router-outlet /></main>`})
class AppComponent {}
bootstrapApplication(AppComponent,{providers:[provideRouter(routes),provideHttpClient()]}).catch(console.error);
