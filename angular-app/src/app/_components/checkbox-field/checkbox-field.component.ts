import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-checkbox-field',
  templateUrl: './checkbox-field.component.html',
  styleUrl: './checkbox-field.component.css'
})
export class CheckboxFieldComponent {
  @Input()
  parentForm!: FormGroup;
  
  @Input()
  controlName!: string;

  @Input()
  displayName = '(displayName not set)';

  @Input()
  displayAsSwitch = false;

  @Input()
  isLarge = false;

  @Output() 
  selectedValueChanged = new EventEmitter();
  
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  selectionChanged(event: Event) {
    this.selectedValueChanged?.emit(this.parentForm.get(this.controlName)?.value);
  }
}