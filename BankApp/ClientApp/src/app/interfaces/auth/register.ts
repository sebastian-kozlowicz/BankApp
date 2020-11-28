import { AddressCreation } from "../address/address-creation";
import { UserCreationBySameUser } from "../user/user-creation-by-same-user";

export interface Register {
  user: UserCreationBySameUser;
  address: AddressCreation;
}
