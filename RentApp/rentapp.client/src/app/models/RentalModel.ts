export interface RentalRequest {
  productId: number;
  startDate: string;
  endDate: string;
}

export interface RentalResponse {
  message: string;
  rentalId?: number;
}
