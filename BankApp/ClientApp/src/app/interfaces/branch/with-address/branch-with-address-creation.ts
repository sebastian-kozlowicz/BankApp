import { AddressCreation } from "../../address/address-creation";
import { BranchCreation } from "../branch-creation";

export interface BranchWithAddressCreation {
  branch: BranchCreation;
  address: AddressCreation;
}
