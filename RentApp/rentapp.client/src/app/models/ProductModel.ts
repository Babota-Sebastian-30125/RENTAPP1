export interface Product {
  id: number;
  name: string;
  category: string;
  location: string; 
  description: string;
  pricePerDay: number;
  available: boolean;
  addedAt: Date;
  userName: string;
  telephoneNumber?: string;
  imagePath?: string;
}
