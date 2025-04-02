
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
/*import { AppComponent } from './app.component';*/
import { StudentComponent } from '../app/app.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { AppRoutingModule } from './app-routing.module';

@NgModule({
  declarations: [
    /*AppComponent,*/
    StudentComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [StudentComponent]
  //bootstrap: []
})
export class StudentModule { }
