export default interface HallFormData {
  title: string | null;
  layoutRows: HallLayoutItem[][] | null;
}

export interface HallLayoutItem {
  rowID: number;
  columnID: number;
  typeID: HallLayoutItemType;
}

export enum HallLayoutItemType {
  EMPTY = 0,
  COMMON = 1,
  VIP = 2,
  COUCH = 3,
}
