import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RentProductComponent } from './rent-product.component';

describe('RentProductComponent', () => {
  let component: RentProductComponent;
  let fixture: ComponentFixture<RentProductComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RentProductComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RentProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
