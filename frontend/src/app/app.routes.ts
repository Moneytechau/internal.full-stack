import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/incident-report/incident-report.component').then(m => m.IncidentReportComponent),
  },
];
