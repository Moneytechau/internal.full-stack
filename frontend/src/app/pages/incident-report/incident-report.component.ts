import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { INCIDENT_TYPES, IncidentReport } from '../../models/incident-report.model';

type Step = 1 | 2 | 3;

@Component({
  selector: 'app-incident-report',
  templateUrl: './incident-report.component.html',
  styleUrl: './incident-report.component.scss',
  imports: [ReactiveFormsModule],
})
export class IncidentReportComponent {
  protected readonly incidentTypes = INCIDENT_TYPES;
  protected readonly currentStep = signal<Step>(1);
  protected readonly submitted = signal(false);

  private fb = new FormBuilder();

  protected form = this.fb.group({
    details: this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      mobile: ['', [Validators.required, Validators.pattern(/^[0-9\s]{8,15}$/)]],
    }),
    incident: this.fb.group({
      incidentType: ['', Validators.required],
      estimatedDamage: [null as number | null, [Validators.required, Validators.min(0)]],
    }),
    specifics: this.fb.group({
      incidentDate: ['', Validators.required],
      location: ['', Validators.required],
      description: ['', [Validators.required, Validators.minLength(10)]],
    }),
  });

  protected get detailsGroup() {
    return this.form.controls.details;
  }

  protected get incidentGroup() {
    return this.form.controls.incident;
  }

  protected get specificsGroup() {
    return this.form.controls.specifics;
  }

  protected continue(): void {
    const group = this.currentStep() === 1 ? this.detailsGroup : this.incidentGroup;

    if (group.invalid) {
      group.markAllAsTouched();
      return;
    }

    this.currentStep.set((this.currentStep() + 1) as Step);
  }

  protected back(): void {
    this.currentStep.set((this.currentStep() - 1) as Step);
  }

  protected submit(): void {
    if (this.specificsGroup.invalid) {
      this.specificsGroup.markAllAsTouched();
      return;
    }

    const report: IncidentReport = {
      ...this.detailsGroup.getRawValue(),
      ...this.incidentGroup.getRawValue(),
      ...this.specificsGroup.getRawValue(),
    } as IncidentReport;

    // Backend submission isn't wired up yet — this is where the API call will go.
    console.log('Incident report ready to submit', report);

    this.submitted.set(true);
  }

  protected startNewReport(): void {
    this.form.reset();
    this.currentStep.set(1);
    this.submitted.set(false);
  }
}
