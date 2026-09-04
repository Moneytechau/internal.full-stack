import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../environments/environment';
import { IncidentReportService } from './incident-report.service';
import { IncidentReportResponse } from '../models/incident-report.model';

describe('IncidentReportService', () => {
  let service: IncidentReportService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiBaseUrl}/api/incidentreports`;

  const sampleReport: IncidentReportResponse = {
    id: '11111111-1111-1111-1111-111111111111',
    fullName: 'Jane Doe',
    mobile: '0412345678',
    incidentType: null,
    estimatedDamage: null,
    incidentDate: null,
    location: null,
    description: null,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(IncidentReportService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('start() posts to the base incidentreports endpoint', async () => {
    const promise = service.start({ fullName: 'Jane Doe', mobile: '0412345678' });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ fullName: 'Jane Doe', mobile: '0412345678' });
    req.flush(sampleReport);

    await expectAsync(promise).toBeResolvedTo(sampleReport);
  });

  it('updateReporterDetails() puts to the report id endpoint', async () => {
    const promise = service.updateReporterDetails(sampleReport.id, {
      fullName: 'Jane Smith',
      mobile: '0498765432',
    });

    const req = httpMock.expectOne(`${baseUrl}/${sampleReport.id}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ fullName: 'Jane Smith', mobile: '0498765432' });
    req.flush({ ...sampleReport, fullName: 'Jane Smith', mobile: '0498765432' });

    await promise;
  });

  it('updateIncident() puts to the incident sub-resource endpoint', async () => {
    const promise = service.updateIncident(sampleReport.id, {
      incidentType: 'Theft',
      estimatedDamage: 1500,
    });

    const req = httpMock.expectOne(`${baseUrl}/${sampleReport.id}/incident`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ incidentType: 'Theft', estimatedDamage: 1500 });
    req.flush({ ...sampleReport, incidentType: 'Theft', estimatedDamage: 1500 });

    await promise;
  });

  it('updateDetails() puts to the details sub-resource endpoint', async () => {
    const promise = service.updateDetails(sampleReport.id, {
      incidentDate: '2026-01-15',
      location: 'Sydney',
      description: 'Vehicle collision at intersection.',
    });

    const req = httpMock.expectOne(`${baseUrl}/${sampleReport.id}/details`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({
      incidentDate: '2026-01-15',
      location: 'Sydney',
      description: 'Vehicle collision at intersection.',
    });
    req.flush({
      ...sampleReport,
      incidentDate: '2026-01-15',
      location: 'Sydney',
      description: 'Vehicle collision at intersection.',
    });

    await promise;
  });
});
