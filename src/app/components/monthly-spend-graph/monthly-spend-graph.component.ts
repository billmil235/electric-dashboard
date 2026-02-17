import { Component, Input, ViewChild, ElementRef, AfterViewInit, OnDestroy, OnChanges, SimpleChanges } from '@angular/core';
import Chart from 'chart.js/auto';
import { ElectricBill } from '../../models/electric-bill.model';

@Component({
  selector: 'app-monthly-spend-graph',
  template: `
    <div class="chart-wrapper">
      <canvas #chartCanvas></canvas>
    </div>
  `,
  styleUrls: ['./monthly-spend-graph.component.css']
})
export class MonthlySpendGraphComponent implements AfterViewInit, OnDestroy, OnChanges {
  @Input() bills: ElectricBill[] = [];
  @Input() loading = false;
  
  private prevBillsLength = 0;
  
  @ViewChild('chartCanvas') chartCanvas!: ElementRef<HTMLCanvasElement>;
  
  private chart: Chart | null = null;
  
  ngAfterViewInit() {
    this.createChart();
  }
  
  ngOnChanges(changes: SimpleChanges) {
    const billsChanged = this.bills.length !== this.prevBillsLength;
    const loadingFinished = changes['loading'] && !this.loading;
    
    if (billsChanged || loadingFinished) {
      this.prevBillsLength = this.bills.length;
      this.updateChart();
    }
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
    
    this.chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        datasets: []
      },
      options: {
        responsive: true,
        maintainAspectRatio: true,
        scales: {
          x: {
            title: {
              display: true,
              text: 'Month'
            }
          },
          y: {
            type: 'linear' as const,
            display: true,
            position: 'left' as const,
            title: {
              display: true,
              text: 'Cost ($)'
            },
            beginAtZero: true
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
                return `${label}: $${value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
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
    
    if (this.bills.length === 0) {
      console.log('No data to display');
      this.clearChart();
      return;
    }
    
    const monthlyData = this.processBillsForMonthlyData(this.bills);
    
    const yearData: { [year: string]: { spend: number[] } } = {};
    
    monthlyData.forEach(entry => {
      if (!yearData[entry.year.toString()]) {
        yearData[entry.year.toString()] = { spend: Array(12).fill(0) };
      }
      
      if (entry.month && entry.spend !== undefined) {
        const monthIndex = entry.month - 1;
        yearData[entry.year.toString()].spend[monthIndex] = entry.spend;
      }
    });
    
    const datasets = Object.keys(yearData)
      .sort((a, b) => parseInt(a) - parseInt(b))
      .map((year, index) => {
        const yearColor = this.getColorForYear(year, index);
        return {
          label: `Year ${year}`,
          data: yearData[year].spend,
          backgroundColor: yearColor,
          borderColor: yearColor,
          borderWidth: 1
        };
      });
    
    this.chart.data.datasets = datasets;
    this.chart.update();
  }
  
  processBillsForMonthlyData(bills: ElectricBill[]) {
    const data = [];
    const currentYear = new Date().getFullYear();
    const threeYearsAgo = currentYear - 3;
    
    const monthlyTotals: { [key: string]: number } = {};
    
    bills.forEach(bill => {
      if (!bill.serviceYear || !bill.billedAmount) return;
      
      if (bill.serviceYear >= threeYearsAgo && bill.serviceYear <= currentYear) {
        const month = bill.serviceMonth || 1;
        const key = `${bill.serviceYear}-${month}`;
        
        if (!monthlyTotals[key]) {
          monthlyTotals[key] = 0;
        }
        
        monthlyTotals[key] += bill.billedAmount;
      }
    });
    
    for (let year = threeYearsAgo; year <= currentYear; year++) {
      for (let month = 1; month <= 12; month++) {
        const key = `${year}-${month}`;
        const spend = monthlyTotals[key] || 0;
        
        data.push({
          year: year,
          month: month,
          spend: spend
        });
      }
    }
    
    return data;
  }
  
  private getColorForYear(year: string, index: number) {
    const hue = (index * 120) % 360;
    return `hsla(${hue}, 70%, 60%, 0.7)`;
  }
  
  private clearChart() {
    if (this.chart) {
      this.chart.data.labels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
      this.chart.data.datasets = [];
      this.chart.update();
    }
  }
}
