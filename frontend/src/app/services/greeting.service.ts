import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { Greeting } from '../models/greeting.model';

@Injectable({ providedIn: 'root' })
export class GreetingService {
  private http = inject(HttpClient);

  public async getGreeting(): Promise<Greeting> {
    return await firstValueFrom(this.http.get<Greeting>(`${environment.apiBaseUrl}/api/greeting`));
  }
}
