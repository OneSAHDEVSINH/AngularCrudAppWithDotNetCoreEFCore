export interface Student {
  id?: number;
  name: string;
  age: number;
  profilePicture?: string | undefined | null; // base64 string from file input
  addresses?: Address[];
}

export interface Address {
  addressID?: number;
  addressLine: string;
  city: string;
  state: string;
  pinCode: string;
  studentId: number;
}
