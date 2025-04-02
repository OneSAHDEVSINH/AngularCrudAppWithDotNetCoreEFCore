import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpInterceptor, HttpHandler, HttpEvent, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Student, Address } from '../app/student.model';
import { Observable, map, catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class StudentService {
  private apiUrl = '/students';
  //private apiUrl = 'https://localhost:7001/api/students';
  constructor(private http: HttpClient) { }

  //getStudents(): Observable<Student[]> {
  //  return this.http.get<Student[]>(this.apiUrl);
  //}

  //getStudent(id: number): Observable<Student> {
  //  return this.http.get<Student>(`${this.apiUrl}/${id}`);
  //}
 
  getStudents(): Observable<Student[]> {
  return this.http.get<any>(this.apiUrl).pipe(
    map(response => {
      console.log("Raw students response:", response);
      
      // Check if response is already an array
      if (Array.isArray(response)) {
        return response;
      }
      
      // Check if response has $values property (JSON.NET style)
      else if (response && response.$values && Array.isArray(response.$values)) {
        return response.$values;
      }
      
      // If it's an object but not an array, check various properties
      else if (response) {
        if (Array.isArray(response.value)) {
          return response.value;
        } else if (Array.isArray(response.items)) {
          return response.items;
        } else if (Array.isArray(response.data)) {
          return response.data;
        } else if (response.id !== undefined) {
          // It might be a single student object
          return [response];
        }
      }
      
      // Default to empty array if we can't determine the structure
      return [];
    }),
    catchError((error: HttpErrorResponse) => {
      console.error('GET Students Error details:', {
        status: error.status,
        message: error.message,
        error: error.error,
        url: error.url,
        headers: error.headers.keys().map(k => `${k}: ${error.headers.get(k)}`)
      });

      // Log the full error for debugging
      console.error('Full error object:', error);

      return throwError(() => error);
    })
  );
}

  getStudent(id: number): Observable<Student> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      map(response => response)
    );
  }

  //createStudent(student: Student): Observable<Student> {
  //  const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
  //  return this.http.post<Student>(this.apiUrl, student);
  //}

  //createStudent(student: Student): Observable<Student> {
  //  // Create FormData object to send both JSON data and file
  //  const formData = new FormData();

  //  // Extract profile picture from student object
  //  const profilePictureBase64 = student.profilePicture;

  //  // Remove profile picture from student object
  //  const studentToSend = {
  //    name: student.name,
  //    age: student.age,
  //    addresses: student.addresses || []
  //  };

  //  // Add student as JSON string
  //  formData.append('student', new Blob([JSON.stringify(studentToSend)],
  //    { type: 'application/json' }));

  //  // If there's a profile picture, convert from base64 and add to FormData
  //  if (profilePictureBase64 && typeof profilePictureBase64 === 'string') {
  //    // Convert base64 string to blob
  //    const byteString = atob(profilePictureBase64.split(',')[1]);
  //    const mimeString = profilePictureBase64.split(',')[0].split(':')[1].split(';')[0];
  //    const ab = new ArrayBuffer(byteString.length);
  //    const ia = new Uint8Array(ab);
  //    for (let i = 0; i < byteString.length; i++) {
  //      ia[i] = byteString.charCodeAt(i);
  //    }
  //    const blob = new Blob([ab], { type: mimeString });

  //    // Add to FormData
  //    formData.append('ProfilePicture', blob, 'profile-picture.jpg');
  //  }

  //  console.log('Sending student with FormData');

  //  // Send without setting Content-Type header (browser will set the correct boundary)
  //  return this.http.post<Student>(this.apiUrl, formData)
  //    .pipe(
  //      catchError((error: HttpErrorResponse) => {
  //        console.error('POST Error details:', {
  //          status: error.status,
  //          message: error.message,
  //          error: error.error,
  //          url: error.url
  //        });
  //        return throwError(() => error);
  //      })
  //    );
  //}

  createStudent(student: Student): Observable<Student> {
    const studentToSend: any = {
      name: student.name,
      age: student.age,
      profilePicture: undefined,
      addresses: student.addresses || []
    };
    // Handle profile picture conversion correctly
    if (student.profilePicture && typeof student.profilePicture === 'string') {
      try {
        // For base64 images, we need to extract just the data part
        const base64String = student.profilePicture;

        // If it's a data URL (starts with "data:"), extract just the base64 part
        if (base64String.startsWith('data:')) {
          // Extract just the base64 part (without the data URL prefix)
          const base64Data = base64String.substring(base64String.indexOf(',') + 1);
          // Convert base64 to byte array on the client side
          studentToSend.profilePicture = base64Data;
        } else {
          // If it's already just base64, use it as is
          studentToSend.profilePicture = base64String;
        }

        console.log('Successfully processed profile picture');
      } catch (error) {
        console.error('Error processing profile picture:', error);
        studentToSend.profilePicture = undefined;
      }
    }
    if (studentToSend.addresses && studentToSend.addresses.length > 0) {
      studentToSend.addresses = studentToSend.addresses.map((addr: Address) => ({
        addressLine: addr.addressLine,
        city: addr.city,
        state: addr.state,
        pinCode: addr.pinCode,
        studentId: 0
      }));
    }
    
    // Actually use the headers in the request
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    console.log('Updating student:', JSON.stringify({
      ...studentToSend,
      profilePicture: studentToSend.profilePicture ? '[BASE64 DATA]' : null
    }));

    // Log the student object before sending for debugging
    //console.log('Sending student:', JSON.stringify(studentToSend));

    return this.http.post<Student>(this.apiUrl, studentToSend, { headers })
      .pipe(
        catchError((error: HttpErrorResponse) => {
          console.error('POST Error details:', {
            status: error.status,
            message: error.message,
            error: error.error,
            url: error.url,
            headers: error.headers.keys().map(k => `${k}: ${error.headers.get(k)}`)
          });
          return throwError(() => error);
        })
      );
  }
  updateStudent(student: Student): Observable<any> {
    const studentToSend: any = {
      name: student.name,
      age: student.age,
      profilePicture: undefined,
      addresses: student.addresses || [],
      id: student.id
    };

    // Handle profile picture conversion correctly
    if (student.profilePicture && typeof student.profilePicture === 'string') {
      try {
        // For base64 images, we need to extract just the data part
        const base64String = student.profilePicture;

        // If it's a data URL (starts with "data:"), extract just the base64 part
        if (base64String.startsWith('data:')) {
          // Extract just the base64 part (without the data URL prefix)
          const base64Data = base64String.substring(base64String.indexOf(',') + 1);
          // Use the base64 data without the prefix
          studentToSend.profilePicture = base64Data;
        } else {
          // If it's already just base64, use it as is
          studentToSend.profilePicture = base64String;
        }

        console.log('Successfully processed profile picture for update');
      } catch (error) {
        console.error('Error processing profile picture:', error);
        studentToSend.profilePicture = undefined;
      }
    }
    // Ensure addresses is properly formatted
    if (studentToSend.addresses && studentToSend.addresses.length > 0) {
      studentToSend.addresses = studentToSend.addresses.map((addr: Address) => ({
        addressId: addr.addressID || 0,
        addressLine: addr.addressLine,
        city: addr.city,
        state: addr.state,
        pinCode: addr.pinCode,
        studentId: student.id || 0
      }));
    }
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    console.log('Updating student:', JSON.stringify({
      ...studentToSend,
      profilePicture: studentToSend.profilePicture ? '[BASE64 DATA]' : null
    }));

    
   // Log what we're sending for debugging
    //console.log('Updating student:', JSON.stringify(studentToSend));

    return this.http.put(`${this.apiUrl}/${student.id}`, studentToSend, { headers })
      .pipe(
        catchError((error: HttpErrorResponse) => {
          console.error('PUT Error details:', {
            status: error.status,
            message: error.message,
            error: error.error,
            url: error.url
          });
          return throwError(() => error);
        })
      );
  }

  deleteStudent(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
