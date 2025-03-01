import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BootstrapDebuggerComponent } from './bootstrap-debugger.component';

describe('BootstrapDebuggerComponent', () => {
  let component: BootstrapDebuggerComponent;
  let fixture: ComponentFixture<BootstrapDebuggerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BootstrapDebuggerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(BootstrapDebuggerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
