export default interface CheckTicketResponse {
  isValid: boolean;
  errorCode: CheckTicketErrorCode | null;
}

export enum CheckTicketErrorCode {
  NOT_EXISTING = 1,
  NOT_PAYED_FOR = 2,
  WRONG_HALL = 3,
}
