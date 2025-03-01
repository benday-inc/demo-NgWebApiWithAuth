import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReadOnlyFieldViewComponent } from './read-only-field-view.component';

describe('ReadOnlyFieldViewComponent', () => {
  let component: ReadOnlyFieldViewComponent;
  let fixture: ComponentFixture<ReadOnlyFieldViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ReadOnlyFieldViewComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ReadOnlyFieldViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
