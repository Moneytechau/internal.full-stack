import { ComponentFixture, TestBed } from '@angular/core/testing';
import { IncidentReportComponent } from './incident-report.component';
import { IncidentReportService } from '../../services/incident-report.service';
import { IncidentReportResponse } from '../../models/incident-report.model';

describe('IncidentReportComponent', () => {
  let component: IncidentReportComponent;
  let fixture: ComponentFixture<IncidentReportComponent>;
  let serviceSpy: jasmine.SpyObj<IncidentReportService>;

  const reportId = '11111111-1111-1111-1111-111111111111';

  function baseReport(): IncidentReportResponse {
    return {
      id: reportId,
      fullName: 'Jane Doe',
      mobile: '0412345678',
      incidentType: null,
      estimatedDamage: null,
      incidentDate: null,
      location: null,
      description: null,
    };
  }

  function fillDetailsStep(): void {
    component['detailsGroup'].setValue({ fullName: 'Jane Doe', mobile: '0412345678' });
  }

  function fillIncidentStep(): void {
    component['incidentGroup'].setValue({ incidentType: 'Theft', estimatedDamage: 1500 });
  }

  function fillSpecificsStep(): void {
    component['specificsGroup'].setValue({
      incidentDate: '2026-01-15',
      location: 'Sydney',
      description: 'Vehicle collision at intersection.',
    });
  }

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj<IncidentReportService>('IncidentReportService', [
      'start',
      'updateReporterDetails',
      'updateIncident',
      'updateDetails',
    ]);

    await TestBed.configureTestingModule({
      imports: [IncidentReportComponent],
      providers: [{ provide: IncidentReportService, useValue: serviceSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(IncidentReportComponent);
    component = fixture.componentInstance;
  });

  it('starts on step 1 with nothing submitted', () => {
    expect(component['currentStep']()).toBe(1);
    expect(component['submitted']()).toBeFalse();
  });

  it('continue() on an invalid step marks controls touched and does not call the service', async () => {
    await component['continue']();

    expect(serviceSpy.start).not.toHaveBeenCalled();
    expect(component['detailsGroup'].touched).toBeTrue();
    expect(component['currentStep']()).toBe(1);
  });

  it('continue() on step 1 starts a new report and advances to step 2', async () => {
    serviceSpy.start.and.resolveTo(baseReport());
    fillDetailsStep();

    await component['continue']();

    expect(serviceSpy.start).toHaveBeenCalledOnceWith({ fullName: 'Jane Doe', mobile: '0412345678' });
    expect(component['currentStep']()).toBe(2);
  });

  it('going back to step 1 and continuing again updates the existing report instead of creating a new one', async () => {
    serviceSpy.start.and.resolveTo(baseReport());
    serviceSpy.updateReporterDetails.and.resolveTo(baseReport());
    fillDetailsStep();

    await component['continue'](); // step 1 -> 2, creates the report
    component['back'](); // step 2 -> 1
    await component['continue'](); // step 1 -> 2 again, should update not create

    expect(serviceSpy.start).toHaveBeenCalledTimes(1);
    expect(serviceSpy.updateReporterDetails).toHaveBeenCalledOnceWith(reportId, {
      fullName: 'Jane Doe',
      mobile: '0412345678',
    });
    expect(component['currentStep']()).toBe(2);
  });

  it('continue() on step 2 updates the incident and advances to step 3', async () => {
    serviceSpy.start.and.resolveTo(baseReport());
    serviceSpy.updateIncident.and.resolveTo(baseReport());
    fillDetailsStep();
    await component['continue']();
    fillIncidentStep();

    await component['continue']();

    expect(serviceSpy.updateIncident).toHaveBeenCalledOnceWith(reportId, {
      incidentType: 'Theft',
      estimatedDamage: 1500,
    });
    expect(component['currentStep']()).toBe(3);
  });

  it('submit() on step 3 saves details and marks the report submitted', async () => {
    serviceSpy.start.and.resolveTo(baseReport());
    serviceSpy.updateIncident.and.resolveTo(baseReport());
    serviceSpy.updateDetails.and.resolveTo(baseReport());
    fillDetailsStep();
    await component['continue']();
    fillIncidentStep();
    await component['continue']();
    fillSpecificsStep();

    await component['submit']();

    expect(serviceSpy.updateDetails).toHaveBeenCalledOnceWith(reportId, {
      incidentDate: '2026-01-15',
      location: 'Sydney',
      description: 'Vehicle collision at intersection.',
    });
    expect(component['submitted']()).toBeTrue();
  });

  it('shows an error and stays on the current step when saving fails', async () => {
    serviceSpy.start.and.rejectWith(new Error('network error'));
    fillDetailsStep();

    await component['continue']();

    expect(component['errorMessage']()).toBe('Something went wrong saving your report. Please try again.');
    expect(component['saving']()).toBeFalse();
    expect(component['currentStep']()).toBe(1);
  });

  it('startNewReport() resets the form, step and submitted state', async () => {
    serviceSpy.start.and.resolveTo(baseReport());
    fillDetailsStep();
    await component['continue']();

    component['startNewReport']();

    expect(component['currentStep']()).toBe(1);
    expect(component['submitted']()).toBeFalse();
    expect(component['form'].pristine).toBeTrue();
  });
});
