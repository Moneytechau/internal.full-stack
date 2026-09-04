import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { INCIDENT_TYPES } from '../../models/incident-report.model';
import { IncidentReportService } from '../../services/incident-report.service';

type Step = 1 | 2 | 3;

function notInFuture(control: AbstractControl): ValidationErrors | null {
  if (!control.value) {
    return null;
  }

  const today = new Date().toISOString().slice(0, 10);
  return control.value > today ? { futureDate: true } : null;
}

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
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly maxDate = new Date().toISOString().slice(0, 10);

  private fb = new FormBuilder();
  private incidentReportService = inject(IncidentReportService);
  private reportId: string | null = null;

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
      incidentDate: ['', [Validators.required, notInFuture]],
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

  protected async continue(): Promise<void> {
    if (this.currentStep() === 1) {
      await this.saveStep(this.detailsGroup, async () => {
        const { fullName, mobile } = this.detailsGroup.getRawValue();
        const report = await this.incidentReportService.start({ fullName: fullName!, mobile: mobile! });
        this.reportId = report.id;
      });
      return;
    }

    await this.saveStep(this.incidentGroup, async () => {
      const { incidentType, estimatedDamage } = this.incidentGroup.getRawValue();
      await this.incidentReportService.updateIncident(this.reportId!, {
        incidentType: incidentType!,
        estimatedDamage: estimatedDamage!,
      });
    });
  }

  protected back(): void {
    this.errorMessage.set(null);
    this.currentStep.set((this.currentStep() - 1) as Step);
  }

  protected async submit(): Promise<void> {
    await this.saveStep(this.specificsGroup, async () => {
      const { incidentDate, location, description } = this.specificsGroup.getRawValue();
      await this.incidentReportService.updateDetails(this.reportId!, {
        incidentDate: incidentDate!,
        location: location!,
        description: description!,
      });
      this.submitted.set(true);
    });
  }

  protected startNewReport(): void {
    this.form.reset();
    this.reportId = null;
    this.currentStep.set(1);
    this.submitted.set(false);
    this.errorMessage.set(null);
  }

  private async saveStep(group: AbstractControl, save: () => Promise<void>): Promise<void> {
    if (group.invalid) {
      group.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.errorMessage.set(null);

    try {
      await save();
      if (this.currentStep() < 3) {
        this.currentStep.set((this.currentStep() + 1) as Step);
      }
    } catch {
      this.errorMessage.set("Something went wrong saving your report. Please try again.");
    } finally {
      this.saving.set(false);
    }
  }
}
