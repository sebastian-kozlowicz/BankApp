import { AddressCreation } from "../address/address-creation";
import { UserCreation } from "../user/user-creation";

export interface Register {
  user: UserCreation;
  address: AddressCreation;
}
