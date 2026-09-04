import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  IncidentReportResponse,
  StartIncidentReportRequest,
  UpdateIncidentDetailsRequest,
  UpdateIncidentRequest,
} from '../models/incident-report.model';

@Injectable({ providedIn: 'root' })
export class IncidentReportService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiBaseUrl}/api/incidentreports`;

  public async start(request: StartIncidentReportRequest): Promise<IncidentReportResponse> {
    return await firstValueFrom(this.http.post<IncidentReportResponse>(this.baseUrl, request));
  }

  public async updateIncident(id: string, request: UpdateIncidentRequest): Promise<IncidentReportResponse> {
    return await firstValueFrom(this.http.put<IncidentReportResponse>(`${this.baseUrl}/${id}/incident`, request));
  }

  public async updateDetails(id: string, request: UpdateIncidentDetailsRequest): Promise<IncidentReportResponse> {
    return await firstValueFrom(this.http.put<IncidentReportResponse>(`${this.baseUrl}/${id}/details`, request));
  }
}
