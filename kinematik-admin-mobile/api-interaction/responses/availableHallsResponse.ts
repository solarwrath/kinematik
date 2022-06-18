export default interface AvailableHallsResponse {
  halls: AvailableHall[];
}

export interface AvailableHall {
  id: number;
  title: string;
}
