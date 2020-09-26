import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BranchWithAddressCreation } from '../interfaces/branch/with-address/branch-with-address-creation';

@Injectable({
  providedIn: 'root'
})
export class BranchService {

  constructor(private http: HttpClient) { }

  private readonly branchesEndpoint = '/api/branches';

  createBranch(branch: BranchWithAddressCreation) {
    return this.http.post(this.branchesEndpoint, branch);
  }
}
