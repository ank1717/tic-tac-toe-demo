import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './landing.html',
  styleUrls: ['./landing.css']
})
export class LandingComponent {
  selectedMode = 'TwoPlayer';

  constructor(private router: Router) {}

  start() {
    // Navigate to /game with selected mode as query param
    this.router.navigate(['/game'], { queryParams: { mode: this.selectedMode } });
  }
}
