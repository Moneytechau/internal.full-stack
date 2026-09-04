export const INCIDENT_TYPES: string[] = [
  'Vehicle accident',
  'Property damage',
  'Theft',
  'Fire',
  'Water damage',
  'Other',
];

export interface StartIncidentReportRequest {
  fullName: string;
  mobile: string;
}

export interface UpdateIncidentRequest {
  incidentType: string;
  estimatedDamage: number;
}

export interface UpdateIncidentDetailsRequest {
  incidentDate: string;
  location: string;
  description: string;
}

export interface IncidentReportResponse {
  id: string;
  fullName: string;
  mobile: string;
  incidentType: string | null;
  estimatedDamage: number | null;
  incidentDate: string | null;
  location: string | null;
  description: string | null;
}
