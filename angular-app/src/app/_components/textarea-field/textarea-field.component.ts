import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-textarea-field',
  templateUrl: './textarea-field.component.html',
  styleUrl: './textarea-field.component.css'
})
export class TextareaFieldComponent {
  @Input()
  parentForm!: FormGroup;
  
  @Input()
  controlName!: string;

  @Input()
  rows: number = 10;

  @Input()
  displayName = '(displayName not set)';

  @Input()
  placeholder = '';

  @Input()
  labelIsBold = 'false';

  @Output() 
  changed = new EventEmitter();

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  valueChanged(event: Event) {
    this.changed?.emit(this.parentForm.get(this.controlName)?.value);
  }
}
