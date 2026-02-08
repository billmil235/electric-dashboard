import { Component, Input, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-consumption-chart',
  template: `
    <div class="chart-wrapper">
      <canvas #chartCanvas></canvas>
    </div>
  `,
  styleUrls: ['./consumption-chart.css']
})
export class ConsumptionChartComponent implements AfterViewInit, OnDestroy {
  @Input() chartData: any[] = [];
  @Input() loading = false;
  
  @ViewChild('chartCanvas') chartCanvas!: ElementRef<HTMLCanvasElement>;
  
  private chart: Chart | null = null;
  
  ngAfterViewInit() {
    this.createChart();
  }
  
  ngOnDestroy() {
    if (this.chart) {
      this.chart.destroy();
    }
  }
  
  private createChart() {
    const canvas = this.chartCanvas?.nativeElement;
    if (!canvas) {
      console.log('Canvas element not found');
      return;
    }
    
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      console.error('Canvas context not available');
      return;
    }
    
    // Create chart with placeholder data
    this.chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: [],
        datasets: [
          {
            label: 'Total Consumption (kWh)',
            data: [],
            backgroundColor: 'rgba(54, 162, 235, 0.7)',
            borderColor: 'rgba(54, 162, 235, 1)',
            borderWidth: 1,
            yAxisID: 'y'
          },
          {
            label: 'Total Sent Back (kWh)',
            data: [],
            backgroundColor: 'rgba(75, 192, 192, 0.7)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1,
            yAxisID: 'y'
          },
          {
            label: 'Total Billed Amount ($)',
            data: [],
            backgroundColor: 'rgba(255, 206, 86, 0.7)',
            borderColor: 'rgba(255, 206, 86, 1)',
            borderWidth: 1,
            yAxisID: 'y1'
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: true,
        scales: {
          x: {
            title: {
              display: true,
              text: 'Year'
            }
          },
          y: {
            type: 'linear' as const,
            display: true,
            position: 'left' as const,
            title: {
              display: true,
              text: 'kWh'
            },
            beginAtZero: true
          },
          y1: {
            type: 'linear' as const,
            display: true,
            position: 'right' as const,
            title: {
              display: true,
              text: 'Amount ($)'
            },
            beginAtZero: true,
            grid: {
              drawOnChartArea: false
            }
          }
        },
        plugins: {
          legend: {
            position: 'top' as const
          },
          tooltip: {
            callbacks: {
              label: (context) => {
                const label = context.dataset.label || '';
                const value = context.raw as number;
                if (label.includes('Amount')) {
                  return `${label}: $${value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
                }
                return `${label}: ${value.toLocaleString('en-US')}`;
              }
            }
          }
        }
      }
    });
    
    this.updateChart();
  }
  
  updateChart() {
    if (!this.chart) {
      console.log('Chart not initialized');
      return;
    }
    
    if (this.chartData.length === 0) {
      console.log('No data to display');
      this.clearChart();
      return;
    }
    
    const labels = this.chartData.map(entry => entry.year.toString());
    
    // Extract data for each dataset
    const consumptionData = this.chartData.map(entry => entry.totalConsumption);
    const sentBackData = this.chartData.map(entry => entry.totalSentBack);
    const billedAmountData = this.chartData.map(entry => entry.totalBilledAmount);
    
    // Update chart data
    this.chart.data.labels = labels;
    this.chart.data.datasets[0].data = consumptionData;
    this.chart.data.datasets[1].data = sentBackData;
    this.chart.data.datasets[2].data = billedAmountData;
    
    this.chart.update();
  }
  
  private clearChart() {
    if (this.chart) {
      this.chart.data.labels = [];
      this.chart.data.datasets[0].data = [];
      this.chart.data.datasets[1].data = [];
      this.chart.data.datasets[2].data = [];
      this.chart.update();
    }
  }
}