export interface JwtToken {
  sub: string;
  jti: string;
  iat: number;
  userId: number;
  role: Array<string>;
  nbf: number;
  exp: number;
  iss: string;
  aud: string;
}
