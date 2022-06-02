export interface HallLayoutItem {
    rowID: number;
    columnID: number;
    seatType: SeatType;
}

export enum SeatType {
    EMPTY = 0,
    COMMON = 1,
    VIP = 2,
    COUCH = 3,
}

export const widthRegistry = {
    [SeatType.EMPTY]: 1,
    [SeatType.COMMON]: 1,
    [SeatType.VIP]: 1,
    [SeatType.COUCH]: 2,
};
 