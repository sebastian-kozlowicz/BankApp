import { Address } from "../address/address";

export interface Branch {
  id: number;
  branchCode: string;
  address: Address;
}
