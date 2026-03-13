import { Component, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SolarGenerationService } from '../../services/solar-generation.service';
import { LoggedInLayout } from '../logged-in-layout/logged-in-layout';
import { SolarDataDto } from '../../services/solar-generation.service';
import { ServiceAddressSelector } from '../../components/service-address-selector/service-address-selector';

@Component({
  selector: 'app-solar-generation',
  imports: [LoggedInLayout, CommonModule, FormsModule, ServiceAddressSelector],
  templateUrl: './solar-generation.html',
  styleUrls: ['./solar-generation.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SolarGeneration {
  headers = signal<string[]>([]);
  selectedDateCol = signal<string | null>(null);
  selectedValueCol = signal<string | null>(null);
  file: File | null = null;
  data = signal<SolarDataDto[]>([]);
  private _selectedAddressId = '';

  constructor(private solarService: SolarGenerationService) {}

  onAddressSelected(addressId: string) {
    this._selectedAddressId = addressId;
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.file = input.files[0];
      console.log('File selected:', this.file.name);
      this.loadHeaders();
    }
  }

  loadHeaders() {
    if (this.file) {
      this.solarService
        .uploadHeader(this.file)
        .subscribe({
          next: (resp: { headers: string[] }) => {
            console.log('API Response:', resp);
            this.headers.set(resp.headers);
          },
          error: (err: Error) => {
            console.error('Error loading headers:', err);
          }
        });
    }
  }

  setDateColumn(col: string) {
    this.selectedDateCol.set(col);
  }

  setValueColumn(col: string) {
    this.selectedValueCol.set(col);
  }

  submitFile() {
    const dateCol = this.selectedDateCol();
    const valueCol = this.selectedValueCol();
    if (this.file && dateCol && valueCol) {
      this.solarService
        .uploadData(this.file, dateCol, valueCol)
        .subscribe((resp: SolarDataDto[]) => {
          this.data.set(resp);
        });
    }
  }
}