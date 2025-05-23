import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyRentalsComponent } from './myrentals.component';

describe('MyrentalsComponent', () => {
  let component: MyRentalsComponent;
  let fixture: ComponentFixture<MyRentalsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyRentalsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyRentalsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
