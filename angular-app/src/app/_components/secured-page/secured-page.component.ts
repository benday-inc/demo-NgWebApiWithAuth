import { Component, OnInit } from '@angular/core';
import { SecuredPageService } from '../../_services/secured-page.service';
import { CommonUtilities } from '../../_common/common-utilities';
import { ApplicationConstants } from '../../_common/application-constants';

@Component({
  selector: 'app-secured-page',
  imports: [],
  templateUrl: './secured-page.component.html',
  styleUrl: './secured-page.component.css'
})
export class SecuredPageComponent implements OnInit {
  public message: string = ApplicationConstants.defaultString;

  constructor(private service: SecuredPageService) {
  }

  ngOnInit() {
    this.refresh();
  }

  public refresh() {
    this.message = ApplicationConstants.defaultString;
    
    this.service.getProtectedData().subscribe({
      next: (response) => {
        console.log(response);
        this.message = response.message;
      },
      error: (err) => {
        console.error(err);
        this.message = CommonUtilities.formatErrorMessage(err);
      }
    });
  }
}
