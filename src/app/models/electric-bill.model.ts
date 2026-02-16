export interface ElectricBill {
  addressId?: string | null;
  periodStartDate?: string;
  periodEndDate?: string;
  consumptionKwh?: number;
  sentBackKwh?: number | null;
  billedAmount?: number;
  unitPrice?: number | null;
  serviceYear?: number | null;
  serviceMonth?: number | null;
  billId?: string | null;
}