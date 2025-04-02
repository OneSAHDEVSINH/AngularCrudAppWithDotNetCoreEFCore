import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Address } from '../app/student.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  private apiUrl = '/Addresses';
  //private apiUrl = 'https://localhost:7001/api/addresses';
  constructor(private http: HttpClient) { }

  getAddressesByStudent(studentId: number): Observable<Address[]> {
    return this.http.get<Address[]>(`${this.apiUrl}/ByStudent/${studentId}`);
  }

  createAddress(address: Address): Observable<Address> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post<Address>(this.apiUrl, address);
  }

  updateAddress(address: Address): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.put(`${this.apiUrl}/${address.addressID}`, address);
  }

  deleteAddress(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
