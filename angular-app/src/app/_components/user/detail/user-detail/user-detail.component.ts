import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../../_services/auth.service';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../../_services/user.service';
import { UserClaim } from '../../../../_model/user-claim';
import { User } from '../../../../_model/user';
import { ApplicationConstants } from '../../../../_common/application-constants';

@Component({
  selector: 'app-user-detail',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './user-detail.component.html',
  styleUrl: './user-detail.component.css'
})
export class UserDetailComponent implements OnInit {

  private formBuilder = inject(FormBuilder);

  message: string = '';

  theForm = this.formBuilder.nonNullable.group({
    username: ['', Validators.required],
    claims: this.formBuilder.nonNullable.array<FormGroup<{
      claimType: FormControl<string>;
      claimValue: FormControl<string>
    }>>([])
  });

  constructor(
    private service: UserService,
    private route: ActivatedRoute,
    private router: Router) {
  }

  ngOnInit() {
    const ownerId = this.getRouteValue('ownerId');
    const id = this.getRouteValue('id');

    this.load(ownerId, id);
  }

  load(ownerId: string, id: string) {
    this.service.get(ownerId, id).subscribe({
      next: (result) => {
        if (result === null) {
          this.message = 'not found';
        }
        else {
          this.afterLoad(result);
        }
      },
      error: (error) => {
        console.error(error);
        this.message = error.message;
      }
    });
  }

  public addClaim() {
    let newClaim = new UserClaim();
    this.theForm.controls.claims.push(this.formBuilder.nonNullable.group({
      claimType: this.formBuilder.nonNullable.control(newClaim.claimType, Validators.required),
      claimValue: this.formBuilder.nonNullable.control(newClaim.claimValue, Validators.required)
    }));
  }

  public removeClaim(index: number) {
    if (this.theForm.controls.claims.controls.length == 0 ||
      this.theForm.controls.claims.controls.length < index) {
      return;
    }
    else {
      this.theForm.controls.claims.controls.splice(index, 1);
    }
  }

  public setClaimTypeToRole(index: number) {
    if (this.theForm.controls.claims.controls.length == 0 ||
      this.theForm.controls.claims.controls.length < index) {
      return;
    }
    else {
      var value = this.theForm.controls.claims.controls[index].value;
      value.claimType = ApplicationConstants.claimTypeRole;

      // this.theForm.controls.claims.controls

      this.theForm.controls.claims.at(index).patchValue(value);
    }
  }


  private afterLoad(result: User) {
    this.theForm.controls.username.setValue(result.email);

    result.claims.forEach((claim) => {
      this.theForm.controls.claims.push(this.formBuilder.nonNullable.group({
        claimType: this.formBuilder.nonNullable.control(claim.claimType, Validators.required),
        claimValue: this.formBuilder.nonNullable.control(claim.claimValue, Validators.required)
      }));
    });
  }

  getRouteValue(valueName: string): string {
    const snapshot = this.route.snapshot.paramMap;

    if (snapshot.has(valueName) === false) {
      throw new Error(`Value for ${valueName} does not exist`);
    }
    else {
      const returnValue = snapshot.get(valueName) as string;

      return returnValue;
    }
  }

  public save() {
    const ownerId = this.getRouteValue('ownerId');
    const id = this.getRouteValue('id');

    let user = new User();
    user.id = id;
    user.email = this.theForm.controls.username.value;
    user.claims = this.getUserClaims();

    this.service.save(ownerId, user).subscribe({
      next: (result) => {
        this.router.navigate(['/user']);
      },
      error: (error) => {
        console.error(error);
        this.message = error.message;
      }
    });
  }

  private getUserClaims(): UserClaim[] {
    var claims: UserClaim[] = [];

    this.theForm.controls.claims.controls.forEach((control) => {
      let claim = new UserClaim();
      claim.claimType = control.controls.claimType.value;
      claim.claimValue = control.controls.claimValue.value;

      claims.push(claim);
    });

    return claims;
  }

  public cancel() {
    this.router.navigate(['/user/list']);
  }
}
