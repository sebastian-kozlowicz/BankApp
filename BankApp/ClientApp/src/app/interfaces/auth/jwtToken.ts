import { UserRole } from "../../enumerators/userRole";

export interface JwtToken {
  sub: string;
  jti: string;
  iat: number;
  userId: number;
  role: Array<UserRole>;
  nbf: number;
  exp: number;
  iss: string;
  aud: string;
}
