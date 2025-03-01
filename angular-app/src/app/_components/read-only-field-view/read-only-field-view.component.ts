import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-read-only-field-view',
  templateUrl: './read-only-field-view.component.html',
  styleUrl: './read-only-field-view.component.css'
})
export class ReadOnlyFieldViewComponent {
  @Input()
  parentForm!: FormGroup;
  
  @Input()
  controlName!: string;

  @Input()
  displayName = '(displayName not set)';
}
