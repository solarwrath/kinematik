import CheckTicketResponse from "./responses/checkTicketResponse";
import AvailableHallsResponse from "./responses/availableHallsResponse";
import Hall from "../domain/hall";
import axios from "axios";

axios.defaults.baseURL = "https://9f23-81-24-208-241.eu.ngrok.io";

export async function getAvailableHalls() {
  const response = await axios.get("/api/halls");
  const parsedResponse: AvailableHallsResponse = response.data;

  return parsedResponse.halls.map((availableHall) => {
    return {
      id: availableHall.id,
      title: availableHall.title,
    } as Hall;
  });
}

export async function checkTicket(bookingID: number, checkedHallID: number) {
  const response = await axios.get(
    `/api/bookings/check/${bookingID}/${checkedHallID}`
  );
  const parsedResponse: CheckTicketResponse = response.data;

  return {
    isValid: parsedResponse.isValid,
    errorCode: parsedResponse.errorCode,
  };
}

export default {
  getAvailableHalls,
  checkTicket,
};
