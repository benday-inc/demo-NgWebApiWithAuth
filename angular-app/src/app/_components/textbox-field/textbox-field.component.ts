import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ControlContainer, FormGroup, NgForm } from '@angular/forms';

@Component({
  selector: 'app-textbox-field',
  templateUrl: './textbox-field.component.html',
  styleUrls: ['./textbox-field.component.css'],
  viewProviders: [{ provide: ControlContainer, useExisting: NgForm }],
})
export class TextboxFieldComponent {
  @Input()
  parentForm!: FormGroup;
  
  @Input()
  controlName!: string;

  @Input()
  displayName = '(displayName not set)';

  @Output() 
  changed = new EventEmitter();

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  valueChanged(event: Event) {
    this.changed?.emit(this.parentForm.get(this.controlName)?.value);
  }
}
