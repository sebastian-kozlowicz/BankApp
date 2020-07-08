export interface Register {
  user: {
    name: string,
    surname: string,
    email: string,
    phoneNumber: string,
    password: string
  };

  address: {
    country: string,
    city: string,
    street: string,
    houseNumber: string,
    apartmentNumber: string,
    postalCode: string
  };
}
