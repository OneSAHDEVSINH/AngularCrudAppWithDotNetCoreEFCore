import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { StudentService } from '../app/student.service';
import { AddressService } from '../app/address.service';
import { Student, Address } from '../app/student.model';
import { FormBuilder, FormGroup, Validators, FormArray, ReactiveFormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';

declare var bootstrap: any; // to access Bootstrap modal

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [
    CommonModule,  // Add CommonModule to imports
    ReactiveFormsModule  // Add ReactiveFormsModule for form controls
  ],
  providers: [
    StudentService,
    AddressService
  ]
})
export class StudentComponent implements OnInit {
  students: Student[] = [];
  selectedStudent: Student | null = null;
  studentForm!: FormGroup;
  modalTitle: string = '';
  errorMessage: string = '';

  @ViewChild('studentModal') studentModal!: ElementRef;
  modalInstance: any;

  constructor(
    private studentService: StudentService,
    private addressService: AddressService,
    private fb: FormBuilder
  ) { }
  
  ngOnInit(): void {
    this.loadStudents();
    this.initializeForm();
  }

  loadStudents() {
    this.studentService.getStudents().subscribe(
      (data) => {
        console.log('API response:', data);
        if (Array.isArray(data)) {
          this.students = data;
        } else {
          console.error('Expected an array but got:', data);
          this.students = [];
        }
      },
      error => {
        console.error('Error loading students:', error);
        this.students = [];
      }
    );
  }

  initializeForm() {
    this.studentForm = this.fb.group({
      id: [null],
      name: ['', Validators.required],
      age: [null, Validators.required],
      profilePicture: [null],
      addresses: this.fb.array([])
    });
  }

  get addressesFormArray(): FormArray {
    return this.studentForm.get('addresses') as FormArray;
  }

  openAddModal() {
    this.modalTitle = "Add Student";
    this.selectedStudent = null;
    this.studentForm.reset();
    this.addressesFormArray.clear();
    this.errorMessage = '';

    this.modalInstance = new bootstrap.Modal(this.studentModal.nativeElement, {
      backdrop: 'static',
      keyboard: false
    });
    this.modalInstance.show();
  }

  openEditModal(student: Student) {
    this.modalTitle = "Edit Student";
    this.selectedStudent = student;
    this.errorMessage = '';
    // Patch basic student info
    this.studentForm.patchValue({
      id: student.id,
      name: student.name,
      age: student.age,
      profilePicture: student.profilePicture
    });
    this.addressesFormArray.clear();
    if (student.addresses && student.addresses.length > 0) {
      student.addresses.forEach(address => {
        this.addressesFormArray.push(this.fb.group({
          addressId: [address.addressID],
          addressLine: [address.addressLine, Validators.required],
          city: [address.city, Validators.required],
          state: [address.state, Validators.required],
          pinCode: [address.pinCode, Validators.required],
          studentId: [address.studentId]
        }));
      });
    }

    this.modalInstance = new bootstrap.Modal(this.studentModal.nativeElement, {
      backdrop: 'static',
      keyboard: false
    });
    this.modalInstance.show();
  }

  openDetailsModal(student: Student) {
    this.modalTitle = "Student Details";
    this.selectedStudent = student;
    this.errorMessage = '';
    this.studentForm.patchValue({
      id: student.id,
      name: student.name,
      age: student.age,
      profilePicture: student.profilePicture
    });
    this.addressesFormArray.clear();
    if (student.addresses && student.addresses.length > 0) {
      student.addresses.forEach(address => {
        this.addressesFormArray.push(this.fb.group({
          addressId: [address.addressID],
          addressLine: [address.addressLine, Validators.required],
          city: [address.city, Validators.required],
          state: [address.state, Validators.required],
          pinCode: [address.pinCode, Validators.required],
          studentId: [address.studentId]
        }));
      });
    }
    // Disable form controls in details view
    this.studentForm.disable();
    this.modalInstance = new bootstrap.Modal(this.studentModal.nativeElement, {
      backdrop: 'static',
      keyboard: false
    });
    this.modalInstance.show();
  }

  closeModal() {
    this.studentForm.enable();
    if (this.modalInstance)
      this.modalInstance.hide();
  }
  // Convert file to byte array
  
  onFileChange(event: any) {
    const file = event.target.files && event.target.files[0];
    if (file) {
      // Validate file type and size
      if (!file.type.match('image.*')) {
        console.error('Only image files are allowed');
        return;
      }

      if (file.size > 5000000) { // 5MB limit
        console.error('File is too large, max 5MB allowed');
        return;
      }

      const reader = new FileReader();
      // Read as data URL (which gives a base64 string)
      reader.readAsDataURL(file);
      reader.onload = () => {
        // This will be a base64 data URL
        console.log('File loaded as base64');
        this.studentForm.patchValue({
          profilePicture: reader.result
        });
      };
      reader.onerror = (error) => {
        console.error('Error reading file:', error);
      };
    }
  }


  saveStudent() {
    if (this.studentForm.invalid) {
      this.studentForm.markAllAsTouched();
      console.log('Form is invalid:', this.studentForm.errors);
      return;
    }
    const studentData: Student = this.studentForm.value;
    if (this.selectedStudent) {
      // Update student
      this.studentService.updateStudent(studentData).subscribe(
        (response) => {
          console.log('Update response:', response);
          this.loadStudents();
          this.modalInstance.hide();
        },
        (error: HttpErrorResponse) => {
          console.error('Update error:', error);
          if (error.error && typeof error.error === 'string') {
            this.errorMessage = error.error;
          } else {
            this.errorMessage = "Error updating student.";
          }
        }
      );
    } else {
      // Create new student
      this.studentService.createStudent(studentData).subscribe(
        (response) => {
          console.log('Create response:', response);
          this.loadStudents();
          this.modalInstance.hide();
          setTimeout(() => this.loadStudents(), 100);
        },
        (error: HttpErrorResponse) => {
          console.error('Create error:', error);
          if (error.error && typeof error.error === 'string') {
            this.errorMessage = error.error;
          } else {
            this.errorMessage = "Error creating student.";
          }
        }
      );
    }
  }

  deleteStudent(student: Student) {
    if (confirm("Are you sure you want to delete this student?")) {
      this.studentService.deleteStudent(student.id!).subscribe(
        () => this.loadStudents(),
        error => console.error(error)
      );
    }
  }

  // Manage dynamic addresses in the form
  addAddress() {
    this.addressesFormArray.push(this.fb.group({
      addressId: [null],
      addressLine: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      pinCode: ['', Validators.required],
      //studentId: this.studentForm.get('id')?.value || 0
    }));
  }

  removeAddress(index: number) {
    this.addressesFormArray.removeAt(index);
  }
}
