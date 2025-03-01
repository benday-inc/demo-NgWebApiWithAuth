import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TextboxFieldComponent } from './textbox-field.component';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';

describe('TextboxFieldComponent', () => {
  let component: TextboxFieldComponent;
  let fixture: ComponentFixture<TextboxFieldComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TextboxFieldComponent],
      imports: [ReactiveFormsModule],      
    });
    fixture = TestBed.createComponent(TextboxFieldComponent);
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
