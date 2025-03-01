import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComboboxFieldComponent } from './combobox-field.component';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';

describe('ComboboxFieldComponent', () => {
  let component: ComboboxFieldComponent;
  let fixture: ComponentFixture<ComboboxFieldComponent>;
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ComboboxFieldComponent],
      imports: [ReactiveFormsModule],
    });
    fixture = TestBed.createComponent(ComboboxFieldComponent);
    component = fixture.componentInstance;
    component.controlName = 'testField';
    component.parentForm = new FormGroup({
      testField: new FormControl('', Validators.required)
    });
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
