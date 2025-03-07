import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { environment } from './environments/environment.development';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));

  export function getBaseUrl() {

    let returnValue = environment.baseUrl;
  
    const temp = document.getElementsByTagName('base')[0].href;
  
    if (temp === undefined || temp === null || temp === '') {
      returnValue = environment.baseUrl;
    }
    else if (temp.startsWith('https://localhost:4200') === true) {
      returnValue = environment.baseUrl;
    }
    // else if (temp.startsWith('https://www.slidespeaker.ai') === true) {
    //   const temp2 = temp.replace('https://www.', 'https://api.');
    //   returnValue = temp2;
    // }    
  
    console.log(`getBaseUrl(): ${returnValue}`);
    
    return returnValue;
  }
