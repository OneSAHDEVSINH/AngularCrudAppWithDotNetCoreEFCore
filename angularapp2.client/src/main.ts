import { platformBrowser } from '@angular/platform-browser';
//import { StudentModule } from './app/app.module';
//import "@angular/compiler"
//platformBrowser().bootstrapModule(StudentModule, {
//  ngZoneEventCoalescing: true,
//})
//  .catch(err => console.error(err));
//import { bootstrapApplication } from '@angular/platform-browser';
//import { StudentComponent } from './app/app.component';
//import "@angular/compiler";

//bootstrapApplication(StudentComponent, {
//  ngZoneEventCoalescing: true,
//})
//  .catch(err => console.error(err));
import { bootstrapApplication } from '@angular/platform-browser';
import { StudentComponent } from './app/app.component';
import "@angular/compiler";
import { provideHttpClient } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { AppRoutingModule } from './app/app-routing.module';
import { StudentService } from './app/student.service';
import { AddressService } from './app/address.service';


bootstrapApplication(StudentComponent, {
  providers: [
    provideHttpClient(),
    importProvidersFrom(AppRoutingModule),
    StudentService,
    AddressService
  ]
})
  .catch(err => console.error(err));

