import { Component } from '@angular/core';
import { LoggedInLayout } from '../logged-in-layout/logged-in-layout';

@Component({
  selector: 'app-solar-generation',
  imports: [LoggedInLayout],
  templateUrl: './solar-generation.html',
  styleUrls: ['./solar-generation.css'],
})
export class SolarGeneration {
  title = 'Solar Generation';
}

