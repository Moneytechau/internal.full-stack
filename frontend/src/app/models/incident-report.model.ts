export interface IncidentReport {
  fullName: string;
  mobile: string;
  incidentType: string;
  estimatedDamage: number;
  incidentDate: string;
  location: string;
  description: string;
}

export const INCIDENT_TYPES: string[] = [
  'Vehicle accident',
  'Property damage',
  'Theft',
  'Fire',
  'Water damage',
  'Other',
];
