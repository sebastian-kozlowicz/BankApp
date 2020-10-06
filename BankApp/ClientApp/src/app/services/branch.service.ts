import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BranchWithAddressCreation } from '../interfaces/branch/with-address/branch-with-address-creation';
import { Branch } from "../interfaces/branch/branch";
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BranchService {

  constructor(private http: HttpClient) { }

  private readonly branchesEndpoint = '/api/branches';

  createBranch(branch: BranchWithAddressCreation): Observable<Branch> {
    return this.http.post<Branch>(this.branchesEndpoint + "/CreateWithAddress", branch);
  }
}
