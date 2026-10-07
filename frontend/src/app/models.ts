export interface Caminhao { id:number; placa:string; modelo:string; capacidadeKg:number; }
export type CaminhaoInput = Omit<Caminhao,'id'>;
export interface Carga { id:number; caminhaoId:number; descricao:string; pesoKg:number; }
export type CargaInput = Omit<Carga,'id'>;
export interface ResumoCaminhao { id:number; placa:string; capacidadeKg:number; pesoTotalKg:number; disponivelKg:number; excedido:boolean; }
export interface Resumo { caminhoes:ResumoCaminhao[]; pesoTotalKg:number; totalCargas:number; }
