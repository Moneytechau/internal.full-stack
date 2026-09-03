import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Greeting } from '../../models/greeting.model';
import { GreetingService } from '../../services/greeting.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent {
  private greetingService = inject(GreetingService);

  protected greeting = signal<Greeting | null>(null);
  protected loading = signal(true);
  protected error = signal(false);

  constructor() {
    this.loadGreeting();
  }

  private async loadGreeting(): Promise<void> {
    this.loading.set(true);
    this.error.set(false);

    try {
      const greeting = await this.greetingService.getGreeting();
      this.greeting.set(greeting);
    } catch {
      this.error.set(true);
    } finally {
      this.loading.set(false);
    }
  }
}
