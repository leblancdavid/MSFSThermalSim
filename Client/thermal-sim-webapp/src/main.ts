import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

// Adds a new app-root to the DOM
// If your selector prefix is different, make sure you adjust this.
const appRoot = document.createElement('app-root'); 
document.body.appendChild(appRoot);

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
