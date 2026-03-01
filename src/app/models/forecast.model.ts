export interface Forecast {
  addressId: string;
  forecastYear: number;
  forecastMonth: number;
  predictedKwh: number;
  algorithmUsed: string;
  confidence: number;
}
