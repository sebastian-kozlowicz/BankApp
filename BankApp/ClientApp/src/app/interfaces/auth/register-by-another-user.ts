import { AddressCreation } from "../address/address-creation";
import { UserCreationByAnotherUser } from "../user/user-creation-by-another-user";

export interface RegisterByAnotherUser {
  user: UserCreationByAnotherUser;
  address: AddressCreation;
}
